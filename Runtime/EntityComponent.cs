namespace ElRaccoone.EntityComponentSystem {

  // An entity component.
  public abstract class EntityComponent<EntityComponentType, EntitySystemType> : UnityEngine.MonoBehaviour, IEntityComponent
    where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new()
    where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new() {

    /// Defines whether this component is enabled.
    private bool isEnabled = false;

    /// Defines whether this component has been initialized.
    private bool isInitialized = false;

    /// The system matched with this entity's component.
    private EntitySystemType system = null;

    /// Gets the system matched with this entity's component. If it's not
    /// defined, it will be fetched from the controller.
    private EntitySystemType GetSystem () {
      if (this.system == null)
        if (Controller.Instance.HasSystem<EntitySystemType> () == true)
          this.system = Controller.Instance.GetSystem<EntitySystemType> ();
        else throw new System.Exception ("Tried to access the system before it was registered");
      return this.system;
    }

    /// During the 'Start' the entity component will be registered 
    /// to the matching system.
    private void Start () =>
      this.GetSystem ().Internal_AddEntity ((EntityComponentType)this);

    /// During the 'OnDisabled' the entity component will invoke its
    /// 'OnEntityDisabled' on the system.
    private void OnDisable () {
      this.isEnabled = false;
      this.GetSystem ().OnEntityDisabled ((EntityComponentType)this);
    }

    /// During the 'OnDestroy' the entity component will unregister it self
    /// to the matching system.
    private void OnDestroy () =>
      this.GetSystem ().Internal_RemoveEntry ((EntityComponentType)this);

    /// During the 'InteralOnUpdate' the entity component will invoke its 
    /// 'OnEntityEnabled' and 'OnEntityInitialized' if needed.
    [Internal]
    public void Internal_OnUpdate () {
      if (this.isInitialized == false) {
        this.isInitialized = true;
        this.GetSystem ().OnEntityInitialized ((EntityComponentType)this);
      }
      if (this.isEnabled == false && this.gameObject.activeInHierarchy == true) {
        this.isEnabled = true;
        this.GetSystem ().OnEntityEnabled ((EntityComponentType)this);
      }
    }
  }
}