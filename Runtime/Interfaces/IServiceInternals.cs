namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base interface for Services with internal methods.
  /// </summary>
  public interface IServiceInternals {
    /// <summary>
    /// Method invoked when the service will initialize internally.
    /// </summary> 
    void OnInitializeInternal ();

    /// <summary>
    /// Method invoked when the service updates internally, will be called every 
    /// frame.
    /// </summary>
    void OnUpdateInternal ();
  }
}