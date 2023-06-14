using System.Collections.Generic;
using UnityEngine;

namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base class for Entity Systems.
  /// </summary>
  /// <typeparam name="EntitySystemType">The type of the entity system.</typeparam>
  /// <typeparam name="EntityComponentType">The type of the entity component.</typeparam>
  public abstract class EntitySystem<EntitySystemType, EntityComponentType> : IEntitySystem, IEntitySystemInternals
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new()
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new() {
    /// <summary>
    /// Defines whether this system has been initialized.
    /// </summary>
    bool isInitialized = false;

    /// <summary>
    /// An instance reference to the controller.
    /// </summary>
    public static EntitySystemType Instance { private set; get; } = null;

    /// <summary>
    /// A list of the system's instantiated entity components.
    /// </summary>
    public List<EntityComponentType> entities { private set; get; } = new List<EntityComponentType> ();

    /// <summary>
    /// The first instantiated entity compoent if this system has any.
    /// </summary>
    public EntityComponentType entity { private set; get; } = null;

    /// <summary>
    /// Defines the number of instantiated entity components this system has any.
    /// </summary>
    public int entityCount { private set; get; } = 0;

    /// <summary>
    /// Defines whether the system has instantiated entity components.
    /// </summary>
    public bool hasEntities { private set; get; } = false;

    /// <summary>
    /// Adds an entity component to this system.
    /// </summary>
    /// <param name="entity">The entity component to add.</param>
    internal void AddEntity (EntityComponentType component) {
      if (hasEntities == false) {
        // Set the first entity component if this system has no entities.
        entity = component;
      }
      // Add the entity component to the list.
      entityCount++;
      hasEntities = true;
      entities.Add (component);
      // Invoke the entity initialize method.
      OnEntityInitialize (component);
    }

    /// <summary>
    /// Method invoked when an entity component is added to this system.
    /// </summary>
    /// <param name="entity">The entity component that was added.</param>
    internal void RemoveEntry (EntityComponentType component) {
      // Remove the entity component from the list.
      entityCount--;
      hasEntities = entityCount > 0;
      // Invoke the entity destroy method.
      OnEntityWillDestroy (component);
      entities.Remove (component);
      // Set the first entity component if this system has no entities.
      entity = hasEntities == false ? null : entities[0];
    }

    /// <summary>
    /// Method invoked when the system will initialize internally.
    /// </summary>
    void IEntitySystemInternals.OnInitializeInternal () {
      // Set the instance reference.
      Instance = Controller.Instance.GetSystem<EntitySystemType> ();
    }

    /// <summary>
    /// Method invoked when the system updates internally, will be called every 
    /// frame.
    /// </summary>
    void IEntitySystemInternals.OnUpdateInternal () {
      if (isInitialized == false) {
        // If the system is not initialized, invoke the initialize methods.
        OnInitialized ();
        if (Controller.Instance.IsSystemEnabled<EntitySystemType> ()) {
          // If the system is enabled, invoke the enabled methods.
          OnEnabled ();
        }
        isInitialized = true;
      }
      for (var entityIndex = 0; entityIndex < entityCount; entityIndex++) {
        // Invoke the entity update method.
        var entityInternals = entities[entityIndex] as IEntityComponentInternals;
        entityInternals.OnUpdateInternal ();
      }
    }

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
    /// Method invoked when the system becomes enabled.
    /// </summary>
    public virtual void OnEnabled () { }

    /// <summary>
    /// Method invoked when the system becomes disabled.
    /// </summary>
    public virtual void OnDisabled () { }

    /// <summary>
    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    public virtual void OnWillDestroy () { }

    /// <summary>
    /// Method invoked when an entity of this system is initializing.
    /// </summary>
    public virtual void OnEntityInitialize (EntityComponentType entity) { }

    /// <summary>
    /// Method invoked when an entity of this system is initialized.
    /// </summary>
    public virtual void OnEntityInitialized (EntityComponentType entity) { }

    /// <summary>
    /// Method invoked when an entity of this system becomes enabled.
    /// </summary>
    public virtual void OnEntityEnabled (EntityComponentType entity) { }

    /// <summary>
    /// Method invoked when an entity of this system becomes disabled.
    /// </summary>
    public virtual void OnEntityDisabled (EntityComponentType entity) { }

    /// <summary>
    /// Method invoked when an entity of this system will destroy.
    /// </summary>
    public virtual void OnEntityWillDestroy (EntityComponentType entity) { }

    /// <summary>
    /// Method invoked before the system will update, return whether this system
    /// should update. will be called every frame.
    /// </summary>
    public virtual bool ShouldUpdate () { return true; }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <returns>The created entity.</returns>
    public EntityComponentType CreateEntity () {
      var gameObject = new UnityEngine.GameObject ("Entity " + typeof (EntityComponentType).Name);
      var component = gameObject.AddComponent<EntityComponentType> ();
      // Return the created entity.
      return component;
    }

    /// <summary>
    /// Clones an entity.
    /// </summary>
    /// <param name="entity">The entity to clone.</param>
    /// <returns>The cloned entity.</returns>
    public EntityComponentType CloneEntity (EntityComponentType entity) {
      var gameObject = Object.Instantiate (entity);
      var component = gameObject.GetComponent<EntityComponentType> ();
      // Return the cloned entity.
      return component;
    }

    /// <summary>
    /// Clones an entity on a given position in the hierarchy.
    /// </summary>
    /// <param name="entity">The entity to clone.</param>
    /// <param name="parentTransform">The parent transform to clone the entity on.</param>
    /// <returns>The cloned entity.</returns>
    public EntityComponentType CloneEntity (EntityComponentType entity, Transform parentTransform) {
      var gameObject = Object.Instantiate (entity, parentTransform);
      var component = gameObject.GetComponent<EntityComponentType> ();
      // Return the cloned entity.
      return component;
    }

    /// <summary>
    /// Finds entities using a predicate match.
    /// </summary>
    /// <param name="match">The predicate match to find the entities.</param>
    /// <returns>The found entities.</returns>
    public EntityComponentType[] MatchEntities (System.Predicate<EntityComponentType> match) {
      var matchedEntities = entities.FindAll (match).ToArray ();
      // Return the matched entities.
      return matchedEntities;
    }

    /// <summary>
    /// Finds an entity using a predicate match.
    /// </summary>
    /// <param name="match">The predicate match to find the entity.</param>
    /// <returns>The found entity.</returns>
    public EntityComponentType MatchEntity (System.Predicate<EntityComponentType> match) {
      var matchedEntity = entities.Find (match);
      // Return the matched entity.
      return matchedEntity;
    }

    /// <summary>
    /// Starts a coroutine on this system.
    /// </summary>
    /// <typeparam name="IEnumeratorType">The type of the coroutine.</typeparam>
    /// <param name="routine">The coroutine to start.</param>
    /// <returns>The coroutine.</returns>
    public Coroutine StartCoroutine<IEnumeratorType> (IEnumerator<IEnumeratorType> routine) {
      // Start the coroutine on the controller.
      return Controller.Instance.StartCoroutine (routine);
    }

    /// <summary>
    /// Stops a given coroutine.
    /// </summary>
    /// <param name="routine">The coroutine to stop.</param>
    public void StopCoroutine<IEnumeratorType> (IEnumerator<IEnumeratorType> routine) {
      // Stop the coroutine on the controller.
      Controller.Instance.StopCoroutine (routine);
    }

    /// <summary>
    /// Stops a given coroutine.
    /// </summary>
    /// <param name="routine">The coroutine to stop.</param>
    public void StopCoroutine (Coroutine routine) {
      // Stop the coroutine on the controller.
      Controller.Instance.StopCoroutine (routine);
    }

    /// <summary>
    /// Sets whether the system is enabled or disabled, enabling the system
    /// allows it to invoke all of the cycle calls such as OnUpdate and 
    /// OnDrawGizmos.
    /// </summary>
    /// <param name="value">True to enable the system, otherwise false.</param>
    public void SetEnabled (bool value) {
      Controller.Instance.SetSystemEnabled<EntitySystemType> (value);
    }
  }
}
