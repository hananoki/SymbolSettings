using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using P = HananokiEditor.SymbolSettings.SettingsProject;

namespace HananokiEditor.SymbolSettings {
	public class SettingsDrawer_Platform {
		[HananokiSettingsRegister]
		public static SettingsItem RegisterSetting() {
			return new SettingsItem() {
				mode = 0,
				displayName = Package.nameNicify + "/Platform",
				version = "",
				gui = DrawGUI,
			};
		}

		static TreeView_EditorSymbols m_treeView;

		public static void DrawGUI() {
			E.Load();
			P.Load();
			var targetGroupList = PlatformUtils.GetSupportList();

			ScopeVertical.Begin();
			HEditorGUILayout.HeaderTitle( "Platform" );
			GUILayout.Space( 8 );
			foreach( var t in targetGroupList ) {
				ScopeChange.Begin();

				var _b = HEditorGUILayout.ToggleBox( P.i.supportPlatform[ (int) t ], t.Icon(), t.GetName() );
				if( ScopeChange.End() ) {
					P.i.supportPlatform[ (int) t ] = _b;
					P.Save();
					//BuildAssistWindow.ChangeActiveTarget();
					Utils.m_treeView_EditorSymbols=null;
				}
			}
			ScopeVertical.End();


		}

	}
}
