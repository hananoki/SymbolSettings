using HananokiEditor.Extensions;
using HananokiRuntime;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityReflection;
using E = HananokiEditor.SymbolSettings.SettingsEditor;
using P = HananokiEditor.SymbolSettings.SettingsProject;



namespace HananokiEditor.SymbolSettings {
	using Item = TreeView_EditorSymbols.Item;

	public sealed class TreeView_EditorSymbols : HTreeView<Item> {

		public class Item : TreeViewItem {
			public SymbolData data;
			public string symbolName {
				get => data.name;
				set => data.name = value;
			}
			public bool GetBuildTargetFlag( BuildTargetGroup v ) {
				return data.platform[ (int) v ];
			}
			public void SetBuildTargetFlag( BuildTargetGroup v, bool flag ) {
				data.platform[ (int) v ] = flag;
			}
			public bool enabled;
			public bool currentEnabled;
			public bool edit;
		}

		const int kDRAG = 0;
		const int kTOGGLE = 1;
		const int kLabel = 2;
		const int cCueName = 3;

		MultiColumnHeaderState.Column[] m_column;
		List<BuildTargetGroup> m_columnBTG;
		BuildTargetGroup m_activeBuildTargetGroup;



		public TreeView_EditorSymbols() : base( new TreeViewState() ) {
			E.Load();
			m_activeBuildTargetGroup = UnityEditorEditorUserBuildSettings.activeBuildTargetGroup;

			showAlternatingRowBackgrounds = true;
			rowHeight = EditorGUIUtility.singleLineHeight;
			var lst = new List<MultiColumnHeaderState.Column>();
			m_columnBTG = new List<BuildTargetGroup>();

			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = GUIContent.none,
				width = 18,
				maxWidth = 18,
				minWidth = 18,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = GUIContent.none,
				width = 20,
				maxWidth = 20,
				minWidth = 20,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( S._SymbolName ),
			} );

			foreach( var p in P.GetCuurentSupportTarget() ) {
				lst.Add( new MultiColumnHeaderState.Column() {
					headerContent = new GUIContent( "", p.IconSmall() ),
					width = 26,
					maxWidth = 26,
					minWidth = 26,
				} );
				m_columnBTG.Add( p );
			}

			m_column = lst.ToArray();
			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( m_column ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}


		public void Localize() {
			m_column[ 2 ].headerContent = new GUIContent( S._SymbolName  );
		}


		public void RegisterFiles() {
			Utils.changeEditorSymbols.Value = false;

			var lst = new List<Item>();
			InitID();

			var values = PlatformUtils.GetSupportList();

			var defined = PlayerSettingsUtils.GetScriptingDefineSymbolsAtList();

			foreach( var p in E.i.m_symbolDataArray.datas ) {
				var item = new Item {
					id = GetID(),
					data = p,
					displayName = p.name,
					enabled = 0 <= defined.FindIndex( x => x == p.name ) ? true : false,
				};
				item.currentEnabled = item.enabled;
				lst.Add( item );
			}

			m_registerItems = lst;
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
			RollbackLastSelect();
		}



		protected override void SingleClickedItem( int id ) {
			var item = ToItem( id );
			item.displayName = item.symbolName;
			BackupLastSelect( item );
		}



		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;

			if( item.currentEnabled && !args.selected ) {
				var col1 = ColorUtils.RGB( 169, 201, 255 );
				col1.a = 0.5f;
				EditorGUI.DrawRect( args.rowRect, col1 );
			}

			for( var i = 0; i < args.GetNumVisibleColumns(); i++ ) {
				var rect = args.GetCellRect( i );
				var columnIndex = args.GetColumn( i );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kDRAG: {
					EditorGUI.LabelField( rect.AlignCenterH( 16 ), EditorHelper.TempContent( EditorIcon.toolbar_minus ) );
					break;
				}
				case kTOGGLE: {
					ScopeChange.Begin();
					var _toggle = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.enabled );
					if( ScopeChange.End() ) {
						item.enabled = _toggle;
						Utils.changeEditorSymbols.Value = true;
					}
					break;
				}
				case kLabel: {
					//
					if( item.edit ) {
						ScopeChange.Begin();
						var _t = EditorGUI.TextField( rect.TrimR( 16 ), item.symbolName );
						if( ScopeChange.End() ) {
							item.symbolName = _t;
							E.Save();
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
				default:
					var btg = m_columnBTG[ columnIndex - cCueName ];
					if( m_activeBuildTargetGroup == btg ) {
						var col1 = ColorUtils.RGB( 169, 201, 255 );
						col1.a = 0.75f;
						EditorGUI.DrawRect( rect.AddW( 4 ), col1 );
					}
					ScopeChange.Begin();
					var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.GetBuildTargetFlag( btg ) );
					if( ScopeChange.End() ) {
						item.SetBuildTargetFlag( btg, _b );
						Utils.changeEditorSymbols.Value = true;
						E.Save();
					}
					break;
				}
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
					E.i.m_symbolDataArray.datas = lst.Select( x => x.data ).ToList();
					E.Save();
					RegisterFiles();
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

