namespace ElRaccoone.EntityComponentSystem {

  /// Base interface for every component
  public interface IEntityComponent {

    /// During the 'InteralOnUpdate' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    void Internal_OnUpdate ();
  }
}