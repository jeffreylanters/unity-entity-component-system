namespace UnityPackages.EntityComponentSystem {

  // An entity component.
  public abstract class EntityComponent<C, S> : UnityEngine.MonoBehaviour, IEntityComponent where C : EntityComponent<C, S>, new () where S : EntitySystem<S, C>, new () {

    /// Defines whether this component is enabled.
    private bool isEnabled = false;

    /// Defines whether this component has been initialized.
    private bool isInitialized = false;

    /// The system matched with this entity's component.
    private S system = null;

    /// Gets the system matched with this entity's component. If it's not
    /// defined, it will be fetched from the controller.
    private S GetSystem () {
      if (this.system == null)
        if (Controller.Instance.HasSystem<S> () == true)
          this.system = Controller.Instance.GetSystem<S> ();
        else throw new System.Exception ("Tried to access the system before it was registered");
      return this.system;
    }

    /// During the 'Start' the entity component will be registered 
    /// to the matching system.
    private void Start () =>
      this.GetSystem ().Internal_AddEntity ((C) this);

    /// During the 'OnDisabled' the entity component will invoke its
    /// 'OnEntityDisabled' on the system.
    private void OnDisable () {
      this.isEnabled = false;
      this.GetSystem ().OnEntityDisabled ((C) this);
    }

    /// During the 'OnDestroy' the entity component will unregister it self
    /// to the matching system.
    private void OnDestroy () =>
      this.GetSystem ().Internal_RemoveEntry ((C) this);

    /// During the 'InteralOnUpdate' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.isInitialized = true;
        this.GetSystem ().OnEntityInitialized ((C) this);
      }
      if (this.isEnabled == false) {
        this.isEnabled = true;
        this.GetSystem ().OnEntityEnabled ((C) this);
      }
    }
  }
}