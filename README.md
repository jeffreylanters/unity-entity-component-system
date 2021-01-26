<div align="center">

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/logo-transparent.png" height="100px">

</br>

# Entity Component System

[![openupm](https://img.shields.io/npm/v/nl.elraccoone.unity-entity-component-system?label=UPM&registry_uri=https://package.openupm.com&style=for-the-badge&color=232c37)](https://openupm.com/packages/nl.elraccoone.unity-entity-component-system/)
[![](https://img.shields.io/github/stars/elraccoone/unity-entity-component-system.svg?style=for-the-badge)]()
[![](https://img.shields.io/badge/build-passing-brightgreen.svg?style=for-the-badge)]()

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

## What's in the box?

### Controllers

**Introduction:** The [Controller](#controllers) is the heart of your Application, each Application should consist of just one, commonly named the MainController. The [Controller](#controllers) is the first entry point of the Entity Component System and is the place where all of your [Systems](#systems) and [Services](#services) are registered. Your [Controller](#controllers) should be attached to a Game Object in your scene and will be marked to not be destroyed when switching scenes.

```csharp
public class MainController : Controller { }
```

**Virtual On Initialize:** The [Controller](#controllers) consists of an OnInitialize virtual method. This method can be overwritten and will be invoked during the very start of your Application. During this cycle properties with the Injected and Asset attribute are being assigned, it is important to invoke the Register method during this cycle since this is the only time in your Application you can register [Systems](#systems) and [Services](#services).

```csharp
public class MainController : Controller {
  public override void OnInitialize () {
    this.Register (
      typeof (MovementSystem),
      typeof (AudioService)
    );
  }
}
```

**Virtual On Initialized:** The [Controller](#controllers) consists of an OnInitialized virtual method. This method can be overwritten and will be invoked when all [Systems](#systems) and [Services](#services) did initialize, and all the properties with the Injected and Asset attributes are assigned.

```csharp
public class MainController : Controller {
  public override void OnInitialized () { }
}
```

**Virtual On Update:** The [Controller](#controllers) consists of an OnUpdate virtual method. This method can be overwritten and will be invoked during the Update cycle. This cycle will run once every frame, the Controller's Update is invoked before the [System's](#systems) and [Service's](#services) update cycles.

```csharp
public class MainController : Controller {
  public override void OnUpdate () { }
}
```

**Enabling Systems:** To enable or disable [Systems](#systems), the [Controller](#controllers) contains of a method EnableSystem which allows [Systems](#systems) to stop their life cycle methods such as OnUpdate, OnPhysics, OnDrawGui and others. You can provide the [System's](#system) type using a generic. Systems are enabled by default.

```csharp
public class MainController : Controller {
  public void SomeMethod () {
    this.SetSystemEnabled<MovementSystem> (true);
    this.SetSystemEnabled<InteractableSystem> (false);
  }
}
```

**Checking Whether Systems Are Enabled:** To check whether [Systems](#systems) are enable or disabled, the [Controller](#controllers) contains of a method IsSystemEnabled. Invoking the method will return a boolean informing if the [System](#systems) is enabled or not. You can provide the [System's](#system) type using a generic.

```csharp
public class MainController : Controller {
  public void SomeMethod () {
    if (this.IsSystemEnabled<MovementSystem> ()) { }
  }
}
```

**Injection:** The [Controller](#controllers) allows the use of the Injected attribute on properties to automatically assign the values of referenced [Systems](#Systems) and [Services](#Services), making all public methods and properties accessible. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle.

```csharp
public class MainController : Controller {
  [Injected] private MovementSystem movementSystem;
  [Injected] private AudioService audioService;
}
```

**Assets:** The [Controller](#controllers) allows the use of the Asset attribute on properties to automatically assign the values of referenced Assets. Assets can be assigned on the [Controller](#controllers) instance in your Scene. When assigning using the empty contructor, the property's name will be used for searching the Asset, to find an Asset by it's name, use the string overload. All types of UnityEngine's Object can be used in these fields. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle. When an asset is not found, an error is thrown.

```csharp
public class MainController : Controller {
  [Asset] private GameObject playerPrefab;
  [Asset ("ShopDialog")] private NpcDialog npcDialog;
}
```

**Notes:** While it is recommended to move as much logic into [Services](#services) and [Systems](#systems), it is possible to let your [Controller](#controllers) house any functionality. If you use the [Controller](#controllers) for this purpose, try to keep it down to only Application wide and core functionality.

### Components

**Introduction:** [Components](#components) are responsible for housing the data of your entities, and should consist of nothing more than that. All properties should be public and will be accessible to all [Systems](#systems) and [Controllers](#controllers) since there is no need for privates. [Components](#components) should be added to your Entities (GameObjects) in the Scene, an Entity is not limited to one [Components](#components) and can hold as many as needed.

```csharp
public class MovementComponent : Component<MovementComponent, MovementSystem> { }
```

**Public Properties:** Public properties are the heart of your [Components](#components), and are here to provide data for the [Systems](#systems) to use. Properties can be added to [Components](#components) like in any other class and can consist of any kind of type.

```csharp
public class MovementComponent : Component<MovementComponent, MovementSystem> {
  public float speed;
  public Vector3 targetPosition;
  public int[] ids;
  public NpcDialog dialog;
}
```

**Editor Protection:** Sometimes you want to hide properties from the Unity Editor when they are, for example are managed by the [Systems](#systems). By flagging these properties with the Protected attribute, it will no longer shows up in the Unity Editor, but is still accessible by the [Systems](#systems).

```csharp
public class MovementComponent : Component<MovementComponent, MovementSystem> {
  [Protected] public float currentSpeed;
}
```

<!-- **Editor Reference:**

```csharp
public class MovementComponent : Component<MovementComponent, MovementSystem> {
  [Referenced] public BoxCollider playerCollider;
}
``` -->

_The components section of the documentation is in process!_

### Systems

**Introduction:** The [Systems](#systems) are responsible for controlling all of your Entity's [Components](#components) and are the closest you'll get of what you're used to when working with MonoBehaviours. The entire life cycles of your Entities are managed in here.

```csharp
public class MovementSystem : System<MovementSystem, MovementComponent> { }
```

**Injection:** The [System](#systems) allows the use of the Injected attribute on properties to automatically assign the values of referenced [Systems](#Systems), [Services](#Services) and [Controllers](#controllers), making all public methods and properties accessible. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle.

```csharp
public class MovementSystem : System<MovementSystem, MovementComponent> {
  [Injected] private MainController mainController;
  [Injected] private HealthSystem healthSystem;
  [Injected] private AudioService audioService;
}
```

**Assets:** The [System](#system) allows the use of the Asset attribute on properties to automatically assign the values of referenced Assets. Assets can be assigned on the [Controller](#controllers) instance in your Scene. When assigning using the empty contructor, the property's name will be used for searching the Asset, to find an Asset by it's name, use the string overload. All types of UnityEngine's Object can be used in these fields. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle. When an asset is not found, an error is thrown.

```csharp
public class MovementSystem : System<MovementSystem, MovementComponent> {
  [Asset] private GameObject playerPrefab;
  [Asset ("ShopDialog")] private NpcDialog npcDialog;
}
```

_The systems section of the documentation is in process!_

### Services

**Introduction:**

```csharp
public class AudioService : Service<AudioService> { }
```

**Injection:** The [Service](#services) allows the use of the Injected attribute on properties to automatically assign the values of referenced [Systems](#Systems), [Services](#Services) and [Controllers](#controllers), making all public methods and properties accessible. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle.

```csharp
public class AudioService : Service<AudioService> {
  [Injected] private MainController mainController;
  [Injected] private MovementSystem movementSystem;
  [Injected] private NetworkService networkService;
}
```

**Assets:** The [Service](#services) allows the use of the Asset attribute on properties to automatically assign the values of referenced Assets. Assets can be assigned on the [Controller](#controllers) instance in your Scene. When assigning using the empty contructor, the property's name will be used for searching the Asset, to find an Asset by it's name, use the string overload. All types of UnityEngine's Object can be used in these fields. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle. When an asset is not found, an error is thrown.

```csharp
public class AudioService : Service<AudioService> {
  [Asset] private GameObject playerPrefab;
  [Asset ("ShopDialog")] private NpcDialog npcDialog;
}
```

_The services section of the documentation is in process!_
