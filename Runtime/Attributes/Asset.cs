namespace ElRaccoone.EntityComponentSystem {

  /// <summary>
  /// Describes an asset which should be assigned automatically.
  /// </summary>
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Asset : System.Attribute {

    /// <summary>
    /// Defines whether the field name should be used for getting the asset from
    /// the controller. When set to false, the asset name string will be used
    /// instead.
    /// </summary>
    private bool useFieldNameAsAssetName = false;

    /// <summary>
    /// The asset name will be used when defined that the field name should not
    /// be used for finding the asset name. Thus using this asset name property
    /// instead.
    /// </summary>
    private string assetName = "";

    /// <summary>
    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// name of the property field.
    /// </summary>
    public Asset () {
      this.useFieldNameAsAssetName = true;
      this.assetName = "";
    }

    /// <summary>
    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// given asset name.
    /// </summary>
    /// <param name="assetName">The name of the Asset.</param>
    public Asset (string assetName) {
      this.useFieldNameAsAssetName = false;
      this.assetName = assetName;
    }

    /// <summary>
    /// Sets the attributes values on an object using reflection.
    /// </summary>
    /// <param name="target">Target object.</param>
    internal static void SetAttributeValues (System.Object target) {
      // Finds all fields with the Asset attribute.
      var type = target.GetType ();
      var fields = type.GetFields (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      foreach (var field in fields) {
        // Getting the field attribute where the type is Asset.
        var assetFieldAttribute = System.Attribute.GetCustomAttribute (field, typeof (Asset)) as Asset;
        if (assetFieldAttribute != null) {
          // If the field attribute is not null, then the field is an asset.
          // Getting the asset from the controller, using the name of the field
          // as the asset name and assigning the asset to the field.
          if (assetFieldAttribute.useFieldNameAsAssetName == true) {
            // If no asset name is defined, then use the field name as the asset
            // name.
            field.SetValue (target, Controller.Instance.GetAsset (field.Name));
          } else {
            // If an asset name is defined, then use the asset name.
            field.SetValue (target, Controller.Instance.GetAsset (assetFieldAttribute.assetName));
          }
        }
      }
    }
  }
}