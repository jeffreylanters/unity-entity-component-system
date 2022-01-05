namespace ElRaccoone.EntityComponentSystem {

  /// <summary>
  /// Base class for every entity system.
  /// </summary>
  public abstract class EntitySystem : IRegisterable {

    /// <summary>
    /// Overrideable method which will be invoked when the service updates 
    /// internally.
    /// </summary>
    internal abstract void InternalInitialize ();

    /// <summary>
    /// Overrideable method which will be invoked when the service updates 
    /// internally.
    /// </summary>
    internal abstract void InternalUpdate ();

    /// <summary>
    /// Overrideable method which will be invoked when the service updates 
    /// internally.
    /// </summary>
    internal abstract void InternalAddEntity (/* TODO EntityComponentType */ UnityEngine.Component component);

    /// <summary>
    /// Overrideable method which will be invoked when the service updates 
    /// internally.
    /// </summary>
    internal abstract void InternalRemoveEntry (/* TODO EntityComponentType */ UnityEngine.Component component);

    /// <summary>
    /// Method invoked when the system will initialize.
    /// </summary>
    public virtual void OnInitialize () { }

    /// <summary>
    /// Method invoked when the system is initialized.
    /// </summary>
    public virtual void OnInitialized () { }

#if ECS_PHYSICS
    /// <summary>
    /// Method invoked when the physics update, will be called every fixed frame.
    /// </summary>
    public virtual void OnPhysics () { }
#endif

    /// <summary>
    /// Method invoked when the system updates, will be called every frame.
    /// </summary>
    public virtual void OnUpdate () { }

#if ECS_GRAPHICS
    /// <summary>
    /// Method invoked when the camera renders, will be called every late frame.
    /// </summary>
    public virtual void OnRender () { }
#endif

    /// <summary>
    // Method invoked when the system is drawing the gizmos, will be called
    // every gizmos draw call.
    /// </summary>
    public virtual void OnDrawGizmos () { }

    /// <summary>
    // Method invoked when the system is drawing the gui, will be called every
    // on gui draw call.
    /// </summary>
    public virtual void OnDrawGui () { }

    /// <summary>
    // Method invoked when the system becomes enabled.
    /// </summary>
    public virtual void OnEnabled () { }

    /// <summary>
    // Method invoked when the system becomes disabled.
    /// </summary>
    public virtual void OnDisabled () { }

    /// <summary>
    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    public virtual void OnWillDestroy () { }

    /// <summary>
    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    /// </summary>
    public virtual bool ShouldUpdate () { return true; }
  }

  /// <summary>
  /// Generic base class for every entity system.
  /// </summary>
  public abstract class EntitySystem<EntitySystemType, EntityComponentType> : EntitySystem
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new()
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new() {

    /// Defines whether this system has been initialized.
    private bool isInitialized = false;

    /// An instance reference to the controller.
    internal static EntitySystemType Instance { private set; get; } = null;

    /// A list of the system's instantiated entity components.
    public System.Collections.Generic.List<EntityComponentType> entities { private set; get; } = new System.Collections.Generic.List<EntityComponentType> ();

    /// The first instantiated entity compoent if this system has any.
    public EntityComponentType entity { private set; get; } = null;

    /// Defines the number of instantiated entity components this system has any.
    public int entityCount { private set; get; } = 0;

    /// Defines whether the system has instantiated entity components.
    public bool hasEntities { private set; get; } = false;

    /// <summary>
    // Method invoked when an entity of this system is initializing.
    /// </summary>
    public virtual void OnEntityInitialize (EntityComponentType entity) { }

    /// <summary>
    // Method invoked when an entity of this system is initialized.
    /// </summary>
    public virtual void OnEntityInitialized (EntityComponentType entity) { }

    /// <summary>
    // Method invoked when an entity of this system becomes enabled.
    /// </summary>
    public virtual void OnEntityEnabled (EntityComponentType entity) { }

    /// <summary>
    // Method invoked when an entity of this system becomes disabled.
    /// </summary>
    public virtual void OnEntityDisabled (EntityComponentType entity) { }

    /// <summary>
    // Method invoked when an entity of this system will destroy.
    /// </summary>
    public virtual void OnEntityWillDestroy (EntityComponentType entity) { }

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

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    internal override void InternalInitialize () =>
      Instance = Controller.Instance.GetSystem<EntitySystemType> ();

    /// Internal method to update the children of the system.
    internal override void InternalUpdate () {
      if (this.isInitialized == false) {
        this.OnInitialized ();
        if (Controller.Instance.IsSystemEnabled<EntitySystemType> () == true)
          this.OnEnabled ();
        this.isInitialized = true;
      }
      for (var _entityIndex = 0; _entityIndex < this.entityCount; _entityIndex++)
        this.entities[_entityIndex].InternalUpdate ();
    }

    /// Internal method to add an entity's component to this system.
    internal override void InternalAddEntity (UnityEngine.Component component) {
      // TODO -- Fix this by casting component.
      // if (this.hasEntities == false)
      //   this.entity = component;
      // this.entityCount++;
      // this.hasEntities = true;
      // this.entities.Add (component);
      // this.OnEntityInitialize (component);
    }

    /// Internal method to remove an entity's component from this system.
    internal override void InternalRemoveEntry (UnityEngine.Component component) {
      // TODO -- Fix this by casting component.
      // this.entityCount--;
      // this.hasEntities = this.entityCount > 0;
      // this.OnEntityWillDestroy (component);
      // this.entities.Remove (component);
      // this.entity = this.hasEntities == false ? null : this.entities[0];
    }
  }
}
