namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base interface for Entity Components with internal methods.
  /// </summary>
  internal interface IEntityComponentInternals {
    /// <summary>
    /// Method invoked when an entity will update internally.
    /// </summary>
    void OnUpdateInternal ();
  }
}