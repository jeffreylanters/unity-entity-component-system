using UnityEngine;

namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base class for Entity Components.
  /// </summary>
  /// <typeparam name="EntityComponentType">The Entity Component type.</typeparam>
  /// <typeparam name="EntitySystemType">The Entity System type.</typeparam>
  public abstract class EntityComponent<EntityComponentType, EntitySystemType> : UnityEngine.MonoBehaviour, IEntityComponent, IEntityComponentInternals
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new()
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new() {
    /// <summary>
    /// Defines whether this component has been initialized.
    /// </summary>
    bool isInitialized = false;

    /// <summary>
    /// The system matched with this entity's component.
    /// </summary>
    EntitySystemType system = null;

    /// <summary>
    /// Defines whether this component is enabled.
    /// </summary>
    public bool isEnabled { get; private set; } = false;

    /// <summary>
    /// Gets the system matched with this entity's component. If it's not
    /// defined, it will be fetched from the controller.
    /// </summary>
    /// <returns>The system.</returns>
    /// <exception cref="System.Exception"></exception>
    EntitySystemType GetSystem () {
      if (system != null)
        return system;
      if (Controller.Instance.HasSystem<EntitySystemType> ()) {
        system = Controller.Instance.GetSystem<EntitySystemType> ();
        return system;
      }
      throw new System.Exception ("Tried to access the System before it was registered");
    }

    /// <summary>
    /// Event invoked by the Unity Engine when the component is started.
    /// </summary>
    void Start () {
      var system = GetSystem ();
      // Add the entity to the system.
      system.AddEntity ((EntityComponentType)this);
    }

    /// <summary>
    /// Event invoked by the Unity Engine when the component is disabled.
    /// </summary> 
    void OnDisable () {
      isEnabled = false;
      var system = GetSystem ();
      // Remove the entity from the system.
      system.OnEntityDisabled ((EntityComponentType)this);
    }

    /// <summary>
    /// Event invoked by the Unity Engine when the component is destroyed.
    /// </summary>
    void OnDestroy () {
      var system = GetSystem ();
      // Remove the entity from the system.
      system.RemoveEntry ((EntityComponentType)this);
    }


    /// <summary>
    /// Method invoked when an entity will update internally.
    /// </summary>
    void IEntityComponentInternals.OnUpdateInternal () {
      var system = GetSystem ();
      if (isInitialized == false) {
        // When the entity was not initialized, initialize it.
        isInitialized = true;
        system.OnEntityInitialized ((EntityComponentType)this);
      }
      if (!isEnabled && gameObject.activeInHierarchy) {
        // When the entity was not enabled, but it is active in the hierarchy,
        // enable it and invoke the event.
        isEnabled = true;
        system.OnEntityEnabled ((EntityComponentType)this);
      }
    }

    /// <summary>
    /// Sets the game object of the entity active.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetActive (bool value) {
      gameObject.SetActive (value);
    }

    /// <summary>
    /// Destroys the entity's game object.
    /// </summary>
    public void Destroy () {
      // Destroy the game object.
      Object.Destroy (gameObject);
    }
  }
}
