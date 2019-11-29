namespace UnityPackages.EntityComponentSystem {
  /// Base interface for every system
  public interface ISystem {
    void OnInitialize ();
    void OnInitialized ();
    void OnInitializeInternal ();
    void OnEnabled ();
    void OnDisabled ();
    void OnUpdate ();
    void OnDrawGizmos ();
    void OnGUI ();
    bool isEnabled { get; set; }
  }
}