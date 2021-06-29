
using System;
using UnityEditor;

namespace HananokiEditor.SymbolSettings {
  public static class Package {
    public const string reverseDomainName = "com.hananoki.symbol-settings";
    public const string name = "SymbolSettings";
    public const string nameNicify = "Symbol Settings";
    public const string editorPrefName = "Hananoki.SymbolSettings";
    public const string version = "2.1.3";
    public static string projectSettingsPath => $"{SharedModule.SettingsEditor.projectSettingDirectory}/SymbolSettings.json";
  }
}
