#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UnityPackages.EntityComponentSystem {

  public static class Utils {

    public static void CreateFile (string title, string fileName, string afterSaveSuffix, string type, params string[] data) {
      var _path = EditorUtility.SaveFilePanel (title, "Assets/", fileName, type);
      if (_path.Length == 0)
        return;
      var _name = _path.Split ('/').Last ().Split ('.').First ();
      using (System.IO.StreamWriter outfile =
        new System.IO.StreamWriter (_path.Replace ("." + type, afterSaveSuffix + "." + type))) {
        var _data = string.Join ("\n", data).Replace ("{{name}}", _name);
        outfile.Write (_data);
      }
      AssetDatabase.Refresh ();
    }
  }
}

#endif