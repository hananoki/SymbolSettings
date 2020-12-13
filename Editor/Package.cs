
using System;
using UnityEditor;

namespace HananokiEditor.SymbolSettings {
  public static class Package {
    public const string name = "SymbolSettings";
    public const string nameNicify = "Symbol Settings";
    public const string editorPrefName = "Hananoki.SymbolSettings";
    public const string version = "1.0.8";
    public static string projectSettingsPath => $"{SharedModule.SettingsEditor.projectSettingDirectory}/SymbolSettings.json";
  }
}
