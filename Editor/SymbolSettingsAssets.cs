using UnityEditor;
using UnityEngine;

namespace Hananoki.SymbolSettings {

	public class SymbolSettingsAssets : ScriptableObject { }


	[CustomEditor( typeof( SymbolSettingsAssets ) )]
	class SymbolSettingsAssetsInspector : Editor {

		public static SymbolSettingsAssets instance;

#if UNITY_2018_3_OR_NEWER
#else
		[MenuItem( "Edit/Symbol Settings" )]
#endif
		public static void Open2() {
			if( instance == null ) {
				instance = CreateInstance<SymbolSettingsAssets>();
				instance.name = Package.name;
			}
			Selection.activeObject = instance;
		}


		SymbolSettingsGUI m_editor;

		void OnEnable() {
			SettingsProject.Load();
			m_editor = SymbolSettingsGUI.Create( SettingsProject.i );
		}

		//void OnDestroy() {
		//	Debug.Log( "OnDestroy" );
		//}

		//void OnDisable() {
		//	Debug.Log( "OnDisable" );

		//}



		/// <summary>
		/// カスタムインスペクターを作成するためにこの関数を実装します
		/// </summary>
		public override void OnInspectorGUI() {
			m_editor?.OnDrawGUI();
		}
	}
}
