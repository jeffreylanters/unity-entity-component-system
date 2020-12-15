namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for every system
  public interface IEntitySystem {

    /// Method invoked when the system will initialize.
    void OnInitialize ();

    /// Method invoked when the system is initialized.
    void OnInitialized ();

    // Method invoked when the system becomes enabled.
    void OnEnabled ();

    // Method invoked when the system becomes disabled.
    void OnDisabled ();

#if ECS_PHYSICS
    /// Method invoked when the physics update, will be called every fixed frame.
    void OnPhysics ();
#endif

    /// Method invoked when the system updates, will be called every frame.
    void OnUpdate ();

#if ECS_GRAPHICS
    /// Method invoked when the camera renders, will be called every late frame.
    void OnRender ();
#endif

    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    bool ShouldUpdate ();

    // Method invoked when the system is drawing the gizmos, will be called
    // every gizmos draw call.
    void OnDrawGizmos ();

    // Method invoked when the system is drawing the gui, will be called every
    // on gui draw call.
    void OnDrawGui ();

    /// Sets whether the system is enabled or disabled, enabling the system allows
    /// it to invoke all of the cycle calls such as OnUpdate and OnDrawGizmos.
    void SetEnabled (bool value);

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void Internal_OnInitialize ();

    /// Internal method to update the children of the system.
    void Internal_OnUpdate ();
  }
}