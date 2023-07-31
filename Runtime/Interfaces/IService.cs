namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base interface for Services.
  /// </summary>
  public interface IService {
    /// <summary>
    /// Method invoked when the service will initialize.
    /// </summary>
    void OnInitialize ();

    /// <summary>
    /// Method invoked when the system is initialized.
    /// </summary>
    void OnInitialized ();

    /// <summary>
    /// Method invoked when the service updates, will be called every frame.
    /// </summary>
    void OnUpdate () { }

    /// <summary>
    // Method invoked before the system will update, return whether this system
    // should update. will be called every frame.
    /// </summary>
    bool ShouldUpdate ();

    /// <summary>
    // Method invoked when the service is drawing the gizmos, will be called
    // every gizmos draw call.
    /// </summary>
    void OnDrawGizmos ();

    /// <summary>
    // Method invoked when the service is drawing the gui, will be called every
    // on gui draw call.
    /// </summary>
    void OnDrawGui ();

    /// <summary>
    /// Method invoked when the service will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    void OnWillDestroy ();
  }
}