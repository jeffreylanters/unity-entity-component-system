namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for component internals.
  internal interface IEntityComponentInternals {

    /// During the 'InteralOnUpdate' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    void OnUpdateInternal ();
  }
}