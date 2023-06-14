<div align="center">

![readme splash](https://raw.githubusercontent.com/jeffreylanters/unity-entity-component-system/master/.github/WIKI/repository-readme-splash.png)

[![license](https://img.shields.io/badge/mit-license-red.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-entity-component-system/blob/master/LICENSE.md)
[![openupm](https://img.shields.io/npm/v/nl.elraccoone.entity-component-system?label=UPM&registry_uri=https://package.openupm.com&style=for-the-badge&color=232c37)](https://openupm.com/packages/nl.elraccoone.entity-component-system/)
[![build](https://img.shields.io/badge/build-passing-brightgreen.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-entity-component-system/actions)
[![deployment](https://img.shields.io/badge/state-success-brightgreen.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-entity-component-system/deployments)
[![stars](https://img.shields.io/github/stars/jeffreylanters/unity-entity-component-system.svg?style=for-the-badge&color=fe8523&label=stargazers)](https://github.com/jeffreylanters/unity-entity-component-system/stargazers)
[![awesome](https://img.shields.io/badge/listed-awesome-fc60a8.svg?style=for-the-badge)](https://github.com/jeffreylanters/awesome-unity-packages)
[![size](https://img.shields.io/github/languages/code-size/jeffreylanters/unity-entity-component-system?style=for-the-badge)](https://github.com/jeffreylanters/unity-entity-component-system/blob/master/Runtime)
[![sponsors](https://img.shields.io/github/sponsors/jeffreylanters?color=E12C9A&style=for-the-badge)](https://github.com/sponsors/jeffreylanters)
[![donate](https://img.shields.io/badge/donate-paypal-F23150?style=for-the-badge)](https://paypal.me/jeffreylanters)

A better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

[**Installation**](#installation) &middot;
[**Documentation**](#documentation) &middot;
[**License**](./LICENSE.md)

**Made with &hearts; by Jeffrey Lanters**

</div>

# Installation

### Using the Unity Package Manager

Install the latest stable release using the Unity Package Manager by adding the following line to your `manifest.json` file located within your project's Packages directory, or by adding the Git URL to the Package Manager Window inside of Unity.

```json
"nl.elraccoone.entity-component-system": "git+https://github.com/jeffreylanters/unity-entity-component-system"
```

### Using OpenUPM

The module is availble on the OpenUPM package registry, you can install the latest stable release using the OpenUPM Package manager's Command Line Tool using the following command.

```sh
openupm add nl.elraccoone.entity-component-system
```

# Documentation

## Getting Started

It's recommended to get started by using the built-in File Generator. When it's your first time using the ECS, you might want to enable the _Overwrite All Virtuals_ option to see all the available methods for each type of class.

<img src="https://raw.githubusercontent.com/jeffreylanters/unity-entity-component-system/master/.github/WIKI/generator.png" width="100%"></br>

## Life Cycles

It's recommended to build your entire project around these life cycle methods.

<img src="https://raw.githubusercontent.com/jeffreylanters/unity-entity-component-system/master/.github/WIKI/lifecycle.png" width="100%"></br>

## Controllers

### Introduction

The `Controller` is the heart of your Application, each Application should consist of just one, commonly named the MainController. The `Controller` is the first entry point of the Entity Component System and is the place where all of your `System` and `Services` are registered. Your `Controller` should be attached to a `GameObject` in your scene and will automatically be marked to not be destroyed when switching scenes.

```csharp
public class MainController: Controller { }
```

### Life Cycle Methods

#### On Initialize Life Cycle Method

The `Controller` consists of a virtual `OnInitialize` method. This lifecycle method can be overwritten and will be invoked at the very start of your Application. During this cycle, properties with the `Injected` and `Asset` attributes are being assigned, and the `OnInitialize` method of each registered `System` and `Service` will be invoked as well.

```csharp
public class MainController: Controller {
  public override void OnInitialize() { }
}
```

#### On Initialized Life Cycle Method

The `Controller` consists of a virtual `OnInitialized` method. This lifecycle method can be overwritten and will be invoked after the `OnInitialize` method has been invoked. During this cycle, the `OnInitialized` method of each registered `System` and `Service` will be invoked as well.

```csharp
public class MainController: Controller {
  public override void OnInitialized() { }
}
```

#### On Update Life Cycle Method

The `Controller` consists of a virtual `OnUpdate` method. This lifecycle method can be overwritten and will be invoked every frame. During this cycle, the `OnUpdate` method of each registered `System` will be invoked as well.

```csharp
public class MainController: Controller {
  public override void OnUpdate() { }
}
```

#### On Will Destroy Life Cycle Method

The `Controller` consists of a virtual `OnWillDestroy` method. This lifecycle method can be overwritten and will be invoked when the Application is about to quit. During this cycle, the `OnWillDestroy` method of each registered `System` and `Service` will be invoked as well.

```csharp
public class MainController: Controller {
  public override void OnWillDestroy() { }
}
```

### Methods

#### Registering Systems and Services

Use this method to `Register` the Systems and Services that are required for your Application to function. The `Register` method accepts a list of `Type` arguments, each of these types should be a `System` or `Service` type.

Registering a `System` or `Service` can only be done once during the `Controller`'s `OnInitialize` life cycle method.

```csharp
public class MainController: Controller {
  public override void OnInitialize() {
    Register(
      typeof(ExampleSystem),
      typeof(ExampleService)
    );
  }
}
```

#### Disabling a System's Update Life Cycles

To enable or disable the life cycle of a `System` or `Service`, use the `SetSystemEnabled` methods. This method accepts a `Type` generic, and a `bool` value to enable or disable the life cycle of the `System` or `Service`.

```csharp
public class MainController: Controller {
  void SomeMethod() {
    SetSystemEnabled<ExampleSystem>(false);
  }
}
```

#### Checking whether a System's Update Life Cycles are Enabled

To check whether the life cycle of a `System` or `Service` is enabled, use the `IsSystemEnabled` methods. This method accepts a `Type` generic, and returns a `bool` value indicating whether the life cycle of the `System` or `Service` is enabled.

```csharp
public class MainController: Controller {
  void SomeMethod() {
    if (IsSystemEnabled<ExampleSystem>()) { }
  }
}
```

#### Manually getting a reference to a System or Service

Something you might want to get a reference to a `System` or `Service` from within something outside of the Entity Component System. To do this, use the `GetSystem`, `GetService`, `HasSystem` and `HasService` methods respectively. These methods accept a `Type` generic, or a `Type` parameter and return the `System` or `Service` instance.

```csharp
public class MainController: Controller {
  void SomeMethod() {
    if (HasSystem<ExampleSystem>()) {
      var exampleSystem = GetSystem<ExampleSystem>();
      var exampleSystem = GetSystem(typeof(ExampleSystem));
    }
    if (HasService<ExampleService>()) {
      var exampleService = GetService<ExampleService>();
      var exampleService = GetService(typeof(ExampleService));
    }
  }
}
```

#### Manually getting Assets

Something you might want to get a reference to an `Asset` from within something outside of the Entity Component System. To do this, use the `GetAsset` and `HasAsset` methods respectively. These methods accepts an optional `Type` generic and a `name` parameter and returns the `Type` or `Object`

```csharp
public class MainController: Controller {
  void SomeMethod() {
    if (HasAsset<ExampleAsset>()) {
      var exampleAsset = GetAsset<ExampleAsset>("MyAssetName");
      var exampleAssetObject = GetAsset("MyAssetName");
    }
  }
}
```

### Attributes

#### Assets

The `Controller` allows the use of the `Asset` attribute on properties to automatically assign the values of referenced Assets. Assets can be assigned on the `Controller` instance in your Scene. When assigning using the empty contructor, the property's name will be used for searching the Asset, to find an Asset by its name, use the string overload. All types of UnityEngine's Object can be used in these fields. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle. When an asset is not found, an error is thrown.

```csharp
public class MainController: Controller {
  [Asset] public ExampleAsset exampleAsset;
  [Asset("MyAssetName")] public ExampleAsset exampleAsset;
}
```

#### Injection

The `Controller` allows the use of the `Injected` attribute on properties to automatically assign the values of referenced Systems and Services, making all public methods and properties accessible. These properties are assigned during the OnInitialize cycle and are available for use at the OnInitialized cycle.

```csharp
public class MainController: Controller {
  [Injected] public ExampleSystem exampleSystem;
  [Injected] public ExampleService exampleService;
}
```

## Components

### Introduction

`Components` are responsible for storing the data of your `entities` while `Systems` are responsible for manipulating that data. `Components` are added to your `entities` (`GameObjects`) in the `Scene`, an `Entity` is not limited to one `Component` and can hold as many as needed.

```csharp
public class MovementComponent : EntityComponent<MovementComponent, MovementSystem> { }
```

#### Entity Data

To provide `Systems` entity data, we'll use properties to store this. All properties should be public and will be accessible to all `Systems` and `Controllers` since there is no need for privates.

```csharp
public class MovementComponent : EntityComponent<MovementComponent, MovementSystem> {
  public float speed;
  public Vector3 targetPosition;
  public int[] ids;
  public NpcDialog dialog;
  [HideInInspector] public bool isMoving;
}
```
