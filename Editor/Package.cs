
using System;
using UnityEditor;

namespace Hananoki.SymbolSettings {
  public static class Package {
    public const string name = "SymbolSettings";
    public const string editorPrefName = "Hananoki.SymbolSettings";
    public const string version = "1.0.2";
    public static string projectSettingsPath => $"{Environment.CurrentDirectory}/ProjectSettings/SymbolSettings.json";
  }
  
#if UNITY_EDITOR
  [EditorLocalizeClass]
  public class LocalizeEvent {
    [EditorLocalizeMethod]
    public static void Changed() {
      foreach( var filename in DirectoryUtils.GetFiles( AssetDatabase.GUIDToAssetPath( "c3b2d643f230b594cabadfe0990156e2" ), "*.csv" ) ) {
        if( filename.Contains( EditorLocalize.GetLocalizeName() ) ) {
          EditorLocalize.Load( Package.name, AssetDatabase.AssetPathToGUID( filename ), "51260bdbda46e5e48abe3d33140b6609" );
        }
      }
    }
  }
#endif
}
