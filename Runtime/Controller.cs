namespace UnityPackages.EntityComponentSystem {
  public abstract class Controller : UnityEngine.MonoBehaviour {
    private System.Collections.Generic.List<ISystem> systems;
    private bool isInitialized;

    public static Controller Instance;

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }

    public bool debugging;

    private void Awake () {
      Instance = this;
      UnityEngine.GameObject.DontDestroyOnLoad (this.gameObject);
      this.systems = new System.Collections.Generic.List<ISystem> ();
      this.OnInitialize ();
    }

    private void Update () {
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].isEnabled == true)
          this.systems[_i].OnUpdate ();
      if (this.isInitialized == false) {
        for (var _i = 0; _i < this.systems.Count; _i++) {
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
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].isEnabled == true)
          this.systems[_i].OnGUI ();
    }

    public void RegisterSystems (params System.Type[] typesOf) {
      if (this.isInitialized == false)
        for (var _i = 0; _i < typesOf.Length; _i++) {
          var _system = (ISystem) System.Activator.CreateInstance (typesOf[_i]);
          this.systems.Add (_system);
          _system.OnInitialize ();
          _system.OnInitializeInternal ();
          _system.isEnabled = true;
        }
      else
        throw new System.Exception ("Unable to registered system outsize of OnInitialize cycle");
    }

    public void EnableSystems (params System.Type[] typesOf) {
      for (var _t = 0; _t < typesOf.Length; _t++)
        for (var _i = 0; _i < this.systems.Count; _i++)
          if (this.systems[_i].GetType () == typesOf[_t]) {
            this.systems[_i].isEnabled = true;
            this.systems[_i].OnEnabled ();
          }
    }

    public void DisableSystems (params System.Type[] typesOf) {
      for (var _t = 0; _t < typesOf.Length; _t++)
        for (var _i = 0; _i < this.systems.Count; _i++)
          if (this.systems[_i].GetType () == typesOf[_t]) {
            this.systems[_i].isEnabled = false;
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

    public bool HasSystem<S> () where S : ISystem, new () {
      var _typeOfS = typeof (S);
      for (var _i = 0; _i < this.systems.Count; _i++)
        if (this.systems[_i].GetType () == _typeOfS)
          return true;
      return false;
    }
  }
}