namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for service internals.
  public interface IServiceInternals {
    /// Internal method to set the instance reference. This method will
    /// be called after the controller and service initialization.
    void OnInitializeInternal ();

    /// Internal method to update the service.
    void OnUpdateInternal ();
  }
}