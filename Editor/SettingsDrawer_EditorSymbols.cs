using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using HananokiRuntime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityReflection;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;



namespace HananokiEditor.SymbolSettings {
	public class SettingsDrawer_EditorSymbols {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSetting() {
			return new SettingsItem() {
				mode = 0,
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}


		public static void Localize() {
			Utils.m_treeView_EditorSymbols?.Localize();
		}

		public static void DrawGUI() {

			E.Load();
			Helper.New( ref Utils.m_treeView_EditorSymbols );

			ScopeIsCompile.Begin();

			HGUIToolbar.Begin();


			ScopeDisable.Begin( Utils.activeSymbol ); if( HGUIToolbar.Button( EditorIcon.toolbar_plus ) ) _add();
			ScopeDisable.Begin( !Utils.m_treeView_EditorSymbols.HasSelection() );
			if( HGUIToolbar.Button( EditorIcon.toolbar_minus ) ) _remove();
			ScopeDisable.End();


			GUILayout.FlexibleSpace();

			if( !Utils.activeSymbol ) {
				ScopeDisable.Begin( !Utils.changeEditorSymbols );
				if( !Utils.changeEditorSymbols ) HGUIToolbar.Button( SS._Apply );
				else {
					if( HGUIToolbar.Button( EditorHelper.TempContent( SS._Apply, EditorIcon.warning ) ) ) _set();
				}
				ScopeDisable.End();
			}


			HGUIToolbar.End();

			/////////////////
			using( new GUILayoutScope( 1, 0 ) ) {
				Utils.m_treeView_EditorSymbols.DrawLayoutGUI();
			}
			ScopeIsCompile.End();

		}


		static void _add() {
			E.AddEditorSymbol();
			Utils.m_treeView_EditorSymbols.RegisterFiles();
		}
		static void _remove() {
			var items = Utils.m_treeView_EditorSymbols.GetSelectionItems().Select( x => x.data ).ToArray();
			E.RemoveEditorSymbol( items );
			Utils.m_treeView_EditorSymbols.RegisterFiles();
		}
		static void _set() {
			var lst = PlayerSettingsUtils.GetScriptingDefineSymbolsAtList();
			var del = new List<string>( 128 );
			var items = Utils.m_treeView_EditorSymbols.m_registerItems;

			foreach( var p in items ) {
				var it = lst.Find( x => x == p.symbolName );
				if( it == null ) continue;

				if( p.enabled && p.GetBuildTargetFlag( UnityEditorEditorUserBuildSettings.activeBuildTargetGroup ) ) continue;

				lst.Remove( p.symbolName );
			}

			var setl = Utils.m_treeView_EditorSymbols.m_registerItems
						.Where( x => x.enabled )
						.Where( x => x.GetBuildTargetFlag( UnityEditorEditorUserBuildSettings.activeBuildTargetGroup ) )
						.Select( x => x.symbolName )
						.ToArray();

			//
			lst.AddRange( setl );
			var adds = lst.Distinct();
			//adds.Print();
			EditorApplication.delayCall += () => {
				PlayerSettingsUtils.SetScriptingDefineSymbols( adds );
				AssetDatabase.Refresh();
				Utils.m_treeView_EditorSymbols.RegisterFiles();
			};
		}
	}
}
