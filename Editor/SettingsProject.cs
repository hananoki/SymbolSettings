using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



namespace HananokiEditor.SymbolSettings {

	[Serializable]
	public class SettingsProject {

		public static SettingsProject i;

		public bool[] supportPlatform = new bool[ 64 ];


		SettingsProject() {
			supportPlatform = new bool[ 64 ];
			supportPlatform[ 1 ] = true;
		}

		public static void Load() {
			if( i != null ) return;

			i = JsonUtility.FromJson<SettingsProject>( fs.ReadAllText( Package.projectSettingsPath ) );
			if( i == null ) {
				i = new SettingsProject();
			}
			Save();
		}

		public static void Save() {
			File.WriteAllText( Package.projectSettingsPath, JsonUtility.ToJson( i, true ) );
		}


		public static BuildTargetGroup[] GetCuurentSupportTarget() {
			Load();
			var lst = new List<BuildTargetGroup>();
			for( int i = 0; i < 64; i++ ) {
				if( SettingsProject.i.supportPlatform[ i ] ) {
					lst.Add( (BuildTargetGroup) i );
				}
			}
			return lst.ToArray();
		}
	}
}
