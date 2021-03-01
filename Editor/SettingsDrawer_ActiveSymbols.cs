using HananokiEditor.SharedModule;
using HananokiRuntime;
using E = HananokiEditor.SymbolSettings.SettingsEditor;



namespace HananokiEditor.SymbolSettings {
	public class SettingsDrawer_ActiveSymbols {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSetting() {
			return new SettingsItem() {
				mode = 0,
				displayName = Package.nameNicify + "/Active Symbols",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}

		static TreeView_ActiveSymbols m_treeView_ActiveSymbols;


		public static void Localize() {
			m_treeView_ActiveSymbols?.Localize();
		}


		public static void DrawGUI() {
			E.Load();
			Helper.New( ref m_treeView_ActiveSymbols );
			ScopeIsCompile.Begin();

			/////////////
			///
			using( new GUILayoutScope( 1, 0 ) ) {
				m_treeView_ActiveSymbols.DrawLayoutGUI();
			}

			ScopeIsCompile.End();
		}


		static void _add() {
			E.AddAutoSymbol();
			m_treeView_ActiveSymbols.RegisterFiles();
		}
		static void _remove() {
			foreach( var p in m_treeView_ActiveSymbols.GetSelectionItems() ) {
				var pp = E.i.m_autoSymbol.Find( x => x.symbolName == p.symbolName );
				if( pp != null ) {
					E.i.m_autoSymbol.Remove( pp );
				}
			}
			E.Save();
			m_treeView_ActiveSymbols.RegisterFiles();
		}
	}
}
