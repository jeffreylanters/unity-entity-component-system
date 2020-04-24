namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for every service
  public interface IService {

    /// ---
    void OnInitialize ();

    /// ---
    void OnInitialized ();

    /// ---
    void OnDrawGizmos ();

    /// ---
    void OnDrawGui ();

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and service initialization.
    void Internal_OnInitialize ();

    /// Internal method to update the service.
    void Internal_OnUpdate ();
  }
}