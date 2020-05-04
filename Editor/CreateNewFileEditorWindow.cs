#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElRaccoone.EntityComponentSystem.Editor {
  public enum NewFileType {
    Controller,
    ComponentAndSystem,
    Service
  }

  public class CreateNewFileEditorWindow : EditorWindow {
    private static NewFileType newFileType;
    private string fileName;

    [MenuItem ("ECS/Create New Controller", false, 100)]
    private static void CreateNewController () {
      CreateNewFileEditorWindow.newFileType = NewFileType.Controller;
      EditorWindow.GetWindowWithRect (typeof (CreateNewFileEditorWindow), new Rect (0, 0, 400, 150), true, "Create New File");
    }

    [MenuItem ("ECS/Create New Component and System", false, 100)]
    private static void CreateNewComponentAndSystem () {
      CreateNewFileEditorWindow.newFileType = NewFileType.ComponentAndSystem;
      EditorWindow.GetWindowWithRect (typeof (CreateNewFileEditorWindow), new Rect (0, 0, 400, 150), true, "Create New File");
    }

    [MenuItem ("ECS/Create New Service", false, 100)]
    private static void CreateNewService () {
      CreateNewFileEditorWindow.newFileType = NewFileType.Service;
      EditorWindow.GetWindowWithRect (typeof (CreateNewFileEditorWindow), new Rect (0, 0, 400, 150), true, "Create New File");
    }

    private void OnGUI () {
      var _newFileTypeReadable = "";
      if (CreateNewFileEditorWindow.newFileType == NewFileType.Controller)
        _newFileTypeReadable = "Controller";
      else if (CreateNewFileEditorWindow.newFileType == NewFileType.ComponentAndSystem)
        _newFileTypeReadable = "Component and System";
      else if (CreateNewFileEditorWindow.newFileType == NewFileType.Service)
        _newFileTypeReadable = "Service";
      GUILayout.BeginHorizontal (); {
        GUILayout.Space (20);
        GUILayout.BeginVertical (); {
          GUILayout.Space (20);
          GUILayout.Label ("Name your new " + _newFileTypeReadable + "...", EditorStyles.largeLabel);
          this.fileName = GUILayout.TextField (this.fileName);
          GUILayout.FlexibleSpace ();
          if (GUILayout.Button ("Create"))
            this.Create ();
          GUILayout.Space (20);
        }
        GUILayout.EndVertical ();
        GUILayout.Space (20);
      }
      GUILayout.EndHorizontal ();
    }

    private void Create () {
      var _fileName = this.fileName;
      this.fileName = "";

      if (CreateNewFileEditorWindow.newFileType == NewFileType.Controller)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Controllers"), _fileName + "Controller", "cs",
          "using ElRaccoone.EntityComponentSystem;\n",
          "public class " + _fileName + "Controller  : Controller {",
          "\tpublic override void OnInitialize () {",
          "\t\tthis.Register();",
          "\t}",
          "}"
        );

      if (CreateNewFileEditorWindow.newFileType == NewFileType.ComponentAndSystem) {
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Components"), _fileName + "Component", "cs",
          "using ElRaccoone.EntityComponentSystem;\n",
          "public class " + _fileName + "Component : EntityComponent<" + _fileName + "Component, " + _fileName + "System> { }"
        );
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Systems"), _fileName + "System", "cs",
          "using ElRaccoone.EntityComponentSystem;\n",
          "public class " + _fileName + "System : EntitySystem<" + _fileName + "System, " + _fileName + "Component> { }"
        );
      }

      if (CreateNewFileEditorWindow.newFileType == NewFileType.Service)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Services"), _fileName + "Service", "cs",
          "using ElRaccoone.EntityComponentSystem;\n",
          "public class " + _fileName + "Service : Service<" + _fileName + "Service> { }"
        );

      this.Close ();
      AssetDatabase.Refresh ();
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

    private void WriteContentToFile (string filePath, string fileName, string fileType, params string[] fileContentLines) {
      using (var outfile = new StreamWriter (filePath + "/" + fileName + "." + fileType)) {
        var _fileContent = "";
        foreach (var _fileContentLine in fileContentLines)
          _fileContent += _fileContentLine + "\n";
        outfile.Write (_fileContent);
      }
      Debug.Log("Creating '" + fileName + "' in '" + filePath + "'.");
    }
  }
}
#endif
