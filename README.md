# Entity Component System

Offers a better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

## Install

```sh
$ git submodule add https://github.com/unity-packages/entity-component-system Assets/packages/entity-component-system
```

## Usage

### Controllers
Controllers are the main element of our ecs, there should only be one per project.
```cs
public class MainController : ECS.Controller {
  public override void OnInitialize () {
    this.RegisterSystems (
      new MovementSystem ()
    );
  }
}
```

### Systems
The systems are controlled by the controller and will controll the component entities it owns.

```cs
public class MovementSystem : ECS.System<MovementSystem, MovementComponent> {
  public override void OnUpdate () {
    var _translation = new Vector3(1, 0, 0) * Time.deltaTime;
    foreach (var _entity in this.entities) {
      _entity.transform.position += _translation * _entity.speed;
    }
  }
}
```

### Components
The components are controlled by the systems and may only contain public data.

```cs
public class MovementComponent : ECS.Component<MovementComponent, MovementSystem> {
  public Vector3 speed;
  [ECS.Protected] public bool dontTouchMe;
}
```
