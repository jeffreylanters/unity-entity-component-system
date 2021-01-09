namespace ElRaccoone.EntityComponentSystem {

  /// Describes an asset which should be assigned automatically.
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Asset : System.Attribute {

    /// Defines whether the field name should be used for getting the asset from
    /// the controller. When set to false, the asset name string will be used
    /// instead.
    private bool useFieldNameAsAssetName = false;

    /// The asset name will be used when defined that the field name should not
    /// be used for finding the asset name. Thus using this asset name property
    /// instead.
    private string assetName = "";

    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// name of the property field.
    public Asset () {
      this.useFieldNameAsAssetName = true;
      this.assetName = "";
    }

    /// Defines the property as an asset, when the controller is done 
    /// registering, the asset will be fetched from the controller using the
    /// given asset name.
    public Asset(string assetName) {
      this.useFieldNameAsAssetName = false;
      this.assetName = assetName;
    }

    /// Sets the attributes values on an object.
    public static void SetAttributeValues (System.Object target) {
      var _fields = target.GetType ().GetFields (
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.NonPublic);
      foreach (var _field in _fields) {
        var _assetFieldAttribute = System.Attribute.GetCustomAttribute (_field, typeof (Asset)) as Asset;
        if (_assetFieldAttribute != null)
          _field.SetValue (target, Controller.Instance.GetAsset(
            _assetFieldAttribute.useFieldNameAsAssetName == true ? _field.Name : _assetFieldAttribute.assetName));
      }
    }
  }
}