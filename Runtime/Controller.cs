namespace UnityPackages.EntityComponentSystem {
  public abstract class Controller : UnityEngine.MonoBehaviour {
    private System.Collections.Generic.List<ISystem> systems;
    private bool isInitialized;

    public static Controller Instance;

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }
    public virtual void OnDrawGui () { }

    private void Awake () {
      Instance = this;
      UnityEngine.GameObject.DontDestroyOnLoad (this.gameObject);
      this.systems = new System.Collections.Generic.List<ISystem> ();
      this.OnInitialize ();
    }

    /// The update is invoked every frame
    private void Update () {

      /// Invoking 'OnUpdate' on each enabled system
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetEnabled () == true)
          this.systems[_i].OnUpdate ();

      /// While the controller is not initialized, Invoke
      /// 'OnEnabled' and 'OnInitialized' on the systems.
      if (this.isInitialized == false) {
        for (var _i = 0; _i < this.systems.Count; _i++) {
          InjectedSystem.SetAttributeValues (this.systems[_i]);
          this.systems[_i].OnEnabled ();
          this.systems[_i].OnInitialized ();
        }

        this.OnInitialized ();
        this.isInitialized = true;
      }
      this.OnUpdate ();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos () {
      if (UnityEngine.Application.isPlaying == true)
        for (var _i = 0; _i < this.systems.Count; _i++)
          this.systems[_i].OnDrawGizmos ();
    }
#endif

    private void OnGUI () {
      this.OnDrawGui ();
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetEnabled () == true)
          this.systems[_i].OnDrawGui ();
    }

    public void RegisterSystems (params System.Type[] typesOf) {
      if (this.isInitialized == false)
        for (var _i = 0; _i < typesOf.Length; _i++) {
          var _system = (ISystem) System.Activator.CreateInstance (typesOf[_i]);
          this.systems.Add (_system);
          _system.OnInitialize ();
          _system.InternalOnInitialize ();
          _system.SetEnabled (true);
        }
      else
        throw new System.Exception ("Unable to registered system outsize of OnInitialize cycle");
    }

    public void EnableSystems (params System.Type[] typesOf) {
      for (var _t = 0; _t < typesOf.Length; _t++)
        for (var _i = 0; _i < this.systems.Count; _i++)
          if (this.systems[_i].GetType () == typesOf[_t]) {
            this.systems[_i].SetEnabled (true);
            this.systems[_i].OnEnabled ();
          }
    }

    public void DisableSystems (params System.Type[] typesOf) {
      for (var _t = 0; _t < typesOf.Length; _t++)
        for (var _i = 0; _i < this.systems.Count; _i++)
          if (this.systems[_i].GetType () == typesOf[_t]) {
            this.systems[_i].SetEnabled (false);
            this.systems[_i].OnDisabled ();
          }
    }

    public S GetSystem<S> () where S : ISystem, new () {
      var _typeOfS = typeof (S);
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetType () == _typeOfS)
          return (S) this.systems[_i];
      return new S ();
    }

    public System.Object GetSystem (System.Type typeOf) {
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetType () == typeOf)
          return this.systems[_i];
      return null;
    }

    public bool HasSystem<S> () where S : ISystem, new () {
      var _typeOfS = typeof (S);
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetType () == _typeOfS)
          return true;
      return false;
    }
  }
}