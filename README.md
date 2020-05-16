<div align="center">

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/logo.jpg" height="100px"></br>

# Entity Component System

[![npm](https://img.shields.io/badge/upm-3.2.0-232c37.svg?style=for-the-badge)]()
[![license](https://img.shields.io/badge/license-Custom-%23ecc531.svg?style=for-the-badge)](./LICENSE.md)
[![npm](https://img.shields.io/badge/sponsor-donate-E12C9A.svg?style=for-the-badge)](https://paypal.me/jeffreylanters)
[![npm](https://img.shields.io/github/stars/elraccoone/unity-entity-component-system.svg?style=for-the-badge)]()

A better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

When using any of the packages, please make sure to **Star** this repository and give credit to **Jeffrey Lanters / El Raccoone** somewhere in your app or game. **It it prohibited to distribute, sublicense, and/or sell copies of the Software!**

**&Lt;**
[**Installation**](#installation) &middot;
[**Documentation**](#documentation) &middot;
[**License**](./LICENSE.md) &middot;
[**Sponsor**](https://paypal.me/jeffreylanters)
**&Gt;**

**Made with &hearts; by Jeffrey Lanters**

</div>

## Installation

Install using the Unity Package Manager. add the following line to your `manifest.json` file located within your project's packages directory.

```json
"nl.elraccoone.entity-component-system": "git+https://github.com/elraccoone/unity-entity-component-system"
```

## Documentation

### Life Cycles

It's recommended to build your entire project around these life cycle methods.

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/lifecycle.png" width="100%"></br>

### Usage Examples

Examples of all the overridable methods, methods and properties of the module.

#### Controllers

```cs
// Create one controller per project as your core
public class MainController : Controller {

  // Event triggered when the controller is initializing
  public override void OnInitialize () {

    // Use the Register Systems or services method to register your systems to the controller
    //  This can only be done during 'OnInitialize'
    this.Register (typeof(ItemSystem), typeof(SpawnService));

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

  // Event triggered when the system is drawing the gui
  //   This event is called every gui draw call
  public override void OnDrawGui () { }
}
```

#### Systems

```cs
// Create a system to take control of your entity's component
public class ItemSystem : EntitySystem<ItemSystem, ItemComponent> {

  // EXAMPLE: Use the Injected attribute to create a permanent
  //   reference other systems or services inside systems or services.
  [Injected] private InventorySystem inventorySystem;
  [Injected] private SpawnService spawnService;

  // Event triggered when the system is initializing
  public override void OnInitialize () { }

  // Event triggered before the system is updating
  // Return whether this system should update.
  //   This event is called every frame
  public override bool ShouldUpdate () {
    return true;
  }

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
    this.GetComponentOnEntity<OtherComponent> (this.entity, entity => { });
    this.HasComponentOnEntity<OtherComponent> (this.entity);

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

#### Components

```cs
// Create a component to provide properties to your entity
// A component should only contain public properties.
public class ItemComponent : EntityComponent<ItemComponent, ItemSystem> {

  // EXAMPLE: Use the 'Protected' attribute to mark properties as inaccessable
  //   The property cannot be changed in the editor inspector
  [Protected] public bool isLegendary;

  // EXAMPLE: Use the 'Referenced' attribute to mark this property as a reference
  //   This makes the editor automatically assign the property based on
  //   the property's name in the transforms children in Editor time.
  //   Casing, spaces and dashes will be ignored while searching.
  [Referenced] public Image itemSprite;
}
```

#### Services

```cs
// Create a services to provide data or handle other non scene logic.
public class SpawnService : Service<SpawnService> {

  // EXAMPLE: Use the Injected attribute to create a permanent
  //   reference other systems or services inside systems or services.
  [Injected] private InventorySystem inventorySystem;
  [Injected] private SpawnService spawnService;

  // Event triggered when the system is initializing
  public override void OnInitialize () { }

  // Event triggered when the system is initialized
  public override void OnInitialized () { }

  // Event triggered when the system is drawing the gizmos
  //   This event is called every gizmos draw call
  public override void OnDrawGizmos () { }

  // Event triggered when the system is listing for GUI events
  //   This event is called every GUI draw call
  public override void OnDrawGui () { }
}
```
