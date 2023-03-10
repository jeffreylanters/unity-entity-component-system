#if UNITY_EDITOR && ECS_DEFINED_COM_UNITY_UGUI
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElRaccoone.EntityComponentSystem.Editor {
  public class GenerateFileEditorWindow : EditorWindow {
    private static GenerateFileType generateFileType;
    private string fileName;
    private bool shouldOverwriteAllVirtuals;
    private bool shouldImportCommonNamespaces;
    private bool shouldAddFileHeaderComments;

    private enum GenerateFileType {
      Controller,
      ComponentAndSystem,
      Service,
      ScriptableObject,
      EmptyClass
    }

    [MenuItem ("ECS/Generate Controller", false, 100)]
    private static void GenerateNewController () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.Controller;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Controller");
    }

    [MenuItem ("ECS/Generate Component and System", false, 100)]
    private static void GenerateNewComponentAndSystem () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.ComponentAndSystem;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Component and System");
    }

    [MenuItem ("ECS/Generate Service", false, 100)]
    private static void GenerateNewService () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.Service;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Service");
    }

    [MenuItem ("ECS/Generate Scriptable Object", false, 100)]
    private static void GenerateNewScriptableObject () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.ScriptableObject;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Scriptable Object");
    }

    [MenuItem ("ECS/Generate Empty Class", false, 100)]
    private static void GenerateNewClass () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.EmptyClass;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Class");
    }

    private void OnGUI () {
      var _newFileTypeReadable = "";
      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Controller)
        _newFileTypeReadable = "Controller";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem)
        _newFileTypeReadable = "Component and System";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Service)
        _newFileTypeReadable = "Service";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ScriptableObject)
        _newFileTypeReadable = "Scriptable Object";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.EmptyClass)
        _newFileTypeReadable = "Empty Class";
      GUILayout.BeginHorizontal ();
      GUILayout.Space (20);
      GUILayout.BeginVertical ();
      GUILayout.Space (20);
      GUILayout.Label ("Name your new " + _newFileTypeReadable + "...", EditorStyles.largeLabel);
      this.fileName = GUILayout.TextField (this.fileName);
      GUILayout.FlexibleSpace ();
      this.shouldImportCommonNamespaces = GUILayout.Toggle (this.shouldImportCommonNamespaces, " Import Common Namespaces");
      this.shouldOverwriteAllVirtuals = GUILayout.Toggle (this.shouldOverwriteAllVirtuals, " Overwrite All Virtuals");
      this.shouldAddFileHeaderComments = GUILayout.Toggle (this.shouldAddFileHeaderComments, " Add File Header Comments");
      GUILayout.FlexibleSpace ();
      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Controller) {
        if (GUILayout.Button ("Generate " + this.fileName + "Controller"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem) {
        if (GUILayout.Button ("Generate " + this.fileName + "Component and -System"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Service) {
        if (GUILayout.Button ("Generate " + this.fileName + "Service"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ScriptableObject) {
        if (GUILayout.Button ("Generate " + this.fileName + "ScriptableObject"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.EmptyClass) {
        if (GUILayout.Button ("Generate " + this.fileName))
          this.Generate ();
      }
      GUILayout.Space (20);
      GUILayout.EndVertical ();
      GUILayout.Space (20);
      GUILayout.EndHorizontal ();
    }

    private void Generate () {
      var _dateTimeStamp = System.DateTime.Now.ToString ("dddd, dd MMMM yyyy HH:mm:ss");
      var _overwriteAllVirtuals = this.shouldOverwriteAllVirtuals;
      var _importCommonNamespaces = this.shouldImportCommonNamespaces;
      var _addFileHeaderComments = this.shouldAddFileHeaderComments;
      var _fileName = this.fileName;
      this.fileName = "";

      switch (GenerateFileEditorWindow.generateFileType) {
        case GenerateFileType.Controller:
          this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Controllers"), _fileName + "Controller", "cs", new string[] {
            "using ElRaccoone.EntityComponentSystem;",
            _importCommonNamespaces ? "using UnityEngine;" : null,
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            "",
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// Controller for " + _fileName + "." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "public class " + _fileName + "Controller : Controller {",
            _addFileHeaderComments ? "\t/// <summary>Method invoked when the Controller is initializing.</summary>" : null,
            "\tpublic override void OnInitialize () {",
            "\t\tthis.Register();",
            "\t}",
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the Controller is initialized.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Update cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnUpdate () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the Controller will destroy.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnWillDestroy () { }" : null,
            "}"
          });
          break;

        case GenerateFileType.ComponentAndSystem:
          this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Components"), _fileName + "Component", "cs", new string[] {
            "using ElRaccoone.EntityComponentSystem;",
            _importCommonNamespaces ? "using UnityEngine;" : null,
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            "",
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// Entity Component for " + _fileName + "." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "public class " + _fileName + "Component : EntityComponent<" + _fileName + "Component, " + _fileName + "System> { }"
          });
          this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Systems"), _fileName + "System", "cs", new string[] {
            "using ElRaccoone.EntityComponentSystem;",
            _importCommonNamespaces ? "using UnityEngine;" : null,
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            "",
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// Entity System for " + _fileName + "." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "public class " + _fileName + "System : EntitySystem<" + _fileName + "System, " + _fileName + "Component> {",
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the System is initializing.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnInitialize () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the System is initialized.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
            _overwriteAllVirtuals ? "" : null,
#if ECS_PHYSICS
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Fixed Update cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnPhysics () { }" : null,
            _overwriteAllVirtuals ? "" : null,
#endif
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Update cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnUpdate () { }" : null,
            _overwriteAllVirtuals ? "" : null,
#if ECS_GRAPHICS
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Late Update cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnRender () { }" : null,
            _overwriteAllVirtuals ? "" : null,
#endif
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Gizmos Render cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnDrawGizmos () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's GUI Render cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnDrawGui () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the System is enabled.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEnabled () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the System is disabled.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnDisabled () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the System will be destroyed.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnWillDestroy () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when an Entity is initializing.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <param name=\"entity\">The initializing Entity.</param>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEntityInitialize (" + _fileName + "Component entity) { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when an Entity is initialized.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <param name=\"entity\">The initialized Entity.</param>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEntityInitialized (" + _fileName + "Component entity) { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when an Entity is enabled.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <param name=\"entity\">The enabled Entity.</param>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEntityEnabled (" + _fileName + "Component entity) { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when an Entity is disabled.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <param name=\"entity\">The disabled Entity.</param>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEntityDisabled (" + _fileName + "Component entity) { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when an Entity will be destroyed.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <param name=\"entity\">The to be destroyed Entity.</param>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnEntityWillDestroy (" + _fileName + "Component entity) { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method incidates whether the System should Update.</summary>" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <returns>Whether the Controller should update this system.</returns>" : null,
            _overwriteAllVirtuals ? "\tpublic override bool ShouldUpdate () {" : null,
            _overwriteAllVirtuals ? "\t\treturn true;" : null,
            _overwriteAllVirtuals ? "\t}" : null,
            "}"
          });
          break;

        case GenerateFileType.Service:
          this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Services"), _fileName + "Service", "cs", new string[] {
            "using ElRaccoone.EntityComponentSystem;",
            _importCommonNamespaces ? "using UnityEngine;" : null,
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            "",
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// Service for " + _fileName + "." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "public class " + _fileName + "Service : Service<" + _fileName + "Service> {",
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the Service is initializing.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnInitialize () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the Service is initialized.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's Gizmos Render cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnDrawGizmos () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked during the Controller's GUI Render cycle.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnDrawGui () { }" : null,
            _overwriteAllVirtuals ? "" : null,
            _overwriteAllVirtuals && _addFileHeaderComments ? "\t/// <summary>Method invoked when the Service will be destoryed.</summary>" : null,
            _overwriteAllVirtuals ? "\tpublic override void OnWillDestroy () { }" : null,
            "}"
          });
          break;

        case GenerateFileType.ScriptableObject:
          this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "ScriptableObjects"), _fileName + "ScriptableObject", "cs", new string[] {
            "using ElRaccoone.EntityComponentSystem;",
            "using UnityEngine;",
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            "",
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// " + _fileName + " Scriptable Object." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "[CreateAssetMenu (fileName = \"" + _fileName + "ScriptableObject\", menuName = \"Scriptable Objects/" + _fileName + "\", order = 1)]",
            "public class " + _fileName + "ScriptableObject : ScriptableObject {",
            "}"
          });
          break;

        case GenerateFileType.EmptyClass:
          this.WriteContentToFile (Application.dataPath, _fileName, "cs", new string[] {
            _importCommonNamespaces ? "using UnityEngine;" : null,
            _importCommonNamespaces ? "using System.Collections.Generic;" : null,
            _importCommonNamespaces ? "" : null,
            _addFileHeaderComments ? "/// <summary>" : null,
            _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
            _addFileHeaderComments ? "/// Author: " : null,
            _addFileHeaderComments ? "/// Created: " + _dateTimeStamp : null,
            _addFileHeaderComments ? "/// " + _fileName + "." : null,
            _addFileHeaderComments ? "/// </summary>" : null,
            "public class " + _fileName + " {",
            "}"
          });
          break;
      }

      this.Close ();
    }

    private string FindDirectoryWithName (string directory, string name) {
      var _directories = this.WalkDirectory (directory);
      foreach (var _directory in _directories)
        if (_directory.Split ('/').Last ().ToLower () == name.ToLower ())
          return _directory;
      Debug.LogWarning ("There is no directory named '" + name + "', creating in project root.");
      return directory;
    }

    private List<string> WalkDirectory (string directory) {
      var _results = new List<string> ();
      var _directories = System.IO.Directory.GetDirectories (directory);
      foreach (var _directory in _directories) {
        _results.Add (_directory);
        var _directoryDirectories = this.WalkDirectory (_directory);
        foreach (var _directoryDirectory in _directoryDirectories)
          _results.Add (_directoryDirectory);
      }
      return _results;
    }

    private void WriteContentToFile (string filePath, string fileName, string fileType, string[] fileContentLines) {
      using (var outfile = new StreamWriter (filePath + "/" + fileName + "." + fileType)) {
        var _fileContent = "";
        foreach (var _fileContentLine in fileContentLines)
          if (_fileContentLine != null)
            _fileContent += _fileContentLine + "\n";
        outfile.Write (_fileContent);
      }
      Debug.Log ("Creating '" + fileName + "' in '" + filePath + "'.");
      AssetDatabase.Refresh ();
    }
  }
}
#endif
