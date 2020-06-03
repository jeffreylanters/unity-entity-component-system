namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for every service
  public interface IService {

    /// Method invoked when the service will initialize.
    void OnInitialize ();

    /// Method invoked when the system is initialized.
    void OnInitialized ();

    // Method invoked when the service is drawing the gizmos, will be called
    // every gizmos draw call.
    void OnDrawGizmos ();

    // Method invoked when the service is drawing the gui, will be called every
    // on gui draw call.
    void OnDrawGui ();

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and service initialization.
    void Internal_OnInitialize ();

    /// Internal method to update the service.
    void Internal_OnUpdate ();
  }
}