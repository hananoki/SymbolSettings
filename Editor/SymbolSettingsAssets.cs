#pragma warning disable 649

using UnityEditor;
using UnityEngine;

namespace HananokiEditor.SymbolSettings {

	public class SymbolSettingsAssets : ScriptableObject { }


	[CustomEditor( typeof( SymbolSettingsAssets ) )]
	class SymbolSettingsAssetsInspector : Editor {

		public static SymbolSettingsAssets instance;

		SymbolSettingsGUI m_editor;

		void OnEnable() {
			SettingsProject.Load();
			m_editor = SymbolSettingsGUI.Create( SettingsProject.i );
		}
		
		public override void OnInspectorGUI() {
			m_editor?.OnDrawGUI();
		}
	}
}
