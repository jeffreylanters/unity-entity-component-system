namespace UnityPackages.EntityComponentSystem {

  /// A service.
  public abstract class Service<ServiceType> : IService
  where ServiceType : Service<ServiceType>, new () {

    public virtual void OnInitialize () { }
    public virtual void OnInitialized () { }
    public virtual void OnDrawGizmos () { }
    public virtual void OnDrawGui () { }

    /// An instance reference to the service.
    public static ServiceType Instance;

    /// Defines whether this service has been initialized.
    private bool isInitialized = false;

    /// Internal method to set the instance reference. This method will
    /// be called after the controller and system initialization.
    public void Internal_OnInitialize () =>
      Instance = Controller.Instance.GetService<ServiceType> ();

    /// Internal method to update the service.
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.OnInitialized ();
        this.isInitialized = true;
      }
    }
  }
}