namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for system internals.
  internal interface IEntitySystemInternals {
    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void OnInitializeInternal ();

    /// Internal method to update the children of the system.
    void OnUpdateInternal ();
  }
}