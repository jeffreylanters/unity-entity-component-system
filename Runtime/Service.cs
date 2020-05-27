namespace ElRaccoone.EntityComponentSystem {

  /// A service.
  public abstract class Service<ServiceType> : IService
    where ServiceType : Service<ServiceType>, new() {

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnDrawGizmos () { }
    public virtual void OnDrawGui () { }

    /// An instance reference to the service.
    public static ServiceType Instance;

    /// Defines whether this service has been initialized.
    private bool isInitialized = false;

    /// Starts a coroutine on this service.
    public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StartCoroutine (routine);

    /// Stops a given coroutine.
    public void StopCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StopCoroutine (routine);

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    [Internal]
    public void Internal_OnInitialize () =>
      Instance = Controller.Instance.GetService<ServiceType> ();

    /// Internal method to update the service.
    [Internal]
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.OnInitialized ();
        this.isInitialized = true;
      }
    }
  }
}