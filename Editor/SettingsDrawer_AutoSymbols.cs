﻿

using HananokiEditor.SharedModule;
using HananokiRuntime;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.SymbolSettings {
	public class SettingsDrawer_AutoSymbols {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSetting() {
			return new SettingsItem() {
				mode = 0,
				displayName = Package.nameNicify + "/AutoSymbols",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}

		static TreeView_AutoSymbols m_treeView_AutoSymbols;


		public static void DrawGUI() {
			E.Load();
			Helper.New( ref m_treeView_AutoSymbols );

			HGUIToolbar.Begin();
			if( HGUIToolbar.Button( EditorIcon.toolbar_plus ) ) _add();
			ScopeDisable.Begin( !m_treeView_AutoSymbols.HasSelection() );
			if( HGUIToolbar.Button( EditorIcon.toolbar_minus ) ) _remove();
			ScopeDisable.End();

			GUILayout.Space( 4 );

			ScopeChange.Begin();
			var _b = HEditorGUILayout.ToggleLeft( S._AutomaticsettingatInitializeOnLoad, E.i.m_autoSetDidReloadScripts );
			if( ScopeChange.End() ) {
				E.i.m_autoSetDidReloadScripts = _b;
				E.Save();
			}
			GUILayout.FlexibleSpace();

			ScopeDisable.Begin( !Utils.changeSetting );
			if( !Utils.changeSetting ) HGUIToolbar.Button( SS._Apply );
			else {
				if( HGUIToolbar.Button( EditorHelper.TempContent( SS._Apply, EditorIcon.warning ) ) ) {
					Utils.ApplySymbols();
				}
			}
			ScopeDisable.End();
			HGUIToolbar.End();

			/////////////
			///
			using( new GUILayoutScope( 1, 0 ) ) {
				m_treeView_AutoSymbols.DrawLayoutGUI();
			}
		}


		static void _add() {
			E.AddAutoSymbol();
			m_treeView_AutoSymbols.RegisterFiles();
		}
		static void _remove() {
			foreach( var p in m_treeView_AutoSymbols.GetSelectionItems() ) {
				var pp = E.i.m_autoSymbol.Find( x => x.symbolName == p.symbolName );
				if( pp != null ) {
					E.i.m_autoSymbol.Remove( pp );
				}
			}
			E.Save();
			m_treeView_AutoSymbols.RegisterFiles();
		}
	}
}
