namespace ElRaccoone.EntityComponentSystem {

  /// Base class for every entity component.
  public abstract class EntityComponent<EntityComponentType, EntitySystemType> : UnityEngine.MonoBehaviour, IEntityComponent
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new()
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new() {

    /// Defines whether this component is enabled.
    private bool isEnabled = false;

    /// Defines whether this component has been initialized.
    private bool isInitialized = false;

    /// The system matched with this entity's component.
    private EntitySystemType system = null;

    /// Gets the system matched with this entity's component. If it's not
    /// defined, it will be fetched from the controller.
    private EntitySystemType GetSystem () {
      if (this.system == null)
        if (Controller.Instance.HasSystem<EntitySystemType> () == true)
          this.system = Controller.Instance.GetSystem<EntitySystemType> ();
        else throw new System.Exception ("Tried to access the system before it was registered");
      return this.system;
    }

    /// During the 'Start' the entity component will be registered 
    /// to the matching system.
    private void Start () =>
      this.GetSystem ().Internal_AddEntity ((EntityComponentType)this);

    /// During the 'OnDisabled' the entity component will invoke its
    /// 'OnEntityDisabled' on the system.
    private void OnDisable () {
      this.isEnabled = false;
      this.GetSystem ().OnEntityDisabled ((EntityComponentType)this);
    }

    /// During the 'OnDestroy' the entity component will unregister it self
    /// to the matching system.
    private void OnDestroy () =>
      this.GetSystem ().Internal_RemoveEntry ((EntityComponentType)this);

    /// Adds an asset to the entity.
    public UnityEngine.Object AddAsset (UnityEngine.Object asset) =>
      UnityEngine.Object.Instantiate (asset, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, this.transform);

    /// Loads a asset from the controller and adds it as an asset to the entity.
    public UnityEngine.Object AddAsset (string assetName) {
      var _assets = Controller.Instance.assets;
      for (var _i = 0; _i < _assets.Length; _i++)
        if (_assets[_i].name == assetName)
          return this.AddAsset (_assets[_i]);
      throw new System.Exception ("Unable to load asset, it was not added to the controller");
    }

    /// Sets the position of an entity.
    public void SetPosition (float x, float y, float z = 0) =>
      this.transform.position = new UnityEngine.Vector3 (x, y, z);

    /// Adds to the position of an entity.
    public void AddPosition (float x, float y, float z = 0) =>
      this.transform.position += new UnityEngine.Vector3 (x, y, z);

    /// Sets the local position of an entity.
    public void SetLocalPosition (float x, float y, float z = 0) =>
      this.transform.localPosition = new UnityEngine.Vector3 (x, y, z);

    /// Adds to the local position of an entity.
    public void AddLocalPosition (float x, float y, float z = 0) =>
      this.transform.localPosition += new UnityEngine.Vector3 (x, y, z);

    /// Sets the EulerAngles of an entity.
    public void SetEulerAngles (float x, float y, float z) =>
      this.transform.eulerAngles = new UnityEngine.Vector3 (x, y, z);

    /// Adds to the EulerAngles of an entity.
    public void AddEulerAngles (float x, float y, float z) =>
      this.transform.eulerAngles += new UnityEngine.Vector3 (x, y, z);

    /// Sets the local EulerAngles of an entity.
    public void SetLocalEulerAngles (float x, float y, float z) =>
      this.transform.localEulerAngles = new UnityEngine.Vector3 (x, y, z);

    /// Adds to the local EulerAngles of an entity.
    public void AddLocalEulerAngles (float x, float y, float z) =>
      this.transform.localEulerAngles += new UnityEngine.Vector3 (x, y, z);

    /// Sets the local scale of an entity.
    public void SetLocalScale (float x, float y, float z) =>
      this.transform.localScale = new UnityEngine.Vector3 (x, y, z);

    /// Adds to the local Scale of an entity.
    public void AddLocalScale (float x, float y, float z) =>
      this.transform.localScale += new UnityEngine.Vector3 (x, y, z);

    /// Sets the game object of the entity active.
    public void SetActive (bool value) =>
      this.gameObject.SetActive (value);

    /// During the 'InteralOnUpdate' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    [Internal]
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.isInitialized = true;
        this.GetSystem ().OnEntityInitialized ((EntityComponentType)this);
      }
      if (this.isEnabled == false && this.gameObject.activeInHierarchy == true) {
        this.isEnabled = true;
        this.GetSystem ().OnEntityEnabled ((EntityComponentType)this);
      }
    }
  }
}