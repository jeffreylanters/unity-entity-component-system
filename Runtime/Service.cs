using System.Collections;

namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base class for Services.
  /// </summary>
  /// <typeparam name="ServiceType">The type of the service.</typeparam>
  public abstract class Service<ServiceType> : IService, IServiceInternals
    where ServiceType : Service<ServiceType>, new() {
    /// <summary>
    /// An instance reference to the service.
    /// </summary>
    public static ServiceType Instance { private set; get; } = null;

    /// <summary>
    /// Defines whether this service has been initialized.
    /// </summary>
    bool isInitialized = false;

    /// <summary>
    /// Method invoked when the service will initialize internally.
    /// </summary>
    void IServiceInternals.OnInitializeInternal () {
      // Set the instance reference.
      Instance = Controller.Instance.GetService<ServiceType> ();
    }

    /// <summary>
    /// Method invoked when the service updates internally, will be called every 
    /// frame.
    /// </summary>
    void IServiceInternals.OnUpdateInternal () {
      if (isInitialized) {
        // When the Service is initialized already, do nothing.
        return;
      }
      // Initialize the service.
      OnInitialized ();
      isInitialized = true;
    }

    /// <summary>
    /// Method invoked when the service will initialize.
    /// </summary>
    public virtual void OnInitialize () { }

    /// <summary>
    /// Method invoked when the system is initialized.
    /// </summary>
    public virtual void OnInitialized () { }

    /// <summary>
    /// Method invoked when the service is drawing the gizmos, will be called
    /// every gizmos draw call.
    /// </summary>
    public virtual void OnDrawGizmos () { }

    /// <summary>
    /// Method invoked when the service is drawing the gui, will be called every
    /// on gui draw call.
    /// </summary>
    public virtual void OnDrawGui () { }

    /// <summary>
    /// Method invoked when the service will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    public virtual void OnWillDestroy () { }

    /// <summary>
    /// Starts a coroutine on this service.
    /// </summary>
    /// <param name="routine">The coroutine to start.</param>
    /// <returns>The coroutine reference.</returns>
    public UnityEngine.Coroutine StartCoroutine (IEnumerator routine) {
      // Use the controller to start the coroutine.
      return Controller.Instance.StartCoroutine (routine);
    }

    /// <summary>
    /// Stops a given coroutine.
    /// </summary>
    /// <param name="routine">The coroutine to stop.</param>
    public void StopCoroutine (IEnumerator routine) {
      // Use the controller to stop the coroutine.
      Controller.Instance.StopCoroutine (routine);
    }
  }
}