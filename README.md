# Entity Component System

![](https://img.shields.io/badge/dependencies-unity--packages-%233bc6d8.svg) ![](https://img.shields.io/badge/license-MIT-%23ecc531.svg)

This simple ECS offers a better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

> NOTE When using this Unity Package, make sure to **Star** this repository. When using any of the packages please make sure to give credits to **Jeffrey Lanters** somewhere in your app or game. **THESE PACKAGES ARE NOT ALLOWED TO BE SOLD ANYWHERE!**

## Install

```
"com.unity-packages.entity-component-system": "git+https://github.com/unity-packages/entity-component-system"
```

[Click here to read the Unity Packages installation guide](https://github.com/unity-packages/installation)

## Usage

### Controllers

Controllers are the main part of our ECS, there should only be one per at a time. The controller initialized the systems you want to be active.

```cs
public class MainController : ECS.Controller {

  public override void OnInitialize () {
    this.RegisterSystems (
      // new ItemSystem()
    );
  }

  public override void OnInitialized () { }

  public override void OnUpdate () { }
}
```

### Systems

The systems are controlled by the controller and will controll the component entities it owns.

```cs
public class ItemSystem : ECS.System<ItemSystem, ItemComponent> {

  public override void OnInitialize () { }

  public override void OnUpdate () {
    this.GetComponentOnEntity<OtherComponent> (this.firstEntity, component => { /* ... */ });
    this.HasComponentOnEntity<OtherComponent> (this.firstEntity);

    this.firstEntity;

    foreach (var _entity in this.entities) {
      // use the entity...
    }
  }

  public override void OnDrawGizmos () { }

  public override void OnGUI () { }

  public override void OnEntityInitialize (ItemComponent entity) { }

  public override void OnEntityInitialized (ItemComponent entity) { }

  public override void OnEntityStart (ItemComponent entity) { }

  public override void OnEntityEnabled (ItemComponent entity) { }

  public override void OnEntityDisabled (ItemComponent entity) { }

  public override void OnEntityWillDestroy (ItemComponent entity) { }
}
```

### Components

The components are controlled by the systems and may only contain public data.

```cs
public class ItemComponent : ECS.Component<ItemComponent, ItemSystem> {

  // Example values
  [ECS.Protected] public bool myProtectedBool;
  [ECS.Reference] public Transform myReferencesTransform;
}
```
