namespace ElRaccoone.EntityComponentSystem {

  /// Base class for every service.
  public abstract class Service<ServiceType> : IService
    where ServiceType : Service<ServiceType>, new() {

    /// Defines whether this service has been initialized.
    private bool isInitialized = false;

    /// An instance reference to the service.
    public static ServiceType Instance { private set; get; } = null;

    /// Method invoked when the service will initialize.
    public virtual void OnInitialize () { }

    /// Method invoked when the system is initialized.
    public virtual void OnInitialized () { }

    // Method invoked when the service is drawing the gizmos, will be called
    // every gizmos draw call.
    public virtual void OnDrawGizmos () { }

    // Method invoked when the service is drawing the gui, will be called every
    // on gui draw call.
    public virtual void OnDrawGui () { }

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