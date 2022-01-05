namespace ElRaccoone.EntityComponentSystem {

  /// <summary>
  /// Base interface for every service.
  /// </summary>
  public abstract class Service : IRegisterable {

    /// <summary>
    /// Overrideable method which will be called when the service is 
    /// initializing internally.
    /// </summary>
    internal abstract void InternalInitialize ();

    /// <summary>
    /// Overrideable method which will be invoked when the service updates 
    /// internally.
    /// </summary>
    internal abstract void InternalUpdate ();

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

    /// Method invoked when the service will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    public virtual void OnWillDestroy () { }
  }

  /// Generic base class for every service.
  public abstract class Service<ServiceType> : Service
    where ServiceType : Service<ServiceType>, new() {

    /// Defines whether this service has been initialized.
    private bool isInitialized = false;

    /// An instance reference to the service.
    internal static ServiceType Instance { private set; get; } = null;

    /// Starts a coroutine on this service.
    public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StartCoroutine (routine);

    /// Stops a given coroutine.
    public void StopCoroutine (System.Collections.IEnumerator routine) =>
      Controller.Instance.StopCoroutine (routine);

    /// <summary>
    /// Event invoked after the controller and system initialization.
    /// </summary>
    internal override void InternalInitialize () {
      // Set the instance reference.
      Instance = Controller.Instance.GetService<ServiceType> ();
    }

    /// <summary>
    /// Event invoked when the controller is updating.
    /// </summary>
    internal override void InternalUpdate () {
      // If not initialized, initialize.
      if (this.isInitialized == false) {
        this.OnInitialized ();
        this.isInitialized = true;
      }
    }
  }
}