namespace UnityPackages.EntityComponentSystem {

  /// An entity system.
  public abstract class EntitySystem<S, C> : IEntitySystem where S : EntitySystem<S, C>, new () where C : EntityComponent<C, S>, new () {
    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }
    public virtual void OnDrawGizmos () { }
    public virtual void OnDrawGui () { }
    public virtual void OnEnabled () { }
    public virtual void OnDisabled () { }
    public virtual void OnEntityInitialize (C entity) { }
    public virtual void OnEntityInitialized (C entity) { }
    public virtual void OnEntityEnabled (C entity) { }
    public virtual void OnEntityDisabled (C entity) { }
    public virtual void OnEntityWillDestroy (C entity) { }

    /// An instance reference to the controller.
    public static S Instance;

    /// Defines whether this system is enabled.
    private bool isEnabled = false;

    /// Defines whether this component has been initialized.
    private bool isInitialized = false;

    /// A list of the system's instantiated entity components.
    public System.Collections.Generic.List<C> entities = new System.Collections.Generic.List<C> ();

    /// The first instantiated entity compoent if this system.
    public C entity { get { return this.entities[0]; } }

    /// Defines the number of instantiated entity components this system has.
    public int entityCount = 0;

    /// Defines whether the system has instantiated entity components.
    public bool hasEntities = false;

    /// Gets a entity's component on a given system.
    public void GetComponentOnEntity<C2> (C entity, System.Action<C2> action) {
      var _entity = entity.GetComponent<C2> ();
      if (_entity != null)
        action (_entity);
    }

    /// Gets a entity's component on a given system.
    public C2 GetComponentOnEntity<C2> (C entity) =>
      entity.GetComponent<C2> ();

    /// Checks whether a given entity has a component.
    public bool HasComponentOnEntity<C2> (C entity) =>
      entity.GetComponent<C2> () != null;

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
    public void Internal_OnInitialize () =>
      Instance = Controller.Instance.GetSystem<S> ();

    /// Internal method to update the children of the system.
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.OnInitialized ();
        if (this.isEnabled == true)
          this.OnEnabled ();
        this.isInitialized = true;
      }
      for (var _entityIndex = 0; _entityIndex < this.entityCount; _entityIndex++)
        this.entities[_entityIndex].Internal_OnUpdate ();
    }

    /// Internal method to add an entity's component to this system.
    public void Internal_AddEntity (C component) {
      this.entityCount++;
      this.hasEntities = true;
      this.entities.Add (component);
      this.OnEntityInitialize (component);
    }

    /// Internal method to remove an entity's component from this system.
    public void Internal_RemoveEntry (C component) {
      this.entityCount--;
      this.hasEntities = this.entityCount > 0;
      this.OnEntityWillDestroy (component);
      this.entities.Remove (component);
    }
  }
}