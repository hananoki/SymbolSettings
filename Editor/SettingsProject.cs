
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HananokiEditor.SymbolSettings {

	[Serializable]
	public class SettingsProject {

		public static SettingsProject i;

		public SymbolDataArray m_projectSymbols;

		[NonSerialized]
		public SymbolDataArray m_editorSymbols;

		public bool[] supportPlatform = new bool[ 64 ];


		SettingsProject() {
			supportPlatform = new bool[ 64 ];
			supportPlatform[ 1 ] = true;
			m_projectSymbols = new SymbolDataArray();
		}

		public static void Load() {
			if( i != null ) return;

			i = JsonUtility.FromJson<SettingsProject>( fs.ReadAllText( Package.projectSettingsPath ) );
			if( i == null ) {
				i = new SettingsProject();
			}
			i.m_editorSymbols = EditorPrefJson<SymbolDataArray>.Get( Package.editorPrefName );
			Save();
		}

		public static void Save() {
			File.WriteAllText( Package.projectSettingsPath, JsonUtility.ToJson( i ) );
			EditorPrefJson<SymbolDataArray>.Set( Package.editorPrefName, i.m_editorSymbols );
		}


		public static SymbolStringList GetSymbolList() {
			Load();
			return new SymbolStringList() {
				project = i.m_projectSymbols.datas.Select( x => x.name ).ToArray(),
				editor = i.m_editorSymbols.datas.Select( x => x.name ).ToArray(),
			};
		}
	}
}
