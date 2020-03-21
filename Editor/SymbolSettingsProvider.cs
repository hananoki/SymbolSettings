
#if UNITY_2018_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Hananoki;

#if UNITY_2019_1_OR_NEWER // using
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif
//using Pref = Hananoki.EditorToolbar.EditorToolbarPreference;
//using Settings = Hananoki.EditorToolbar.EditorToolbarSettings;

#if UNITY_2018_3_OR_NEWER

namespace Hananoki.SymbolSettings {

	public class SymbolSettingsProvider : SettingsProvider {

		SymbolSettingsGUI m_editor;

		public SymbolSettingsProvider( string path, SettingsScope scope ) : base( path, scope ) {
			SettingsProject.Load();
			m_editor = SymbolSettingsGUI.Create( SettingsProject.i );
		}


		public override void OnActivate( string searchContext, VisualElement rootElement ) {
			SettingsProject.Load();
			m_editor = SymbolSettingsGUI.Create( SettingsProject.i );
		}

		//public override void OnDeactivate() {}

		public override void OnTitleBarGUI() {
			GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel );
		}

		public void DrawGUI() {

			GUILayout.BeginHorizontal();
			GUILayout.Space( 4 );
			GUILayout.BeginVertical();
			m_editor?.OnDrawGUI();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			//using( new GUILayout.HorizontalScope() ) {
			//	if( GUILayout.Button( "Register Class" ) ) {
			//		var t = typeof( EditorToolbarClass );
			//		Settings.i.reg = new List<Settings.Module>();
			//		foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
			//			foreach( Type type in assembly.GetTypes() ) {
			//				if( type.GetCustomAttribute( t ) == null ) continue;
			//				Settings.i.reg.Add( new Settings.Module( assembly.FullName.Split( ',' )[ 0 ], type.FullName ) );
			//			}
			//		}
			//		Settings.Save();
			//		EditorToolbar.MakeMenuCommand();
			//	}
			//	if( GUILayout.Button( "Unregister Class" ) ) {
			//		Settings.i.reg = new List<Settings.Module>();
			//		Settings.Save();
			//		EditorToolbar.MakeMenuCommand();
			//	}
			//}
			//if( Settings.i.reg != null ) {
			//	foreach( var p in Settings.i.reg ) {
			//		EditorGUILayout.LabelField( $"{p.assemblyName} : {p.className}" );
			//	}
			//}
		}

		public override void OnGUI( string searchContext ) {
			DrawGUI();
		}


		//public override void OnFooterBarGUI() {}

		[SettingsProvider]
		private static SettingsProvider Create() {
			//if( !Pref.i.enableProjectSettingsProvider ) return null;
			var provider = new SymbolSettingsProvider( $"Hananoki/{Package.name}", SettingsScope.Project );

			return provider;
		}
	}
}

#endif

#endif
