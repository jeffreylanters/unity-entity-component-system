namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for every system
  public interface IEntitySystem {

    /// ---
    void OnInitialize ();

    /// ---
    void OnInitialized ();

    /// ---
    void OnEnabled ();

    /// ---
    void OnDisabled ();

    /// ---
    void OnUpdate ();

    /// ---
    bool ShouldUpdate ();

    /// ---
    void OnDrawGizmos ();

    /// ---
    void OnDrawGui ();

    /// ---
    void SetEnabled (bool isEnabled);

    /// ---
    bool GetEnabled ();

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void Internal_OnInitialize ();

    /// Internal method to update the children of the system.
    void Internal_OnUpdate ();
  }
}