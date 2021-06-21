using HananokiEditor.Extensions;
using HananokiRuntime;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using UnityObject = UnityEngine.Object;



namespace HananokiEditor.SymbolSettings {
	using Item = TreeView_AutoSymbols.Item;

	public sealed class TreeView_AutoSymbols : HTreeView<Item> {

		const int kMOVE = 0;
		const int kCHECK = 1;
		const int cAudioName = 2;
		const int cCueName = 3;



		public class Item : TreeViewItem {
			public AutoSymbol data;

			public bool enable {
				get => data.enable;
				set => data.enable = value;
			}
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
				headerContent = new GUIContent( "", EditorIcon.collabnew ),
				width = 24,
				maxWidth = 24,
				minWidth = 24,
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

			RegisterFiles();
		}


		public void Localize() {
			multiColumnHeader.state.columns[ 2 ].headerContent = new GUIContent( S._SymbolName );
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
					displayName = p.symbolName,
					currentEnabled = 0 <= defined.FindIndex( x => x == p.symbolName ) ? true : false,
				};
				lst.Add( item );
			}

			m_registerItems = lst;
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
			RollbackLastSelect();
		}


		protected override void OnSingleClickedItem( Item item ) {
			BackupLastSelect( item );
		}


		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;
			bool changed = false;

			if( !args.selected ) {
				if( !item.enable ) {
					var col1 = Color.black;
					col1.a = 0.5f;
					EditorGUI.DrawRect( args.rowRect, col1 );
				}
				if( item.currentEnabled ) {
					var col1 = ColorUtils.RGB( 169, 201, 255 );
					col1.a = 0.5f;
					EditorGUI.DrawRect( args.rowRect, col1 );
				}
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
				case kCHECK:
					ScopeChange.Begin();
					var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.enable );
					if( ScopeChange.End() ) {
						item.enable = _b;
						changed = true;
					}
					break;
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


		#region DragAndDrop

		class DragAndDropData {
			public List<Item> dragItems;

			public static DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
			public DragAndDropArgs args;

			public Item dropItem => (Item) args.parentItem;

			public DragAndDropData( string dragID, DragAndDropArgs args ) {
				this.args = args;
				dragItems = DragAndDrop.GetGenericData( dragID ) as List<Item>;
			}

			public List<Item> HandleBetweenItems( List<Item> items ) {
				if( dragItems == null ) {
					visualMode = DragAndDropVisualMode.None;
					return null;
				}

				visualMode = DragAndDropVisualMode.Move;

				// ドロップを実行します
				if( !args.performDrop ) return null;

				var lst = items.ToList();
				var insertIndex = args.insertAtIndex;

				foreach( var p in dragItems ) {
					var idx = lst.FindIndex( x => x.id == p.id );
					if( idx < insertIndex ) {
						insertIndex--;
					}
					lst.Remove( p );
				}

				lst.InsertRange( insertIndex, dragItems );

				return lst;
			}
		}

		protected override DragAndDropVisualMode HandleDragAndDrop( DragAndDropArgs args ) {
			var data = new DragAndDropData( dragID, args );

			switch( args.dragAndDropPosition ) {
			case DragAndDropPosition.BetweenItems:
				var lst = data.HandleBetweenItems( m_registerItems );
				if( lst != null ) {
					E.i.m_autoSymbol = lst.Select( x => x.data ).ToList();
					E.Save();
					RegisterFiles(  );
					SelectItem( m_registerItems.Find( x => x.symbolName == data.dragItems[ 0 ].symbolName ) );
				}
				break;
			case DragAndDropPosition.UponItem:
			case DragAndDropPosition.OutsideItems:
				DragAndDropData.visualMode = DragAndDropVisualMode.None;
				break;
			}
			return DragAndDropData.visualMode;
		}


		protected override void SetupDragAndDrop( SetupDragAndDropArgs args ) {
			if( args.draggedItemIDs == null ) return;

			DragAndDrop.PrepareStartDrag();

			DragAndDrop.paths = null;
			DragAndDrop.SetGenericData( dragID, ToItems( args.draggedItemIDs ).ToList() );
			DragAndDrop.visualMode = DragAndDropVisualMode.None;
			DragAndDrop.StartDrag( $"{dragID}.StartDrag" );
		}

		protected override bool CanStartDrag( CanStartDragArgs args ) {
			return true;
		}
		#endregion
	}
}

