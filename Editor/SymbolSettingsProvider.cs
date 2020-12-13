#define ENABLE_HANANOKI_SETTINGS

using UnityEngine;
using HananokiEditor.SharedModule;
using UnityEditor;

namespace HananokiEditor.SymbolSettings {

	public static class SymbolSettingsProvider {

		static SymbolSettingsGUI m_editor;

		public static void DrawGUI2() {
			DrawGUI( "" );
		}

		public static void DrawGUI( string searchText ) {
			SettingsProject.Load();
			if( m_editor == null ) {
				m_editor = SymbolSettingsGUI.Create( SettingsProject.i );
			}
			GUILayout.BeginHorizontal();
			GUILayout.Space( 4 );
			GUILayout.BeginVertical();
			m_editor?.OnDrawGUI();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}



#if !ENABLE_HANANOKI_SETTINGS
#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		[SettingsProvider]
		static SettingsProvider Create() {
			var provider = new SettingsProvider( $"Hananoki/{Package.name}", SettingsScope.Project ) {
				label = Package.name,
				guiHandler = DrawGUI,
				titleBarGuiHandler = () => GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel ),
			};
			return provider;
			////if( !Pref.i.enableProjectSettingsProvider ) return null;
			//var provider = new SymbolSettingsProvider( $"Hananoki/{Package.name}", SettingsScope.Project );

			//return provider;
		}
#endif
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	//[SettingsClass]
	public class SettingsEvent {
		[HananokiSettingsRegister]
		public static SettingsItem Changed() {
			return new SettingsItem() {
				mode = 1,
				displayName = Package.nameNicify,
				version = Package.version,
				gui = SymbolSettingsProvider.DrawGUI2,
			};
		}
	}
#endif
}


