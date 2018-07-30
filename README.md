# Entity Component System

Offers a better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. It leverages the C# Job System and Burst Compiler enabling you to take full advantage of today's multicore processors. By moving from object-oriented to data-oriented design it will be easier for you to reuse the code and easier for others to understand and work on it.

## Install

```sh
$ git submodule add https://github.com/unity-packages/entity-component-system Assets/packages/entity-component-system
```

## Usage

### Controllers
Controllers are the main element of our ecs, there should only be one per project.

`public class Controller : UnityEngine.MonoBehaviour`
- Overrides
  - `public override void OnInitialize ()`
- Methods
  - `public void RegisterSystems (params ISystem[] systems)`
  - `public S GetSystem<S> () where S : ISystem, new ()`
- Variables
  - `public static Controller Instance`

**Example controller**
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

`public class System<S, C> : ISystem where S : System<S, C>, new () where C : Component<C, S>, new ()`
- Overrides
  - `public virtual void OnInitialize ()`
  - `public virtual void OnUpdate ()`
  - `public virtual void OnDrawGizmos ()`
  - `public virtual void OnEntityInitialize (C component)`
- Methods
  - `public void AddEntity (C component)`
  - `public void RemoveEntry (C component)`
- Variables
  - `public List<C> entities`
  - `public C firstEntity`
  
**Example system**
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

`public class Component<C, S> : UnityEngine.MonoBehaviour where C : Component<C, S>, new () where S : System<S, C>, new ()`
  
**Example component**
```cs
public class MovementComponent : ECS.Component<MovementComponent, MovementSystem> {
  public Vector3 speed;
  [ECS.Protected] public bool dontTouchMe;
}
```
