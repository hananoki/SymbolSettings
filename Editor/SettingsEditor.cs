using HananokiRuntime;
using System.Collections.Generic;
using E = HananokiEditor.SymbolSettings.SettingsEditor;



namespace HananokiEditor.SymbolSettings {
	[System.Serializable]
	public class SettingsEditor {
		public List<AutoSymbol> m_autoSymbol;
		public bool m_autoSetDidReloadScripts;

		public SymbolDataArray m_symbolDataArray;

		public int version;

		public static E i;

		public static void Load() {
			if( i != null ) return;

			i = EditorPrefJson<E>.Get( Package.editorPrefName );
			if( i.version != 2 ) {
				var data = EditorPrefJson<SymbolDataArray>.Get( Package.editorPrefName );
				i.m_symbolDataArray = data;
				i.version = 2;
			}
			Helper.New( ref i.m_autoSymbol );
		}



		public static void Save() {
			if( i == null ) {
				return;
			}
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}


		public static void AddEditorSymbol() {
			i.m_symbolDataArray.datas.Add( new SymbolData() );
			Save();
		}
		public static void RemoveEditorSymbol( SymbolData[] data ) {
			foreach( var p in data ) {
				if( i.m_symbolDataArray.datas.IndexOf( p ) < 0 ) continue;
				i.m_symbolDataArray.datas.Remove( p );
			}
			Save();
		}
		public static void AddAutoSymbol() {
			i.m_autoSymbol.Add( new AutoSymbol { enable = true, } );
			Save();
		}
	}
}

