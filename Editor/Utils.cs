using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;



namespace HananokiEditor.SymbolSettings {
	[InitializeOnLoad]
	public sealed class Utils {

		public static SessionStateBool changeSetting = new SessionStateBool( $"{Package.name}.m_changeSetting" );
		public static SessionStateBool changeEditorSymbols = new SessionStateBool( $"{Package.name}.changeEditorSymbols" );
		public static SessionStateBool activeSymbol = new SessionStateBool( $"{Package.name}.activeSymbol" );

		public static TreeView_EditorSymbols m_treeView_EditorSymbols;

		static Utils() {
			ApplySymbols();
		}


		public static void ApplySymbols() {
			E.Load();
			if( E.i.m_autoSetDidReloadScripts ) {
				EditorApplication.delayCall += InternalApplySymbols;
			}
		}


		internal static void InternalApplySymbols() {
			try {
				Utils.changeSetting.Value = false;
				E.Load();

				var lst = PlayerSettingsUtils.GetScriptingDefineSymbolsAtList();
				var work = new List<string>( lst );

				foreach( var p in E.i.m_autoSymbol ) {
					// シンボルが無効なのでスルー
					if( p.symbolName.IsEmpty() ) continue;

					// 項目がが無効化されているのでスルー
					if( !p.enable ) continue;

					var path = p.GUID.ToAssetPath();
					if( !path.IsExistsFile() && !path.IsExistsDirectory() ) {
						work.Remove( p.symbolName );
					}
					else {
						work.Add( p.symbolName );
					}
				}
				work = work.Distinct().ToList();

				var work2 = new List<string>( lst );

				var w1 = work.OrderBy( x => x ).ToList();
				var w2 = work2.OrderBy( x => x ).ToList();
				bool check = false;
				for( int i = 0; i < w1.Count; i++ ) {
					if( w2.Count <= i ) {
						check = true;
						continue;
					}
					if( w1[ i ] != w2[ i ] ) {
						check = true;
						break;
					}
				}

				if( !check ) return;
				EditorUtility.DisplayProgressBar( Package.nameNicify, S._Therewasautomaticsettingofsymbols_Compile_, 1.00f );
				PlayerSettingsUtils.SetScriptingDefineSymbols( work );
				AssetDatabase.Refresh();
				EditorUtility.ClearProgressBar();
			}
			catch( Exception e ) {
				Debug.LogException( e );
			}
		}


		public static void InitLocalize() {
			EditorApplication.delayCall += () => {
				SettingsDrawer_EditorSymbols.Localize();
				SettingsDrawer_ActiveSymbols.Localize();
				SettingsDrawer_AutoSymbols.Localize();
			};
		}
	}
}
