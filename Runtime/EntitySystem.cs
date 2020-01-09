namespace UnityPackages.EntityComponentSystem {

  /// An entity system.
  public abstract class EntitySystem<EntitySystemType, EntityComponentType> : IEntitySystem
  where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new ()
  where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new () {

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnUpdate () { }
    public virtual void OnDrawGizmos () { }
    public virtual void OnDrawGui () { }
    public virtual void OnEnabled () { }
    public virtual void OnDisabled () { }
    public virtual void OnEntityInitialize (EntityComponentType entity) { }
    public virtual void OnEntityInitialized (EntityComponentType entity) { }
    public virtual void OnEntityEnabled (EntityComponentType entity) { }
    public virtual void OnEntityDisabled (EntityComponentType entity) { }
    public virtual void OnEntityWillDestroy (EntityComponentType entity) { }
    public virtual bool ShouldUpdate () { return true; }

    /// An instance reference to the controller.
    public static EntitySystemType Instance;

    /// Defines whether this system is enabled.
    private bool isEnabled = false;

    /// Defines whether this component has been initialized.
    private bool isInitialized = false;

    /// A list of the system's instantiated entity components.
    public System.Collections.Generic.List<EntityComponentType> entities = new System.Collections.Generic.List<EntityComponentType> ();

    /// The first instantiated entity compoent if this system.
    public EntityComponentType entity = null;

    /// Defines the number of instantiated entity components this system has.
    public int entityCount = 0;

    /// Defines whether the system has instantiated entity components.
    public bool hasEntities = false;

    /// Gets a entity's component on a given system.
    public void GetComponentOnEntity<C2> (EntityComponentType entity, System.Action<C2> action) {
      var _entity = entity.GetComponent<C2> ();
      if (_entity != null)
        action (_entity);
    }

    /// Gets a entity's component on a given system.
    public C2 GetComponentOnEntity<C2> (EntityComponentType entity) =>
      entity.GetComponent<C2> ();

    /// Checks whether a given entity has a component.
    public bool HasComponentOnEntity<C2> (EntityComponentType entity) =>
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
      Instance = Controller.Instance.GetSystem<EntitySystemType> ();

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
    public void Internal_AddEntity (EntityComponentType component) {
      if (this.hasEntities == false)
        this.entity = component;
      this.entityCount++;
      this.hasEntities = true;
      this.entities.Add (component);
      this.OnEntityInitialize (component);
    }

    /// Internal method to remove an entity's component from this system.
    public void Internal_RemoveEntry (EntityComponentType component) {
      this.entityCount--;
      this.hasEntities = this.entityCount > 0;
      this.OnEntityWillDestroy (component);
      this.entities.Remove (component);
      this.entity = this.hasEntities == false ? null : this.entities[0];
    }
  }
}