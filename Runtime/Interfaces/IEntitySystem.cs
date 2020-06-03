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

    /// Method invoked when the system updates, will be called every frame.
    void OnUpdate ();

    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    bool ShouldUpdate ();

    // Method invoked when the system is drawing the gizmos, will be called
    // every gizmos draw call.
    void OnDrawGizmos ();

    // Method invoked when the system is drawing the gui, will be called every
    // on gui draw call.
    void OnDrawGui ();

    /// Enables or disables this system.
    void SetEnabled (bool isEnabled);

    /// Gets the enabled status of this system
    bool GetEnabled ();

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void Internal_OnInitialize ();

    /// Internal method to update the children of the system.
    void Internal_OnUpdate ();
  }
}