<div align="center">

<img src="https://raw.githubusercontent.com/elraccoone/unity-entity-component-system/master/.github/WIKI/logo-transparent.png" height="100px">

</br>

# Entity Component System

[![npm](https://img.shields.io/badge/upm-3.7.0-232c37.svg?style=for-the-badge)]()
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

## The basics

### Controllers

**Introduction** | The controller is the core of your application, it is of such importance that each application should only contain one of it. The controller is where your application will start from and all systems and services are registered. This can only be done during the OnInitialize method using the controller's Register method. To get started with your first controller you can use the generator, for the controller to work it should be assined to a game object in your scene.

```csharp
public class MainController : Controller {
  public override void OnInitialize () {
    this.Register (
      typeof (EnemyMovementSystem),
      typeof (AudioService)
    );
  }
}
```

The controller contains a series of overwriteable lifecyle methods. Consult the life cycles sheet for more information about when and in which order these methods will be invoked. All life cycle methods are optional and can be implemented all at once.

```csharp
public class MainController : Controller {
  public override void OnInitialize () { }
  public override void OnInitialized () { }
  public override void OnUpdate () { }
}
```

While it is recommended to move as much logic into [services](#Services) and [systems](#Systems), it is possible to let your controller house any functionality. If you use the controller for this purpose, try to keep it down to only application wide and core functionality. All public methods and properties are accessibly when having the controller [injected](#Injectables).

### Systems

### Components

### Services

### Injectables

### Assets
