
using System;
using UnityEditor;

namespace Hananoki.SymbolSettings {
  public static class Package {
    public const string name = "SymbolSettings";
    public const string editorPrefName = "Hananoki.SymbolSettings";
    public const string version = "1.0.7";
    public static string projectSettingsPath => $"{SharedModule.SettingsEditor.projectSettingDirectory}/SymbolSettings.json";
  }
}
