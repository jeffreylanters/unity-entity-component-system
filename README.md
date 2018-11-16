# Entity Component System

![](https://img.shields.io/badge/dependencies-unity--packages-%233bc6d8.svg) ![](https://img.shields.io/badge/license-MIT-%23ecc531.svg)

This simple ECS offers a better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

> NOTE When using this Unity Package, make sure to **Star** this repository. When using any of the packages please make sure to give credits to **Jeffrey Lanters** somewhere in your app or game. **These packages are not allowed to be sold anywhere!**

## Install

```sh
$ git submodule add https://github.com/unity-packages/entity-component-system Assets/packages/entity-component-system
```

## Usage

### Controllers
Controllers are the main part of our ECS, there should only be one per project. The controller initialized the systems you want to be active.

```cs
public class MainController : ECS.Controller {

  // EVENT is triggered when the ECS is initialized.
  public override void OnInitialize () {
  
    // Start by registering all the systems.
    // Initialize the systems you want to use...
    this.RegisterSystems (    
      new MovementSystem (),
      new AttackSystem ()
    );
  }
  
  // EVENT is triggered when all the ECS are intialized.
  public override void OnInitialized () { }
  
  // EVENT is triggered every frame.
  public override void OnUpdate () { }
}
```

### Systems
The systems are controlled by the controller and will controll the component entities it owns.

```cs
// Movement System inherets the ECS System and uses itself and its component as generics.
public class MovementSystem : ECS.System<MovementSystem, MovementComponent> {

  // EVENT is triggered when the system is initialized.
  public override void OnInitialize () { }
      
  // EVENT is triggered when a new entity is intialized.
  public override void OnEntityInitialize (MovementComponent entity) {
    
    // Modify all the properties on the component entity.
    entity.dontTouchMe = true;
    
    // Get access to another component on an entity.
    this.GetComponentOnEntity<AttackComponent> (entity, attackComponent => {
      attackComponent.strenght = 10;
    });
  }
  
  // EVENT is triggered when an entity will be destroyed.
  public override void OnEntityWillDestroy (MovementComponent entity) { }

  // EVENT is triggered every frame.
  public override void OnUpdate () {
    var _translation = new Vector3(1, 0, 0) * Time.deltaTime;
    
    // Loop all the entities to do something with them...
    foreach (var _entity in this.entities) {
    
      // Modify all the properties on the component entity.
      _entity.transform.position += _translation * _entity.speed;
    }
  }
  
  // EVENT is triggered every gizmos draw call.
  public override void OnDrawGizmos () { }
}
```

### Components
The components are controlled by the systems and may only contain public data.

```cs
// Movement component inherets the ECS Component and uses its system and itself as generics.
public class MovementComponent : ECS.Component<MovementComponent, MovementSystem> {

  // Only add public variables to your component.
  public Vector3 speed;
  
  // Use Protected to lock variables in the editor.
  [ECS.Protected] public bool dontTouchMe;
  
  // Use Reference to mark variables as refereces.
  [ECS.Reference] public Rigidbody rigidbody;
}
```
 
