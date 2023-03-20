namespace ElRaccoone.EntityComponentSystem {

  /// Base class for every entity system.
  public abstract class EntitySystem<EntitySystemType, EntityComponentType> : IEntitySystem, IEntitySystemInternals
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new()
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new() {

    /// Defines whether this system has been initialized.
    private bool isInitialized = false;

    /// An instance reference to the controller.
    public static EntitySystemType Instance { private set; get; } = null;

    /// A list of the system's instantiated entity components.
    public System.Collections.Generic.List<EntityComponentType> entities { private set; get; } = new System.Collections.Generic.List<EntityComponentType> ();

    /// The first instantiated entity compoent if this system has any.
    public EntityComponentType entity { private set; get; } = null;

    /// Defines the number of instantiated entity components this system has any.
    public int entityCount { private set; get; } = 0;

    /// Defines whether the system has instantiated entity components.
    public bool hasEntities { private set; get; } = false;

    /// Method invoked when the system will initialize.
    public virtual void OnInitialize () { }

    /// Method invoked when the system is initialized.
    public virtual void OnInitialized () { }

#if ECS_PHYSICS
    /// Method invoked when the physics update, will be called every fixed frame.
    public virtual void OnPhysics () { }
#endif

    /// Method invoked when the system updates, will be called every frame.
    public virtual void OnUpdate () { }

#if ECS_GRAPHICS
    /// Method invoked when the camera renders, will be called every late frame.
    public virtual void OnRender () { }
#endif

    // Method invoked when the system is drawing the gizmos, will be called
    // every gizmos draw call.
    public virtual void OnDrawGizmos () { }

    // Method invoked when the system is drawing the gui, will be called every
    // on gui draw call.
    public virtual void OnDrawGui () { }

    // Method invoked when the system becomes enabled.
    public virtual void OnEnabled () { }

    // Method invoked when the system becomes disabled.
    public virtual void OnDisabled () { }

    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    public virtual void OnWillDestroy () { }

    // Method invoked when an entity of this system is initializing.
    public virtual void OnEntityInitialize (EntityComponentType entity) { }

    // Method invoked when an entity of this system is initialized.
    public virtual void OnEntityInitialized (EntityComponentType entity) { }

    // Method invoked when an entity of this system becomes enabled.
    public virtual void OnEntityEnabled (EntityComponentType entity) { }

    // Method invoked when an entity of this system becomes disabled.
    public virtual void OnEntityDisabled (EntityComponentType entity) { }

    // Method invoked when an entity of this system will destroy.
    public virtual void OnEntityWillDestroy (EntityComponentType entity) { }

    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    public virtual bool ShouldUpdate () { return true; }

    /// Returns another component on an entity.
    public void GetComponentOnEntity<C> (EntityComponentType entity, System.Action<C> then) {
      var _entity = entity.GetComponent<C> ();
      if (_entity != null)
        then (_entity);
    }

    /// Returns another component on an entity.
    public C GetComponentOnEntity<C> (EntityComponentType entity) =>
      entity.GetComponent<C> ();

    /// Checks whether an entity has a specific component.
    public bool HasComponentOnEntity<C> (EntityComponentType entity) =>
      entity.GetComponent<C> () != null;

    /// Creates a new entity.
    public EntityComponentType CreateEntity () =>
      new UnityEngine.GameObject ("Entity " + typeof (EntityComponentType).Name)
        .AddComponent<EntityComponentType> ();

    /// Clones an entity.
    public EntityComponentType CloneEntity (EntityComponentType entity) =>
      UnityEngine.Object.Instantiate (entity).GetComponent<EntityComponentType> ();

    /// Clones an entity on a given position in the hierarchy.
    public EntityComponentType CloneEntity (EntityComponentType entity, UnityEngine.Transform parentTransform) =>
      UnityEngine.Object.Instantiate (entity, parentTransform).GetComponent<EntityComponentType> ();

    /// Finds entities using a predicate match.
    public EntityComponentType[] MatchEntities (System.Predicate<EntityComponentType> match) =>
      this.entities.FindAll (match).ToArray ();

    /// Finds an entity using a predicate match.
    public EntityComponentType MatchEntity (System.Predicate<EntityComponentType> match) =>
      this.entities.Find (match);

    /// Starts a coroutine on this system.
    public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StartCoroutine (routine);

    /// Stops a given coroutine.
    public void StopCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StopCoroutine (routine);

    /// Stops a given coroutine.
    public void StopCoroutine (UnityEngine.Coroutine routine) =>
      Controller.Instance.StopCoroutine (routine);

    /// Sets whether the system is enabled or disabled, enabling the system allows
    /// it to invoke all of the cycle calls such as OnUpdate and OnDrawGizmos.
    public void SetEnabled (bool value) =>
      Controller.Instance.SetSystemEnabled<EntitySystemType> (value);

    /// Internal method to add an entity's component to this system.
    internal void AddEntity (EntityComponentType component) {
      if (this.hasEntities == false)
        this.entity = component;
      this.entityCount++;
      this.hasEntities = true;
      this.entities.Add (component);
      this.OnEntityInitialize (component);
    }

    /// Internal method to remove an entity's component from this system.
    internal void RemoveEntry (EntityComponentType component) {
      this.entityCount--;
      this.hasEntities = this.entityCount > 0;
      this.OnEntityWillDestroy (component);
      this.entities.Remove (component);
      this.entity = this.hasEntities == false ? null : this.entities[0];
    }

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void IEntitySystemInternals.OnInitializeInternal () =>
      Instance = Controller.Instance.GetSystem<EntitySystemType> ();

    /// Internal method to update the children of the system.
    void IEntitySystemInternals.OnUpdateInternal () {
      if (this.isInitialized == false) {
        this.OnInitialized ();
        if (Controller.Instance.IsSystemEnabled<EntitySystemType> () == true)
          this.OnEnabled ();
        this.isInitialized = true;
      }
      for (var _entityIndex = 0; _entityIndex < this.entityCount; _entityIndex++)
        (this.entities[_entityIndex] as IEntityComponentInternals).OnUpdateInternal ();
    }
  }
}
