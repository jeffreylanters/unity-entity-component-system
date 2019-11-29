namespace UnityPackages.EntityComponentSystem {
  public abstract class EntitySystem<S, C> : ISystem where S : EntitySystem<S, C>, new () where C : EntityComponent<C, S>, new () {

    /// An instance reference to the controller. This reference will be set during the
    ///  'OnInitializeInternal' method inside this class. This method will be called
    ///  after the controller and system initialization.
    public static S Instance;

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }
    public virtual void OnDrawGizmos () { }
    public virtual void OnGUI () { }
    public virtual void OnEnabled () { }
    public virtual void OnDisabled () { }
    public virtual void OnEntityInitialize (C entity) { }
    public virtual void OnEntityInitialized (C entity) { }
    public virtual void OnEntityStart (C entity) { }
    public virtual void OnEntityEnabled (C entity) { }
    public virtual void OnEntityDisabled (C entity) { }
    public virtual void OnEntityWillDestroy (C entity) { }

    private bool isEnabled;

    public System.Collections.Generic.List<C> entities;
    public C firstEntity { get { return this.entities[0]; } }

    public EntitySystem () =>
      this.entities = new System.Collections.Generic.List<C> ();

    public void AddEntity (C component) {
      this.entities.Add (component);
      this.OnEntityInitialize (component);
    }

    public void RemoveEntry (C component) {
      this.OnEntityWillDestroy (component);
      this.entities.Remove (component);
    }

    public void GetComponentOnEntity<GEC> (C entity, System.Action<GEC> action) {
      var _entity = entity.GetComponent<GEC> ();
      if (_entity != null)
        action (_entity);
    }

    public bool HasComponentOnEntity<GEC> (C entity) =>
      entity.GetComponent<GEC> () != null;

    public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StartCoroutine (routine);

    public void StopCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StopCoroutine (routine);

    public void InternalOnInitialize () =>
      Instance = Controller.Instance.GetSystem<S> ();

    public void InternalSetEnabled (bool isEnabled) =>
      Instance.isEnabled = isEnabled;

    public bool InternalGetEnabled () =>
      Instance.isEnabled;
  }
}