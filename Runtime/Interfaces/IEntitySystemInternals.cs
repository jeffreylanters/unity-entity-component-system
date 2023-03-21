namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base interface for Entity Systems with internal methods.
  /// </summary>
  internal interface IEntitySystemInternals {
    /// <summary>
    /// Method invoked when the system will initialize internally.
    /// </summary>
    void OnInitializeInternal ();

    /// <summary>
    /// Method invoked when the system updates internally, will be called every 
    /// frame.
    /// </summary>
    void OnUpdateInternal ();
  }
}