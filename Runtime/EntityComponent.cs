namespace UnityPackages.EntityComponentSystem {
  public abstract class EntityComponent<C, S> : UnityEngine.MonoBehaviour where C : EntityComponent<C, S>, new () where S : EntitySystem<S, C>, new () {
    private bool isEntityEnabled = false;
    private bool isEntityInitialized = false;
    private S system;

    private S GetSystem () {
      if (this.system == null)
        if (Controller.Instance.HasSystem<S> () == true)
          this.system = Controller.Instance.GetSystem<S> ();
        else
          throw new System.Exception ("Tried to access the system before it was registered");
      return this.system;
    }

    private void Start () {
      this.GetSystem ().AddEntity ((C) this);
      this.GetSystem ().OnEntityStart ((C) this);
    }

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

    private void OnDisable () {
      this.isEntityEnabled = false;
      this.GetSystem ().OnEntityDisabled ((C) this);
    }

    private void OnDestroy () {
      this.GetSystem ().RemoveEntry ((C) this);
    }
  }
}