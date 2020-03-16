namespace UnityPackages.EntityComponentSystem {
  public abstract class Controller : UnityEngine.MonoBehaviour {
    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }
    public virtual void OnDrawGui () { }

    /// A reference to the controller.
    public static Controller Instance;

    /// A list of the controller's instantiated entity systems.
    private System.Collections.Generic.List<IEntitySystem> systems;

    /// A list of the controller's instantiated services.
    private System.Collections.Generic.List<IService> services;

    /// Defines whether this controller has been intialized.
    private bool isInitialized;

    /// During the awake, this system will start the initialization.
    private void Awake () {
      UnityEngine.GameObject.DontDestroyOnLoad (this.gameObject);
      Controller.Instance = this;
      this.systems = new System.Collections.Generic.List<IEntitySystem> ();
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

      // Invoking 'OnUpdate' on the controller, each enabled system that wants
      // to be updated using ShouldUpdate.
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        this.systems[_systemIndex].Internal_OnUpdate ();
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        this.services[_serviceIndex].Internal_OnUpdate ();
      this.OnUpdate ();
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++) {
        var _system = this.systems[_systemIndex];
        if (_system.GetEnabled () == true)
          if (_system.ShouldUpdate () == true)
            _system.OnUpdate ();
      }
    }

#if UNITY_EDITOR
    /// Invokes the OnDrawGizmos on the Systems
    private void OnDrawGizmos () {
      if (UnityEngine.Application.isPlaying == true) {
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          this.systems[_systemIndex].OnDrawGizmos ();
        for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
          this.services[_serviceIndex].OnDrawGizmos ();
      }
    }
#endif

    /// Invokes the OnDrawGui on the Systems
    private void OnGUI () {
      this.OnDrawGui ();
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetEnabled () == true)
          this.systems[_systemIndex].OnDrawGui ();
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        this.services[_serviceIndex].OnDrawGui ();
    }

    /// Register systems and services to this controller.
    public void Register (params System.Type[] typesOf) {
      if (this.isInitialized == true)
        throw new System.Exception ("Unable to registered system outsize of OnInitialize cycle");

      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++) {
        var _instance = System.Activator.CreateInstance (typesOf[_typeOfIndex]);

        // When the instance is a type of the system, add it to the systems
        if (_instance is IEntitySystem) {
          var _system = _instance as IEntitySystem;
          this.systems.Add (_system);
          _system.OnInitialize ();
          _system.Internal_OnInitialize ();
          _system.SetEnabled (true);
        }

        // When the instance is a type of the system, add it to the services
        if (_instance is IService) {
          var _service = _instance as IService;
          this.services.Add (_service);
          _service.OnInitialize ();
          _service.Internal_OnInitialize ();
        }
      }

      // Set Values of the 'InjectedSystem' attributes
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        Injected.SetAttributeValues (this.systems[_systemIndex]);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        Injected.SetAttributeValues (this.services[_serviceIndex]);
    }

    /// Enables systems.
    public void EnableSystems (params System.Type[] typesOf) {
      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          if (this.systems[_systemIndex].GetType () == typesOf[_typeOfIndex]) {
            this.systems[_systemIndex].SetEnabled (true);
            this.systems[_systemIndex].OnEnabled ();
          }
    }

    /// Disables systems.
    public void DisableSystems (params System.Type[] typesOf) {
      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          if (this.systems[_systemIndex].GetType () == typesOf[_typeOfIndex]) {
            this.systems[_systemIndex].SetEnabled (false);
            this.systems[_systemIndex].OnDisabled ();
          }
    }

    /// Gets a system from this controller.
    public S GetSystem<S> () where S : IEntitySystem, new () {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return (S) this.systems[_systemIndex];
      throw new System.Exception ("Unable to get system, it was not registerd");
    }

    /// Gets a system from this controller.
    public System.Object GetSystem (System.Type typeOf) {
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == typeOf)
          return this.systems[_systemIndex];
      throw new System.Exception ("Unable to get system, it was not registerd");
    }

    /// Check whether this controller has a system.
    public bool HasSystem<S> () where S : IEntitySystem, new () {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return true;
      return false;
    }

    /// Gets a service from this controller.
    public S GetService<S> () where S : IService, new () {
      var _typeOfS = typeof (S);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == _typeOfS)
          return (S) this.services[_serviceIndex];
      throw new System.Exception ("Unable to get service, it was not registerd");
    }

    /// Gets a system from this controller.
    public System.Object GetService (System.Type typeOf) {
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == typeOf)
          return this.services[_serviceIndex];
      throw new System.Exception ("Unable to get service, it was not registerd");
    }

    /// Check whether this controller has a service.
    public bool HasService<S> () where S : IService, new () {
      var _typeOfS = typeof (S);
      for (var _serviceIndex = 0; _serviceIndex < this.services.Count; _serviceIndex++)
        if (this.services[_serviceIndex].GetType () == _typeOfS)
          return true;
      return false;
    }
  }
}