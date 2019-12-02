namespace UnityPackages.EntityComponentSystem {
  
  // An entity component.
  public abstract class EntityComponent<C, S> : UnityEngine.MonoBehaviour where C : EntityComponent<C, S>, new () where S : EntitySystem<S, C>, new () {

    private bool isEntityEnabled = false;
    private bool isEntityInitialized = false;
    private S system = null;

    /// Gets the system matched with this entity's component. If it's not
    /// defined, it will be fetched from the controller.
    private S GetSystem () {
      if (this.system == null)
        if (Controller.Instance.HasSystem<S> () == true)
          this.system = Controller.Instance.GetSystem<S> ();
        else
          throw new System.Exception ("Tried to access the system before it was registered");
      return this.system;
    }

    /// During the 'Start' the entity component will be registered 
    /// to the matching system.
    private void Start () =>
      this.GetSystem ().InternalAddEntity ((C) this);

    /// During the 'Update' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    private void Update () {
      if (this.isEntityEnabled == false) {
        this.isEntityEnabled = true;
        this.GetSystem ().OnEntityEnabled ((C) this);
      }
      if (this.isEntityInitialized == false) {
        this.isEntityInitialized = true;
        this.GetSystem ().OnEntityInitialized ((C) this);
      }
    }

    /// During the 'OnDisabled' the entity component will invoke its
    /// 'OnEntityDisabled' on the system.
    private void OnDisable () {
      this.isEntityEnabled = false;
      this.GetSystem ().OnEntityDisabled ((C) this);
    }

    /// During the 'OnDestroy' the entity component will unregister it self
    /// to the matching system.
    private void OnDestroy () =>
      this.GetSystem ().InternalRemoveEntry ((C) this);
  }
}
