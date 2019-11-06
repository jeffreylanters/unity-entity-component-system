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
        "",
        "public class {{name}}Controller : ECS.Controller {",
        "\tpublic override void OnInitialize () {",
        "\t\tthis.RegisterSystems (/* typeof(MyItemSystem), ... */);",
        "\t\tthis.EnableSystems(/* typeof(MyItemSystem), ... */);",
        "\t\tthis.DisableSystems(/* typeof(MyItemSystem), ... */);",
        "\t}",
        "",
        "\tpublic override void OnInitialized () { }",
        "",
        "\tpublic override void OnUpdate () { }",
        "}"
      );
    }

    [MenuItem ("Assets/Create/Entity Component System/Component")]
    private static void CreateNewComponent () {
      Utils.CreateFile (
        "Save new Component",
        "Item",
        "Component",
        "cs",
        "using UnityPackages.EntityComponentSystem;",
        "using UnityEngine;",
        "",
        "public class {{name}}Component : ECS.Component<{{name}}Component, {{name}}System> {",
        "\t// [ECS.Protected] public bool myProtectedBool;",
        "\t// [ECS.Reference] public Transform myReferencesTransform;",
        "}"
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
        "",
        "public class {{name}}System : ECS.System<{{name}}System, {{name}}Component> {",
        "\tpublic override void OnInitialize () { }",
        "",
        "\tpublic override void OnEntityInitialize ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnEntityStart ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnEntityEnabled ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnEntityInitialized ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnUpdate () {",
        "\t\t// this.firstEntity;",
        "\t\tforeach (var _entity in this.entities) { }",
        "\t}",
        "",
        "\tpublic override void OnEnabled () { }",
        "",
        "\tpublic override void OnInitialized () { }",
        "",
        "\tpublic override void OnEntityDisabled ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnEntityWillDestroy ({{name}}Component entity) { }",
        "",
        "\tpublic override void OnDrawGizmos () { }",
        "",
        "\tpublic override void OnGUI () { }",
        "",
        "\tpublic override void OnDisabled () { }",
        "}"
      );
    }
  }
}

#endif