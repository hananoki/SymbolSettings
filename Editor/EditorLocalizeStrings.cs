﻿// Generated by Assets/Hananoki/SymbolSettings/Localize/en-US.csv

namespace Hananoki.SymbolSettings {
	public static class L {
		public static string Tr( int n ) {
			try {
				return EditorLocalize.GetPakage( Package.name ).m_Strings[ n ];
			}
			catch( System.Exception ) {
			}
			return string.Empty;
		}
	}
	public static class S {
		public static string _EditSettings => L.Tr( 0 );
		public static string _ActiveBuildTarget => L.Tr( 1 );
		public static string _PreviewScriptingDefineSymbols => L.Tr( 2 );
		public static string _Project_ScriptingDefineSymbols_ => L.Tr( 3 );
		public static string _EditorShared_ScriptingDefineSymbols_ => L.Tr( 4 );
		public static string _BuildOnly_ScriptingDefineSymbols_ => L.Tr( 5 );
		public static string _EnablePlatform => L.Tr( 6 );
		public static string _ShowBuilt_inSymbols => L.Tr( 7 );
		public static string _Nothingcanbespecifiedforthecurrentbuildtarget => L.Tr( 8 );
	}

#if UNITY_EDITOR
  [EditorLocalizeClass]
  public class LocalizeEvent {
    [EditorLocalizeMethod]
    public static void Changed() {
			EditorLocalize.Load( Package.name, "c3b2d643f230b594cabadfe0990156e2" );
		}
	}
#endif
}
