namespace ElRaccoone.EntityComponentSystem {

  /// Base class for every controller.
  public abstract class Controller : UnityEngine.MonoBehaviour, IController {

    /// A list of the controller's instantiated entity systems.
    private System.Collections.Generic.List<IEntitySystem> systems;

    /// A list of the controller's instantiated entity systems which are enabled.
    private System.Collections.Generic.List<IEntitySystem> enabledSystemsCache;

    /// A list of the controller's instantiated services.
    private System.Collections.Generic.List<IService> services;

    /// Defines whether this controller has been intialized.
    private bool isInitialized;

    /// A reference to the controller.
    public static Controller Instance { private set; get; } = null;

    /// The assets that can be added to entities.
    public UnityEngine.Object[] assets;

    /// During the awake, this system will start the initialization.
    private void Awake () {
      if (Controller.Instance != null)
        throw new System.Exception ("A project cannot exceed the limit of one controller!");
      UnityEngine.Object.DontDestroyOnLoad (this.transform.root.gameObject);
      Controller.Instance = this;
      this.systems = new System.Collections.Generic.List<IEntitySystem> ();
      this.enabledSystemsCache = new System.Collections.Generic.List<IEntitySystem> ();
      this.services = new System.Collections.Generic.List<IService> ();
      this.OnInitialize ();
    }

    /// During the Update
    private void Update () {
      // while the controller is not initialized it will invoke 'OnInitialized'
      // on itelf. And then 'OnEnabled' and 'OnInitialized' on the systems.
      if (this.isInitialized == false) {
        this.OnInitialized ();
        this.isInitialized = true;
      }

      // Invoking 'Internal_OnUpdate' on each system.
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        this.systems[_systemIndex].Internal_OnUpdate ();
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        this.services[_serviceIndex].Internal_OnUpdate ();
      // Invoking 'OnUpdate' on the controller.
      this.OnUpdate ();
      // Invoking 'OnUpdate' on each enabled system that Should to be updated.
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++) {
        var _system = this.enabledSystemsCache[_systemIndex];
        if (_system.ShouldUpdate () == true)
          _system.OnUpdate ();
      }
    }

#if ECS_PHYSICS || ECS_ALL
    /// During the FixedUpdate, 'OnPhysics' will be invoked on each enabled system.
    private void FixedUpdate () {
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++)
        this.enabledSystemsCache[_systemIndex].OnPhysics ();
    }
#endif

#if ECS_GRAPHICS || ECS_ALL
    /// During the LateUpdate, 'OnRender' will be invoked on each enabled system.
    private void LateUpdate () {
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++)
        this.enabledSystemsCache[_systemIndex].OnRender ();
    }
#endif

    /// When the controller is destoryed, it will invoke 'OnWillDestroy' on the
    /// controller and on each of the systems and services.
    private void OnDestroy () {
      this.OnWillDestroy ();
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        this.systems[_systemIndex].OnWillDestroy ();
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        this.services[_serviceIndex].OnWillDestroy ();
    }

#if UNITY_EDITOR
    /// During the OnDrawGizmos, 'OnDrawGizmos' will be invoked on each enabled
    /// system and all services.
    private void OnDrawGizmos () {
      if (UnityEngine.Application.isPlaying == true) {
        for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++) 
          this.enabledSystemsCache[_systemIndex].OnDrawGizmos ();
        for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
          this.services[_serviceIndex].OnDrawGizmos ();
      }
    }
