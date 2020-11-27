<div align="center">

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/logo.jpg" height="100px">

</br>

# Entity Component System

[![npm](https://img.shields.io/badge/upm-3.5.0-232c37.svg?style=for-the-badge)]()
[![npm](https://img.shields.io/github/stars/elraccoone/unity-entity-component-system.svg?style=for-the-badge)]()
[![npm](https://img.shields.io/badge/build-passing-brightgreen.svg?style=for-the-badge)]()

A better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

**&Lt;**
[**Installation**](#installation) &middot;
[**Documentation**](#documentation) &middot;
[**License**](./LICENSE.md)
**&Gt;**

</br></br>

[![npm](https://img.shields.io/badge/sponsor_the_project-donate-E12C9A.svg?style=for-the-badge)](https://paypal.me/jeffreylanters)

Hi! My name is Jeffrey Lanters, thanks for checking out my modules! I've been a Unity developer for years when in 2020 I decided to start sharing my modules by open-sourcing them. So feel free to look around. If you're using this module for production, please consider donating to support the project. When using any of the packages, please make sure to **Star** this repository and give credit to **Jeffrey Lanters** somewhere in your app or game. Also keep in mind **it it prohibited to sublicense and/or sell copies of the Software in stores such as the Unity Asset Store!** Thanks for your time.

**&Lt;**
**Made with &hearts; by Jeffrey Lanters**
**&Gt;**

</br>

</div>

# Installation

Install the latest stable release using the Unity Package Manager by adding the following line to your `manifest.json` file located within your project's Packages directory.

```json
"nl.elraccoone.entity-component-system": "git+https://github.com/elraccoone/unity-entity-component-system"
```

# Documentation

## Getting Started

It's recommended to get started by using the built-in File Generator. When it's your first time using the ECS, you might want to enable the _Overwrite All Virtuals_ option to see all the available methods for each type of class.

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/generator.png" width="100%"></br>

## Life Cycles

It's recommended to build your entire project around these life cycle methods.

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/lifecycle.png" width="100%"></br>

## Classes, Methods and Properties

Use build in file generator to create new instances for any of these types.

```cs
/// Base class for every controller.
/// NOTE: This class allows the usage of [Injected] systems, services and controller.
public abstract class Controller {

  /// A reference to the controller.
  public static Controller Instance;

  /// Method invoked when the controller is initializing.
  public virtual void OnInitialize ();
  /// Method invoked when the controller is initialized.
  public virtual void OnInitialized ();
  /// Method invoked when the controller updates, will be called every frame.
  public virtual void OnUpdate ();
  /// Method invoked when the controller is drawing the gizmos, will be called
  /// every gizmos draw call.
  public virtual void OnDrawGui ();

  /// Register your systems and services to the controller. This can only be
  /// done during 'OnInitialize' cycle.
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
  /// Gets an asset from this controller.
  public AssetType GetAsset<AssetType> (string name);
  /// Check whether this controller has an asset.
  public bool HasAsset (string name);
}
```

```cs
/// Base class for every entity component.
/// NOTE: This class allows the usage of [Referenced] and [Protected] objects and primitives.
public abstract class EntityComponent<EntityComponentType, EntitySystemType> : UnityEngine.MonoBehaviour, IEntityComponent
  where EntityComponentType : EntityComponent<EntityComponentType, EntitySystemType>, new()
  where EntitySystemType : EntitySystem<EntitySystemType, EntityComponentType>, new() {

  /// Defines whether this component is enabled.
  public bool isEnabled { get; };
  /// Sets the game object of the entity active.
  public void SetActive (bool value);
  /// Destroys the game object of the entity.
  public void Destroy ();
  /// Adds an asset to the entity.
  public void AddAsset (UnityEngine.Object asset);
  /// Loads a asset from the controller and adds it as an asset to the entity.
  public void AddAsset (string name);

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
/// Base class for every entity system.
/// NOTE: This class allows the usage of [Injected] systems, services and controller.
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

  /// Method invoked when the system will initialize.
  public virtual void OnInitialize ();
  /// Method invoked when the system is initialized.
  public virtual void OnInitialized ();
  /// Method invoked when the physics update, will be called every fixed frame.
  /// NOTE: Define the ECS_PHYSICS scripting symbol to use this method.
  public virtual void OnPhysics ();
  /// Method invoked when the system is drawing the gizmos, will be called
  /// every gizmos draw call.
  public virtual void OnUpdate ();
  /// Method invoked when the camera renders, will be called every late frame.
  /// NOTE: Define the ECS_RENDER scripting symbol to use this method.
  public virtual void OnRender ();
  /// Method invoked when the system is drawing the gizmos, will be called
  /// every gizmos draw call.
  public virtual void OnDrawGizmos ();
  /// Method invoked when the system is drawing the gui, will be called every
  /// on gui draw call.
  public virtual void OnDrawGui ();
  /// Method invoked when the system becomes enabled.
  public virtual void OnEnabled ();
  /// Method invoked when the system becomes disabled.
  public virtual void OnDisabled ();
  /// Method invoked when an entity of this system is initializing.
  public virtual void OnEntityInitialize (EntityComponentType entity);
  /// Method invoked when an entity of this system is initialized.
  public virtual void OnEntityInitialized (EntityComponentType entity);
  /// Method invoked when an entity of this system becomes enabled.
  public virtual void OnEntityEnabled (EntityComponentType entity);
  /// Method invoked when an entity of this system becomes disabled.
  public virtual void OnEntityDisabled (EntityComponentType entity);
  /// Method invoked when an entity of this system will destroy.
  public virtual void OnEntityWillDestroy (EntityComponentType entity);
  /// Method invoked before the system will update, return whether this system
  /// should update. will be called every frame.
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
  /// Clones an entity on a given position in the hierarchy.
  public EntityComponentType CloneEntity (EntityComponentType entity, UnityEngine.Transform parentTransform);
  /// Finds entities using a predicate match.
  public EntityComponentType[] MatchEntities (System.Predicate<EntityComponentType> match);
  /// Finds an entity using a predicate match.
  public EntityComponentType[] MatchEntity (System.Predicate<EntityComponentType> match);
  /// Starts a coroutine on this system.
  public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine);
  /// Stops a given coroutine.
  public void StopCoroutine (System.Collections.IEnumerator routine);
  public void StopCoroutine (UnityEngine.Coroutine routine);
  /// Enables or disables this system.
  public void SetEnabled (bool isEnabled);
  /// Gets the enabled status of this system
  public bool GetEnabled ();
}
```

```cs
/// Base class for every service
/// NOTE: This class allows the usage of [Injected] systems, services and controller.
public abstract class Service<ServiceType> : IService
  where ServiceType : Service<ServiceType>, new() {

  /// An instance reference to the service.
  public static ServiceType Instance;

  /// Method invoked when the service will initialize.
  public virtual void OnInitialize ();
  /// Method invoked when the system is initialized.
  public virtual void OnInitialized ();
  /// Method invoked when the service is drawing the gizmos, will be called
  /// every gizmos draw call.
  public virtual void OnDrawGizmos ();
  /// Method invoked when the service is drawing the gui, will be called every
  /// on gui draw call.
  public virtual void OnDrawGui ();

  /// Starts a coroutine on this service.
  public UnityEngine.Coroutine StartCoroutine (System.Collections.IEnumerator routine);
  /// Stops a given coroutine.
  public void StopCoroutine (System.Collections.IEnumerator routine);
}
```
