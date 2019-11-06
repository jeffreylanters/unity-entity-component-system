#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UnityPackages.EntityComponentSystem {

  public static class Menus {

    [MenuItem ("Assets/Create/Entity Component System/Controller")]
    private static void CreateNewController () {
      Utils.CreateFile (
        "Save new Controller",
        "Main",
        "Controller",
        "cs",
        "using UnityPackages.EntityComponentSystem;",
        "public class {{name}}Controller : ECS.Controller { }"
      );
    }

    [MenuItem ("Assets/Create/Entity Component System/Component")]
    private static void CreateNewComponent () {
      Utils.CreateFile (
        "Save new Component",
        "Item",
        "Component",
        "cs",
        "using UnityEngine;",
        "using UnityPackages.EntityComponentSystem;",
        "public class {{name}}Component : ECS.Component<{{name}}Component, {{name}}System> { }"
      );
    }

    [MenuItem ("Assets/Create/Entity Component System/System")]
    private static void CreateNewSystem () {
      Utils.CreateFile (
        "Save new System",
        "Item",
        "System",
        "cs",
        "using UnityPackages.EntityComponentSystem;",
        "public class {{name}}System : ECS.System<{{name}}System, {{name}}Component> { }"
      );
    }
  }
}

#endif