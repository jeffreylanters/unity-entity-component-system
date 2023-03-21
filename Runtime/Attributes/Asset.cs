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
    bool useFieldNameAsAssetName = false;

    /// <summary>
    /// The asset name will be used when defined that the field name should not
    /// be used for finding the asset name. Thus using this asset name property
    /// instead.
    /// </summary>
    string assetName = "";

    /// <summary>
    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// name of the property field.
    /// </summary>
    public Asset () {
      useFieldNameAsAssetName = true;
      assetName = "";
    }

    /// <summary>
    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// given asset name.
    /// </summary>
    /// <param name="assetName">The asset name.</param>
    public Asset (string assetName) {
      useFieldNameAsAssetName = false;
      this.assetName = assetName;
    }

    /// <summary>
    /// Sets the attributes values on an object.
    /// </summary>
    /// <param name="target">The target object.</param>
    public static void SetAttributeValues (System.Object target) {
      var targetType = target.GetType ();
      var fields = targetType.GetFields (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      foreach (var field in fields) {
        var assetFieldAttribute = System.Attribute.GetCustomAttribute (field, typeof (Asset)) as Asset;
        if (assetFieldAttribute == null) {
          continue;
        }
        var assetName = assetFieldAttribute.useFieldNameAsAssetName ? field.Name : assetFieldAttribute.assetName;
        var asset = Controller.Instance.GetAsset (assetName);
        field.SetValue (target, asset);
      }
    }
  }
}