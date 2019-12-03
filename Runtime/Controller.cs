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

    /// Defines whether this controller has been intialized.
    private bool isInitialized;

    /// During the awake, this system will start the initialization.
    private void Awake () {
      UnityEngine.GameObject.DontDestroyOnLoad (this.gameObject);
      Controller.Instance = this;
      this.systems = new System.Collections.Generic.List<IEntitySystem> ();
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

      // Invoking 'OnUpdate' on the controller, each enabled system
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        this.systems[_systemIndex].Internal_OnUpdate ();
      this.OnUpdate ();
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetEnabled () == true)
          this.systems[_systemIndex].OnUpdate ();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos () {
      if (UnityEngine.Application.isPlaying == true)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          this.systems[_systemIndex].OnDrawGizmos ();
    }
#endif

    private void OnGUI () {
      this.OnDrawGui ();
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetEnabled () == true)
          this.systems[_systemIndex].OnDrawGui ();
    }

    public void RegisterSystems (params System.Type[] typesOf) {
      if (this.isInitialized == true)
        throw new System.Exception ("Unable to registered system outsize of OnInitialize cycle");

      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++) {
        var _system = (IEntitySystem) System.Activator.CreateInstance (typesOf[_typeOfIndex]);
        this.systems.Add (_system);
        _system.OnInitialize ();
        _system.Internal_OnInitialize ();
        _system.SetEnabled (true);
      }

      // Set Values of the 'InjectedSystem' attributes
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        InjectedSystem.SetAttributeValues (this.systems[_systemIndex]);
    }

    public void EnableSystems (params System.Type[] typesOf) {
      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          if (this.systems[_systemIndex].GetType () == typesOf[_typeOfIndex]) {
            this.systems[_systemIndex].SetEnabled (true);
            this.systems[_systemIndex].OnEnabled ();
          }
    }

    public void DisableSystems (params System.Type[] typesOf) {
      for (var _typeOfIndex = 0; _typeOfIndex < typesOf.Length; _typeOfIndex++)
        for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
          if (this.systems[_systemIndex].GetType () == typesOf[_typeOfIndex]) {
            this.systems[_systemIndex].SetEnabled (false);
            this.systems[_systemIndex].OnDisabled ();
          }
    }

    public S GetSystem<S> () where S : IEntitySystem, new () {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return (S) this.systems[_systemIndex];
      return new S ();
    }

    public System.Object GetSystem (System.Type typeOf) {
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == typeOf)
          return this.systems[_systemIndex];
      return null;
    }

    public bool HasSystem<S> () where S : IEntitySystem, new () {
      var _typeOfS = typeof (S);
      for (var _systemIndex = 0; _systemIndex < this.systems.Count; _systemIndex++)
        if (this.systems[_systemIndex].GetType () == _typeOfS)
          return true;
      return false;
    }
  }
}