using HananokiEditor.Extensions;
using HananokiRuntime;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.SymbolSettings {
	using Item = TreeView_AutoSymbols.Item;

	public sealed class TreeView_AutoSymbols : HTreeView<Item> {

		const int kMOVE = 0;
		const int cAudioName = 1;
		const int cCueName = 2;



		public class Item : TreeViewItem {
			public AutoSymbol data;

			public string symbolName {
				get => data.symbolName;
				set => data.symbolName = value;
			}
			public string GUID {
				get => data.GUID;
				set => data.GUID = value;
			}
			public bool edit;
			public bool currentEnabled;
		}


		public TreeView_AutoSymbols() : base( new TreeViewState() ) {
			E.Load();

			showAlternatingRowBackgrounds = true;
			rowHeight = EditorGUIUtility.singleLineHeight;

			var lst = new List<MultiColumnHeaderState.Column>();

			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = GUIContent.none,
				width = 18,
				maxWidth = 18,
				minWidth = 18,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( S._SymbolName ),
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( "GUID" ),
			} );

			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( lst.ToArray() ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}


		public void RegisterFiles() {
			Utils.changeSetting.Value = false;


			var lst = new List<Item>();
			InitID();
			var defined = PlayerSettingsUtils.GetScriptingDefineSymbolsAtList();

			foreach( var p in E.i.m_autoSymbol ) {
				var item = new Item {
					id = GetID(),
					data = p,
					currentEnabled = 0 <= defined.FindIndex( x => x == p.symbolName ) ? true : false,
				};
				lst.Add( item );
			}

			m_registerItems = lst;
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
		}



		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;
			bool changed = false;

			if( item.currentEnabled && !args.selected ) {
				var col1 = ColorUtils.RGB( 169, 201, 255 );
				col1.a = 0.5f;
				EditorGUI.DrawRect( args.rowRect, col1 );
			}

			for( var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++ ) {
				var rect = args.GetCellRect( visibleColumnIndex );
				var columnIndex = args.GetColumn( visibleColumnIndex );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kMOVE: {
					EditorGUI.LabelField( rect.AlignCenterH( 16 ), EditorHelper.TempContent( EditorIcon.toolbar_minus ) );
					break;
				}
				case cAudioName: {
					if( item.edit ) {
						ScopeChange.Begin();
						var _t = EditorGUI.TextField( rect.TrimR( 16 ), item.symbolName );
						if( ScopeChange.End() ) {
							item.symbolName = _t;
							changed = true;
						}
					}
					else {
						Label( args, rect.AlignCenterH( EditorGUIUtility.singleLineHeight ), item.symbolName, labelStyle );
					}
					if( HEditorGUI.IconButton( rect.AlignR( 16 ), item.edit ? Icon.GetBuiltinSKins( "in lockbutton" ) : Icon.GetBuiltinSKins( "in lockbutton on" ) ) ) {
						item.edit.Invert();
					}
					break;
				}
				case cCueName: {
					ScopeChange.Begin();
					var _t = HEditorGUI.GUIDObjectField<UnityObject>( rect, item.GUID );
					if( ScopeChange.End() ) {
						item.GUID = _t;
						changed = true;
					}
					break;
				}
				default:
					break;
				}
			}

			if( changed ) {
				E.Save();
				Utils.changeSetting.Value = true;
				//Utils.ApplySymbols();
			}
		}
	}
}

