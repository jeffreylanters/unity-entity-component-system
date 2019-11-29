namespace UnityPackages.EntityComponentSystem {

  /// Base interface for every system
  public interface ISystem {

    /// ---
    void OnInitialize ();

    /// ---
    void OnInitialized ();

    /// ---
    void OnEnabled ();

    /// ---
    void OnDisabled ();

    /// ---
    void OnUpdate ();

    /// ---
    void OnDrawGizmos ();

    /// ---
    void OnGUI ();

    /// ---
    void SetEnabled (bool isEnabled);

    /// ---
    bool GetEnabled ();

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    void InternalOnInitialize ();
  }
}