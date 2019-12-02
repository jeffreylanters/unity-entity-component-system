namespace UnityPackages.EntityComponentSystem {

  public abstract class EntitySystem<S, C> : ISystem where S : EntitySystem<S, C>, new () where C : EntityComponent<C, S>, new () {

    /// An instance reference to the controller. This reference will be set during the
    ///  'InternalOnInitialize' method inside this class. This method will be called
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

    public System.Collections.Generic.List<C> entities = new System.Collections.Generic.List<C> ();
    public C firstEntity { get { return this.entities[0]; } }
    public int entityCount = 0;
    public bool hasEntities = false;

    /// Gets a component on a given system.
    public void GetComponentOnEntity<GEC> (C entity, System.Action<GEC> action) {
      var _entity = entity.GetComponent<GEC> ();
      if (_entity != null)
        action (_entity);
    }

    /// Gets a component on a given system.
    public GEC GetComponentOnEntity<GEC> (C entity) =>
      entity.GetComponent<GEC> ();

    /// Checks if a given entity has a component.
    public bool HasComponentOnEntity<GEC> (C entity) =>
      entity.GetComponent<GEC> () != null;

    /// Starts a coroutine on this system.
    public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StartCoroutine (routine);

    /// Stops a given coroutine.
    public void StopCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StopCoroutine (routine);

    /// Enables or disables this system.
    public void SetEnabled (bool isEnabled) =>
      Instance.isEnabled = isEnabled;

    /// Gets the enabled status of this system
    public bool GetEnabled () =>
      Instance.isEnabled;

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    public void InternalOnInitialize () =>
      Instance = Controller.Instance.GetSystem<S> ();

    /// Internal method to add an entity to this system.
    public void InternalAddEntity (C component) {
      this.entityCount++;
      this.hasEntities = true;
      this.entities.Add (component);
      this.OnEntityInitialize (component);
    }

    /// Internal method to remove an entity from this system.
    public void InternalRemoveEntry (C component) {
      this.entityCount--;
      this.hasEntities = this.entityCount > 0;
      this.OnEntityWillDestroy (component);
      this.entities.Remove (component);
    }
  }
}
