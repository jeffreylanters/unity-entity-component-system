namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base interface for Entity Systems.
  /// </summary>
  public interface IEntitySystem {
    /// <summary>
    /// Method invoked when the system will initialize.
    /// </summary>
    void OnInitialize ();

    /// <summary>
    /// Method invoked when the system is initialized.
    /// </summary>
    void OnInitialized ();

    /// <summary>
    // Method invoked when the system becomes enabled.
    /// </summary>
    void OnEnabled ();

    /// <summary>
    // Method invoked when the system becomes disabled.
    /// </summary>
    void OnDisabled ();

#if ECS_PHYSICS
    /// <summary>
    /// Method invoked when the physics update, will be called every fixed frame.
    /// </summary>
    void OnPhysics ();
#endif

    /// <summary>
    /// Method invoked when the system updates, will be called every frame.
    /// </summary>
    void OnUpdate ();

#if ECS_GRAPHICS
    /// <summary>
    /// Method invoked when the camera renders, will be called every late frame.
    /// </summary>
    void OnRender ();
#endif

    /// <summary>
    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    /// </summary>
    bool ShouldUpdate ();

    /// <summary>
    // Method invoked when the system is drawing the gizmos, will be called
    // every gizmos draw call.
    /// </summary>
    void OnDrawGizmos ();

    /// <summary>
    // Method invoked when the system is drawing the gui, will be called every
    // on gui draw call.
    /// </summary>
    void OnDrawGui ();

    /// <summary>
    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    void OnWillDestroy ();

    /// <summary>
    /// Sets whether the system is enabled or disabled, enabling the system allows
    /// it to invoke all of the cycle calls such as OnUpdate and OnDrawGizmos.
    /// </summary>
    void SetEnabled (bool value);
  }
}