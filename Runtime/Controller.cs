namespace ElRaccoone.EntityComponentSystem {

  /// <summary>
  /// Base class for every controller.
  /// </summary>
  public abstract class Controller : UnityEngine.MonoBehaviour {

    /// <summary>
    /// Defines whether this controller has been intialized.
    /// </summary>
    private bool isInitialized;

    /// <summary>
    /// A list of the controller's instantiated entity systems.
    /// </summary>
    private System.Collections.Generic.List<EntitySystem> systems;

    /// <summary>
    /// A list of the controller's instantiated entity systems which are enabled.
    /// </summary>
    private System.Collections.Generic.List<EntitySystem> enabledSystemsCache;

    /// <summary>
    /// A list of the controller's instantiated services.
    /// </summary>
    private System.Collections.Generic.List<Service> services;

    /// <summary>
    /// A reference to the controller.
    /// TODO -- Set its protection level to internal?
    /// </summary>
    internal static Controller Instance { private set; get; } = null;

    /// <summary>
    /// The assets that can be added to entities.
    /// TODO -- Set this as readonly?
    /// </summary>
    public UnityEngine.Object[] assets;

    /// <summary>
    /// Event invoked by the Controller is awoken. This will
    /// initialize the Controller.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void Awake () {
      // If the Controller is already initialized, throw an exception.
      if (Controller.Instance != null)
        throw new System.Exception ("A project cannot exceed the limit of one controller!");
      // Make sure the Controller wont get destroyed.
      UnityEngine.Object.DontDestroyOnLoad (this.transform.root.gameObject);
      // Set the Controller's instance.
      Controller.Instance = this;
      // Initialize the properties.
      this.systems = new System.Collections.Generic.List<EntitySystem> ();
      this.enabledSystemsCache = new System.Collections.Generic.List<EntitySystem> ();
      this.services = new System.Collections.Generic.List<Service> ();
      // Initialize the controller.
      this.OnInitialize ();
    }

    /// <summary>
    /// Event invoked by the MonoBehaviour when itupdates.
    /// </summary>
    private void Update () {
      // If the controller is not initialized, it will invoke 'OnInitialized'
      // on itelf.
      if (this.isInitialized == false) {
        this.OnInitialized ();
        this.isInitialized = true;
      }
      // Invoking an Internal Update on each System and Service.
      for (var systemIndex = 0; systemIndex < this.systems.Count; systemIndex++) {
        this.systems[systemIndex].InternalUpdate ();
      }
      for (var serviceIndex = 0; serviceIndex < this.services.Count; serviceIndex++) {
        this.services[serviceIndex].InternalUpdate ();
      }
      // Invoking OnUpdate on this Controller.
      this.OnUpdate ();
      // Invoking OnUpdate on each enabled System that should update.
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
        var system = this.enabledSystemsCache[systemIndex];
        // Invoke the Should Update method to determine if the system should
        // be updated.
        if (system.ShouldUpdate () == true) {
          system.OnUpdate ();
        }
      }
    }

#if ECS_PHYSICS || ECS_ALL
    /// <summary>
    /// Event invoked by the MonoBehaviour when it updates fixed.
    /// </summary>
    private void FixedUpdate () {
      // If the ECS_PHYSICS compiler flag is enabled, then we'll invoke the
      // OnFixedUpdate as the OnPhysics method on each System and Service.
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
        this.enabledSystemsCache[systemIndex].OnPhysics ();
      }
    }
#endif

#if ECS_GRAPHICS || ECS_ALL
    /// <summary>
    /// Event invoked by the MonoBehaviour when it updates late.
    /// </summary>
    private void LateUpdate () {
      // If the ECS_GRAPHICS compiler flag is enabled, then we'll invoke the
      // LateUpdate as the OnRender method on each System and Service.
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
        this.enabledSystemsCache[systemIndex].OnRender ();
      }
    }
