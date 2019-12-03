# Entity Component System

![](https://img.shields.io/badge/dependencies-unity--packages-%233bc6d8.svg) ![](https://img.shields.io/badge/license-MIT-%23ecc531.svg)

A better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

> NOTE When using this Unity Package, make sure to **Star** this repository. When using any of the packages please make sure to give credits to **Jeffrey Lanters** somewhere in your app or game. **THESE PACKAGES ARE NOT ALLOWED TO BE SOLD ANYWHERE!**

## Install

```
"com.unity-packages.entity-component-system": "git+https://github.com/unity-packages/entity-component-system"
```

[Click here to read the Unity Packages installation guide](https://github.com/unity-packages/installation)

## Life Cycles

```csharp
           CONTROLLERS   ENTITYSYSTEMS       ╔════════════════════════════╗
                "INITIALIZATION"             ║ * = overridable method     ║
         OnInitialize* ↓                     ║ ; = internal event         ║
                       ↓ *OnInitialize       ╚════════════════════════════╝
                       ↓ *OnEntityInitialize
                       ↓ *OnEntityStart
                       ↓ *OnEntityEnabled
                       ↓ *OnEntityInitialized
        OnInitialized* ↓
                       ↓ ;Reference InjectedSystems
                       ↓ *OnEnabled
                       ↓ *OnInitialized
                    "LOGIC" ← ← ← ← ← ← ← ←
             OnUpdate* ↓                  ↑
                       ↓ *OnUpdate        ↑
                       ↓  → → → → → → → → ↑
                  "RENDERING" ← ← ← ← ← ← ←
            OnDrawGui* ↓                  ↑
                       ↓ *OnDrawGui       ↑
         OnDrawGizmos* ↓                  ↑
                       ↓ *OnDrawGizmos    ↑
                       ↓  → → → → → → → → ↑
               "DECOMMISSIONING"
                       ↓ *OnDisabled
                       ↓ *OnEntityDisabled
                       ↓ *OnEntityWillDestory
```

## Usage Examples

### Controllers

```cs
// Create one controller per project as your core
public class MainController : Controller {

  // Event triggered when the controller is initializing
  public override void OnInitialize () {

    // Use the Register Systems method to register your systems to the controller
    //  This can only be done during the initialization
    this.RegisterSystems (typeof(ItemSystem));

    // EXAMPLE: Use the Enable Systems method to enable any of your registered systems
    this.EnableSystems (typeof(ItemSystem));

    // EXAMPLE: Use the Disable Systems method to disable any of your registered systems
    this.DisableSystems (typeof(ItemSystem));
  }

  // Event triggered when the controller is initialized
  public override void OnInitialized () { }

  // Event triggered when the system is updating
  //   This event is called every frame
  public override void OnUpdate () { }

  // Event triggered when the system is drawing the gizmos
  //   This event is called every gizmos draw call
  public override void OnDrawGizmos () { }
}
```

### Systems

```cs
// Create a system to take control of your entity's component
public class ItemSystem : EntitySystem<ItemSystem, ItemComponent> {

  // EXAMPLE: Use the InjectedSystem attribute to create a permanent
  //   reference other systems. Can only be used within outer systems.
  [InjectedSystem] private InventorySystem inventorySystem;

  // Event triggered when the system is initializing
  public override void OnInitialize () { }

  // Event triggered when the system is updating
  //   This event is called every frame
  public override void OnUpdate () {

    // EXAMPLE: Access the first entity's component
    this.entity;

    // EXAMPLE: Access all the entities components
    foreach (var _entity in this.entities) { }

    // EXAMPLE: Use the cached entity count to improve performance
    this.entityCount;

    // EXAMPLE: Use the cached has entity to improve performance
    this.hasEntities;

    // EXAMPLE: Use the static 'Instance' to access other systems
    InventorySystem.Instance;

    // EXAMPLE: Use the 'GetComponentOnEntity' and 'HasComponentOnEntity'
    //   methods to access other components on entities
    this.GetComponentOnEntity<OtherComponent> (this.firstEntity, entity => { });
    this.HasComponentOnEntity<OtherComponent> (this.firstEntity);

    // EXAMPLE: Use the 'StartCoroutine' and 'StopCoroutine' on IEnumerators
    //   Even though a System is no MonoBehaviour, it still can manage coroutines
    this.StartCoroutine ();
    this.StopCoroutine ();
  }

  // Event triggered when the system is enabled
  public override void OnEnabled () { }

  // Event triggered when the system is initialized
  public override void OnInitialized () { }

  // Event triggered when the system is drawing the gizmos
  //   This event is called every gizmos draw call
  public override void OnDrawGizmos () { }

  // Event triggered when the system is listing for GUI events
  //   This event is called every GUI draw call
  public override void OnDrawGui () { }

  // Event triggered when the system is disabled
  public override void OnDisabled () { }

  // Event triggered when an entity of this system is initializing
  public override void OnEntityInitialize (ItemComponent entity) { }

  // Event triggered when an entity of this system is started
  public override void OnEntityStart (ItemComponent entity) { }

  // Event triggered when an entity of this system is enabled
  public override void OnEntityEnabled (ItemComponent entity) { }

  // Event triggered when an entity of this system is initialized
  public override void OnEntityInitialized (ItemComponent entity) { }

  // Event triggered when an entity of this system is disabled
  public override void OnEntityDisabled (ItemComponent entity) { }

  // Event triggered when an entity of this system will destroy
  public override void OnEntityWillDestroy (ItemComponent entity) { }
}
```

### Components

```cs
// Create a component to provide properties to your entity
// A component should only contain public properties.
public class ItemComponent : EntityComponent<ItemComponent, ItemSystem> {

  // EXAMPLE: Use the 'EditorProtection' attribute to mark properties as inaccessable
  //   The property cannot be changed in the editor inspector
  [EditorProtection] public bool isLegendary;

  // EXAMPLE: Use the 'Reference' attribute to mark this property as a reference
  //   This makes the editor automatically assign the property based on
  //   the property's name in the transforms children in Editor time.
  //   Casing, spaces and dashes will be ignored while searching.
  [EditorReference] public Image itemSprite;
}
```
