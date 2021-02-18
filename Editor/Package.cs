
using System;
using UnityEditor;

namespace HananokiEditor.SymbolSettings {
  public static class Package {
    public const string name = "SymbolSettings";
    public const string nameNicify = "Symbol Settings";
    public const string editorPrefName = "Hananoki.SymbolSettings";
    public const string version = "2.0.0";
    public static string projectSettingsPath => $"{SharedModule.SettingsEditor.projectSettingDirectory}/SymbolSettings.json";
  }
}
