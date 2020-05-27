<div align="center">

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/logo.jpg" height="100px"></br>

# Entity Component System

[![npm](https://img.shields.io/badge/upm-3.3.0-232c37.svg?style=for-the-badge)]()
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

Use build in file generator to create new instances for any of these types.

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/lifecycle.png" width="100%"></br>

### Basic Example Usage

```cs
public class MainController : Controller {
  public override void OnInitialize () {
    this.Register (typeof (EnemySystem));
  }
}
public class EnemyComponent : EntityComponent<EnemyComponent, EnemySystem> {
  [Referenced] public BoxCollider collider;
  [Protected] public int level;
  public float speed;
}
public class EnemySystem : EntitySystem<EnemySystem, EnemyComponent> {
  [Injected] public AiSystem aiSystem;
  [Injected] public AudioService AudioService;
  public override void OnUpdate () {
    var _delta = Time.deltaTime;
    foreach (var _entity in this.entities)
      _entity.AddPosition(_delta * _entity.speed, 0, 0);
  }
}
```

### Meta Data

```cs
/// A controller.
public abstract class Controller {

  /// A reference to the controller.
  public static Controller Instance;

  /// Event triggered when the controller is initializing.
  public virtual void OnInitialize ();
  /// Event triggered when the controller is initialized.
  public virtual void OnInitialized ();
  /// Event triggered when the controller updates, will be called every frame.
  public virtual void OnUpdate ();
  // Event triggered when the controller is drawing the gizmos, will be called
  // every gizmos draw call.
  public virtual void OnDrawGui ();

  // Register your systems and services to the controller. This can only be
  // done during 'OnInitialize' cycle.
  public void Register (params System.Type[] typesOf);
  /// Enables systems.
  public void EnableSystems (params System.Type[] typesOf);
  /// Disables systems.
  public void DisableSystems (params System.Type[] typesOf);
  /// Gets a system from this controller.
  public S GetSystem<S> () where S : IEntitySystem, new();
  /// Gets a system from this controller.
  public System.Object GetSystem (System.Type typeOf);
  /// Check whether this controller has a system.
  public bool HasSystem<S> () where S : IEntitySystem, new();
  /// Gets a service from this controller.
  public S GetService<S> () where S : IService, new();
  /// Gets a system from this controller.
  public System.Object GetService (System.Type typeOf);
  /// Check whether this controller has a service.
  public bool HasService<S> () where S : IService, new();
}
```

```cs
// An entity component.
public abstract class EntityComponent<EntityComponentType, EntitySystemType> : UnityEngine.MonoBehaviour, IEntityComponent
  where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new()
  where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new() {

  /// Adds an asset to the entity.
  public void AddAsset (UnityEngine.Object asset);
  /// Loads a resources and adds it as an asset to the entity.
  public void AddAsset (string assetResourcePath);
  /// Sets the position of an entity.
  public void SetPosition (float x, float y, float z = 0);
  /// Adds to the position of an entity.
  public void AddPosition (float x, float y, float z = 0);
  /// Sets the local position of an entity.
  public void SetLocalPosition (float x, float y, float z = 0);
  /// Adds to the local position of an entity.
  public void AddLocalPosition (float x, float y, float z = 0);
  /// Sets the EulerAngles of an entity.
  public void SetEulerAngles (float x, float y, float z);
  /// Adds to the EulerAngles of an entity.
  public void AddEulerAngles (float x, float y, float z);
  /// Sets the local EulerAngles of an entity.
  public void SetLocalEulerAngles (float x, float y, float z);
  /// Adds to the local EulerAngles of an entity.
  public void AddLocalEulerAngles (float x, float y, float z);
  /// Sets the local scale of an entity.
  public void SetLocalScale (float x, float y, float z);
  /// Adds to the local Scale of an entity.
  public void AddLocalScale (float x, float y, float z);
}
```

```cs
/// An entity system.
public abstract class EntitySystem<EntitySystemType, EntityComponentType> : IEntitySystem
  where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new()
  where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new() {

  /// An instance reference to the controller.
  public static EntitySystemType Instance;
  /// A list of the system's instantiated entity components.
  public System.Collections.Generic.List<EntityComponentType> entities;
  /// The first instantiated entity compoent if this system.
  public EntityComponentType entity;
  /// Defines the number of instantiated entity components this system has.
  public int entityCount;
  /// Defines whether the system has instantiated entity components.
  public bool hasEntities;

    /// Event triggered when the system will initialize.
  public virtual void OnInitialize ();
  /// Event triggered when the system is initialized.
  public virtual void OnInitialized ();
  /// Event triggered when the system updates, will be called every frame.
  public virtual void OnUpdate ();
  // Event triggered when the system is drawing the gizmos, will be called
  // every gizmos draw call.
  public virtual void OnDrawGizmos ();
  // Event triggered when the system is drawing the gui, will be called every
  // on gui draw call.
  public virtual void OnDrawGui ();
  // Event triggered when the system becomes enabled.
  public virtual void OnEnabled ();
  // Event triggered when the system becomes disabled.
  public virtual void OnDisabled ();
  // Event triggered when an entity of this system is initializing.
  public virtual void OnEntityInitialize (EntityComponentType entity);
  // Event triggered when an entity of this system is initialized.
  public virtual void OnEntityInitialized (EntityComponentType entity);
  // Event triggered when an entity of this system becomes enabled.
  public virtual void OnEntityEnabled (EntityComponentType entity);
  // Event triggered when an entity of this system becomes disabled.
  public virtual void OnEntityDisabled (EntityComponentType entity);
  // Event triggered when an entity of this system will destroy.
  public virtual void OnEntityWillDestroy (EntityComponentType entity);
  // Event triggered before the system will update, return whether this system
  // should update. will be called every frame.
  public virtual bool ShouldUpdate ();

  /// Returns another component on an entity.
  public void GetComponentOnEntity<C> (EntityComponentType entity, System.Action<C> then);
  /// Returns another component on an entity.
  public C GetComponentOnEntity<C> (EntityComponentType entity);
  /// Checks whether an entity has a specific component.
  public bool HasComponentOnEntity<C> (EntityComponentType entity);
  /// Creates a new entity.
  public EntityComponentType CreateEntity ();
  /// Clones an entity.
  public EntityComponentType CloneEntity (EntityComponentType entity);
  /// Finds entities using a predicate match.
  public EntityComponentType[] MatchEntities (System.Predicate<EntityComponentType> match);
  /// Starts a coroutine on this system.
  public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine);
  /// Stops a given coroutine.
  public void StopCoroutine (System.Collections.IEnumerator routine);
  /// Enables or disables this system.
  public void SetEnabled (bool isEnabled);
  /// Gets the enabled status of this system
  public bool GetEnabled ();
}
```