#endif

    /// <summary>
    /// Event invoked by the MonoBehaviour when it is destroyed.
    /// </summary>
    private void OnDestroy () {
      // Invoking OnWillDestroy evnet on this Controller and each System and
      // Service it contains.
      this.OnWillDestroy ();
      for (var systemIndex = 0; systemIndex < this.systems.Count; systemIndex++) {
        this.systems[systemIndex].OnWillDestroy ();
      }
      for (var serviceIndex = 0; serviceIndex < this.services.Count; serviceIndex++) {
        this.services[serviceIndex].OnWillDestroy ();
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Event invoked by the MonoBehaviour when the Gizmos are drawn.
    /// </summary>
    private void OnDrawGizmos () {
      // If the UNITY_EDITOR compiler flag is enabled, and the Application is
      // playing, then we'll invoke the OnDrawGizmos as the OnDrawGizmos method
      // on each enabled System and Service.
      if (UnityEngine.Application.isPlaying == true) {
        for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
          this.enabledSystemsCache[systemIndex].OnDrawGizmos ();
        }
        for (var serviceIndex = 0; serviceIndex < this.services.Count; serviceIndex++) {
          this.services[serviceIndex].OnDrawGizmos ();
        }
      }
    }
#endif

    /// <summary>
    /// Event invoked by the MonoBehaviour when the GUI is drawn.
    /// </summary>
    private void OnGUI () {
      // Invoking OnGUI on each enabled System and Service in this Controller.
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
        this.enabledSystemsCache[systemIndex].OnDrawGui ();
      }
      for (var serviceIndex = 0; serviceIndex < this.services.Count; serviceIndex++) {
        this.services[serviceIndex].OnDrawGui ();
      }
    }

    /// <summary>
    /// Method invoked when the controller is initializing.
    /// </summary>
    public virtual void OnInitialize () { }

    /// <summary>
    /// Method invoked when the controller is initialized.
    /// </summary>
    public virtual void OnInitialized () { }

    /// <summary>
    /// Method invoked when the controller updates, will be called every frame.
    /// </summary>
    public virtual void OnUpdate () { }

    /// <summary>
    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    public virtual void OnWillDestroy () { }

    /// <summary>
    /// Registers Systems and Services to the controller.
    /// </summary>
    /// <param name="registerables">Systems and Services.</param>
    /// <exception cref="System.Exception">Exception.</exception>
    public void Register (params IRegisterable[] registerables) {
      if (this.isInitialized == true)
        throw new System.Exception ("Registration can only performed during the OnInitialize cycle");
      // Register each registerable.
      for (var registerableIndex = 0; registerableIndex < registerables.Length; registerableIndex++) {
        var registerable = registerables[registerableIndex];
        // When the registerable is a type of the System.
        if (registerable is EntitySystem) {
          var system = registerable as EntitySystem;
          // The registerable will be added to the list of systems.
          this.systems.Add (system);
          this.enabledSystemsCache.Add (system);
          // The registerable will be initialized.
          system.OnInitialize ();
          system.InternalInitialize ();
        }
        // When the registerable is a type of the Service.
        if (registerable is Service) {
          var service = registerable as Service;
          // The registerable will be added to the list of services.
          this.services.Add (service);
          // The registerable will be initialized.
          service.OnInitialize ();
          service.InternalInitialize ();
        }
      }
      // Setting the values of properties with a Injected and Asset attribute 
      // in this controller and all of its systems and services.
      Injected.SetAttributeValues (this);
      Asset.SetAttributeValues (this);
      for (var systemIndex = 0; systemIndex < this.systems.Count; systemIndex++) {
        Injected.SetAttributeValues (this.systems[systemIndex]);
        Asset.SetAttributeValues (this.systems[systemIndex]);
      }
      for (var serviceIndex = 0; serviceIndex < this.services.Count; serviceIndex++) {
        Injected.SetAttributeValues (this.services[serviceIndex]);
        Asset.SetAttributeValues (this.services[serviceIndex]);
      }
    }

    /// <summary>
    /// Enables or disabled a system, enabling the systems allows them to invoke
    /// their cycle methods such as OnUpdate, OnPhysics, OnDrawGui and others.
    /// </summary>
    /// <typeparam name="EntitySystemType">The Type of the Entity System.</typeparam>
    /// <param name="enabled">Defines whether the system should be enabled.</param>
    internal void SetSystemEnabled<EntitySystemType> (bool enabled)
      where EntitySystemType : EntitySystem {
      var typeOf = typeof (EntitySystemType);
      // Looping through all the enabled Entity Systems.
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++) {
        // If the system is of the type generic system.
        if (this.enabledSystemsCache[systemIndex].GetType () == typeOf) {
          if (enabled == false) {
            // If it should be disabled, then we'll remove it from the list of
            // enabled systems, and invoke the On Disabled event on it. Then
            this.enabledSystemsCache[systemIndex].OnDisabled ();
            this.enabledSystemsCache.RemoveAt (systemIndex);
            return;
          }
          // If it should be enabled, then there is nothing we have to do since
          // it it already in this list. then we'll stop the execution of this 
          // method.
          return;
        }
      }
      // If we'll end up here, it was not found in the list of enabled systems,
      // if it should be enabled, we'll find the system in the list of systems.
      if (enabled == true) {
        for (var systemIndex = 0; systemIndex < this.systems.Count; systemIndex++) {
          if (this.systems[systemIndex].GetType () == typeOf) {
            // Then we'll add this system to the list of enabled systems, and
            // invoke the On Enabled event on it.
            this.enabledSystemsCache.Add (this.systems[systemIndex]);
            this.systems[systemIndex].OnEnabled ();
            // Then we'll stop the execution of this method.
            return;
          }
        }
      }
    }

    /// <summary>
    /// Returns whether a system is enabled.
    /// </summary>
    /// <typeparam name="EntitySystemType">The Type of the Entity System.</typeparam>
    /// <returns>Whether a system is enabled.</returns>
    internal bool IsSystemEnabled<EntitySystemType> ()
      where EntitySystemType : EntitySystem {
      var typeOf = typeof (EntitySystemType);
      for (var systemIndex = 0; systemIndex < this.enabledSystemsCache.Count; systemIndex++)
        if (this.enabledSystemsCache[systemIndex].GetType () == typeOf)
          return true;
      return false;
    }

    /// Gets a system from this controller.
    public S GetSystem<S> () where S : EntitySystem, new() {
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
    public bool HasSystem<S> () where S : EntitySystem, new() {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return true;
      return false;
    }

    /// Gets a service from this controller.
    public S GetService<S> () where S : Service, new() {
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
    public bool HasService<S> () where S : Service, new() {
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