#endif

    /// During the OnGUI, 'OnDrawGui' will be invoked on each enabled system and
    /// all services.
    private void OnGUI () {
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++)
        this.enabledSystemsCache[_systemIndex].OnDrawGui ();
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        this.services[_serviceIndex].OnDrawGui ();
    }

    /// Method invoked when the controller is initializing.
    public virtual void OnInitialize () { }

    /// Method invoked when the controller is initialized.
    public virtual void OnInitialized () { }

    /// Method invoked when the controller updates, will be called every frame.
    public virtual void OnUpdate () { }

    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    public virtual void OnWillDestroy () { }

    // Register your systems and services to the controller. This can only be
    // done during 'OnInitialize' cycle.
    public void Register (params System.Type[] typesOf) {
      if (this.isInitialized == true)
        throw new System.Exception ("Cannot to registered System outsize of OnInitialize cycle");

      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++) {
        var _instance = System.Activator.CreateInstance (typesOf[_typeOfIndex]);

        // When the instance is a type of the system, add it to the systems
        if (_instance is IEntitySystem) {
          var _system = _instance as IEntitySystem;
          this.systems.Add (_system);
          this.enabledSystemsCache.Add (_system);
          _system.OnInitialize ();
          _system.Internal_OnInitialize ();
        }

        // When the instance is a type of the system, add it to the services
        if (_instance is IService) {
          var _service = _instance as IService;
          this.services.Add (_service);
          _service.OnInitialize ();
          _service.Internal_OnInitialize ();
        }
      }

      // Set Values of the 'Injected' attributes
      Injected.SetAttributeValues (this);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        Injected.SetAttributeValues (this.systems[_systemIndex]);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        Injected.SetAttributeValues (this.services[_serviceIndex]);

      // Set Values of the 'Asset' attributes
      Asset.SetAttributeValues (this);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        Asset.SetAttributeValues (this.systems[_systemIndex]);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        Asset.SetAttributeValues (this.services[_serviceIndex]);
    }

    /// Enables or disabled a system, enabling the systems allows them to invoke
    /// their cycle methods such as OnUpdate, OnPhysics, OnDrawGui and others.
    public void SetSystemEnabled<S> (bool value) {
      var _typeOf = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++)
        if (this.enabledSystemsCache[_systemIndex].GetType () == _typeOf)
          if (value == true)
            return;
          else {
            this.enabledSystemsCache[_systemIndex].OnDisabled ();
            this.enabledSystemsCache.RemoveAt (_systemIndex);
            return;
          }
      if (value == true)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          if (this.systems[_systemIndex].GetType () == _typeOf) {
            this.enabledSystemsCache.Add (this.systems[_systemIndex]);
            this.systems[_systemIndex].OnEnabled ();
            return;
          }
    }

    /// Returns whether a system is enabled.
    public bool IsSystemEnabled<S> () {
      var _typeOf = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.enabledSystemsCache.Count; _systemIndex++)
        if (this.enabledSystemsCache[_systemIndex].GetType () == _typeOf)
          return true;
      return false;
    }

    /// Gets a system from this controller.
    public S GetSystem<S> () where S : IEntitySystem, new() {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return (S)this.systems[_systemIndex];
      throw new System.Exception ($"Unable to get System of type {_typeOfS}, it was not registerd to the Controller");
    }

    /// Gets a system from this controller.
    public System.Object GetSystem (System.Type typeOf) {
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == typeOf)
          return this.systems[_systemIndex];
      throw new System.Exception ($"Unable to get System of type {typeOf}, it was not registerd to the Controller");
    }

    /// Check whether this controller has a system.
    public bool HasSystem<S> () where S : IEntitySystem, new() {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return true;
      return false;
    }

    /// Gets a service from this controller.
    public S GetService<S> () where S : IService, new() {
      var _typeOfS = typeof (S);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == _typeOfS)
          return (S)this.services[_serviceIndex];
      throw new System.Exception ($"Unable to get Service of type {_typeOfS}, it was not registerd to the Controller");
    }

    /// Gets a system from this controller.
    public System.Object GetService (System.Type typeOf) {
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == typeOf)
          return this.services[_serviceIndex];
      throw new System.Exception ($"Unable to get Service of type {typeOf}, it was not registerd to the Controller");
    }

    /// Check whether this controller has a service.
    public bool HasService<S> () where S : IService, new() {
      var _typeOfS = typeof (S);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == _typeOfS)
          return true;
      return false;
    }

    /// Gets an asset from this controller.
    public UnityEngine.Object GetAsset (string name) {
      for (var _i = 0; _i < this.assets.Length; _i++)
        if (this.assets[_i].name == name)
          return this.assets[_i];
      throw new System.Exception ($"Unable to get Asset '{name}', it was not found on the Controller");
    }

    /// Gets an asset from this controller.
    public AssetType GetAsset<AssetType> (string name) where AssetType : UnityEngine.Object {
      return this.GetAsset (name) as AssetType;
    }

    /// Check whether this controller has an asset.
    public bool HasAsset (string name) {
      for (var _i = 0; _i < this.assets.Length; _i++)
        if (this.assets[_i].name == name)
          return true;
      return false;
    }
  }
}
