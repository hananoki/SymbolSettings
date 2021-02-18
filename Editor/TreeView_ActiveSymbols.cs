using HananokiEditor.Extensions;
using HananokiRuntime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.SymbolSettings {
	using Item = TreeView_ActiveSymbols.Item;

	public sealed class TreeView_ActiveSymbols : HTreeView<Item> {

		public class Item : TreeViewItem {
			public string symbolName;
		}

		const int kCLIP_BOARD = 0;
		const int kTOGGLE = 1;

		List<string> m_scriptingDefineSymbols;

		public TreeView_ActiveSymbols() : base( new TreeViewState() ) {
			E.Load();
			m_scriptingDefineSymbols = PlayerSettingsUtils.GetScriptingDefineSymbolsAtList();

			showAlternatingRowBackgrounds = true;
			rowHeight = EditorGUIUtility.singleLineHeight;
			var lst = new List<MultiColumnHeaderState.Column>();

			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = GUIContent.none,
				width = 24,
				maxWidth = 24,
				minWidth = 24,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( S._SymbolName ),
			} );

			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( lst.ToArray() ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}



		public void RegisterFiles() {
			Utils.changeEditorSymbols.Value = false;

			var lst = new List<Item>();
			InitID();


			foreach( var p in EditorUserBuildSettings.activeScriptCompilationDefines.OrderBy( x => x ) ) {
				lst.Add( new Item {
					id = GetID(),
					displayName = p,
					//value = pp.Path,
					//depth = 1,
				} );
			}

			m_registerItems = lst;
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
		}


		public bool IsRemove() {
			if( !HasSelection() ) return false;
			var e = GetSelectionItems().Where( x => 0 <= m_scriptingDefineSymbols.IndexOf( x.displayName ) );
			return 0 < e.Count();
		}

		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;

			if( 0 <= m_scriptingDefineSymbols.IndexOf( item.displayName ) ) {
				var col1 = ColorUtils.RGB( 169, 201, 255 );
				col1.a = 0.5f;
				EditorGUI.DrawRect( args.rowRect, col1 );
			}

			for( var i = 0; i < args.GetNumVisibleColumns(); i++ ) {
				var rect = args.GetCellRect( i );
				var columnIndex = args.GetColumn( i );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kCLIP_BOARD: {
					if( HEditorGUI.IconButton( rect.AlignCenter( 18, 16 ), EditorIcon.clipboard ) ) {
						Clipboard.SetText( item.displayName );
						EditorHelper.ShowMessagePop( $"{SS._Copytoclipboard}\n{item.displayName}" );
					}
					break;
				}
				case kTOGGLE: {
					Label( args, rect, item.displayName );
					break;
				}
				}
			}
		}
	}
}

