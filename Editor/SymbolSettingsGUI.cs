//using Hananoki.SymbolSettings.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using SS = Hananoki.SharedModule.S;

namespace Hananoki.SymbolSettings {
	class SymbolSettingsGUI {

		public SettingsProject self;// => target as SymbolSettings;

		public bool m_editMode = false;
		public bool m_hasDirty = false;

		static bool s_showBuiltin;
		Vector2 m_scrollPlatform;
		Vector2 mScrollPos;

		ReorderableList m_rlst;
		ReorderableList m_rlst2;

		bool m_changed;

		List<int> currentSuppotPlatform = new List<int>();

		public class Styles {
			public GUIStyle none;
			public GUIStyle ShurikenModuleTitle;
			public GUIStyle BoldLabel;
			public GUIStyle Button;
			public GUIStyle ButtonIcon;

			public float IconSize;
			public float DropDownOffset;
			public float ColorAlpha;

			public Styles() {
				IconSize = 16;
				DropDownOffset = 10;
				ColorAlpha = 0.25f;
#if UNITY_2019_3_OR_NEWER // UI調整
				IconSize = 18;
				DropDownOffset = 16;
				ColorAlpha = 0.25f;
#endif
				ButtonIcon = new GUIStyle( EditorStyles.label );
				ButtonIcon.margin = new RectOffset( 4, 4, 0, 0 );
				ButtonIcon.padding = new RectOffset( 0, 0, 0, 0 );
				none = new GUIStyle();

				ShurikenModuleTitle = new GUIStyle( "ShurikenModuleTitle" );
				ShurikenModuleTitle.font = EditorStyles.label.font;
				//ShurikenModuleTitle.fontSize += 1;
				BoldLabel = new GUIStyle( "BoldLabel" );
				Button = new GUIStyle( "Button" );
			}
		}

		public static Styles s_Styles;


		public static SymbolSettingsGUI Create( SettingsProject a ) {
			var e = new SymbolSettingsGUI();
			e.self = a;
			e.Init();
			return e;
		}

		void InitSupportPlatform() {
			currentSuppotPlatform = new List<int>();
			for( int i = 0; i < 64; i++ ) {
				if( self.supportPlatform[ i ] ) {
					currentSuppotPlatform.Add( i );
				}
			}
		}


		void Init() {

			InitSupportPlatform();

			foreach( var p in self.m_projectSymbols.datas ) {
				p.toggle = false;
			}

			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup( UnityEditorUserBuildSettings.activeBuildTargetGroup ).Split( ';' );
			foreach( var s in symbols ) {
				int index = self.m_projectSymbols.datas.FindIndex( x => x.name == s );
				if( 0 <= index ) {
					self.m_projectSymbols.datas[ index ].toggle = true;
				}
				index = self.m_editorSymbols.datas.FindIndex( x => x.name == s );
				if( 0 <= index ) {
					self.m_editorSymbols.datas[ index ].toggle = true;
				}
			}
			SettingsProject.Save();

			m_rlst = MakeReorderableList( S._Project_ScriptingDefineSymbols_, self.m_projectSymbols );
			m_rlst2 = MakeReorderableList( S._EditorShared_ScriptingDefineSymbols_, self.m_editorSymbols );
		}



		public ReorderableList MakeReorderableList( string name, SymbolDataArray array ) {
			var r = new ReorderableList( array.datas, typeof( SymbolData ) );

			var paddingY = 4.0f;
			var ctrlH = EditorGUIUtility.singleLineHeight;

			r.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, name );
			};
			r.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				var item = array.datas[ index ];
				var padH = paddingY * 0.5f;
				var h = EditorGUIUtility.singleLineHeight + paddingY;
				var r1 = new Rect( rect.x, padH + rect.y + ( h * 0 ), rect.width, ctrlH );

				EditorGUI.BeginChangeCheck();

				var _no = EditorGUI.TextField( r1, item.name );

				if( EditorGUI.EndChangeCheck() ) {
					item.name = _no;
					m_changed = true;
				}
			};

			r.elementHeight = ( ( ctrlH + paddingY ) );

			return r;
		}


		List<string> GetSymbolList( BuildTargetGroup buildTargetGroup ) {
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup( buildTargetGroup );
			return new List<string>( symbols.Split( ';' ) );
		}


		void OnApplyButton() {


			m_hasDirty = false;

			foreach( var i in currentSuppotPlatform ) {
				var lst = GetSymbolList( (BuildTargetGroup) i ).Where( x => !string.IsNullOrEmpty( x ) ).ToList();

				var symbols = new List<SymbolData>();
				symbols.AddRange( self.m_projectSymbols.datas );
				symbols.AddRange( self.m_editorSymbols.datas );

				foreach( var item in symbols ) {
					lst.RemoveAll( x => x == item.name );
				}
				foreach( var item in symbols ) {
					if( item.toggle ) {
						if( item.platform[ i ] ) {
							lst.Add( item.name );
						}
					}
				}
				var s = string.Join( ";", lst );

				PlayerSettings.SetScriptingDefineSymbolsForGroup( (BuildTargetGroup) i, s );
			}

		}

		bool Toggle( BuildTargetGroup target, string name, bool value ) {
			using( new EditorGUILayout.HorizontalScope( EditorStyles.helpBox ) ) {
				GUILayout.Label( PlatformUtils.GetIcon( (int) target ), s_Styles.ButtonIcon, GUILayout.Width( s_Styles.IconSize ), GUILayout.Height( s_Styles.IconSize ) );
				GUILayout.Space( 16 );
				bool bb = EditorGUILayout.ToggleLeft( new GUIContent( name ), value );
				return bb;
			}
		}

		void DrawEditMode() {
			try {
				EditorGUILayout.Space();

				m_changed = false;

				EditorGUI.BeginChangeCheck();

				m_rlst.DoLayoutList();
				m_rlst2.DoLayoutList();

				if( EditorGUI.EndChangeCheck() ) {
					m_changed = true;
				}

				if( m_changed ) {
					SettingsProject.Save();
				}
			}
			catch( System.Exception e ) {
				UnityEngine.Debug.LogError( e );
			}

			EditorGUILayout.LabelField( S._EnablePlatform, s_Styles.BoldLabel );


			var values = PlatformUtils.GetSupportList();
			bool iOSCheckd = false;
			bool metroCheckd = false;

			using( var scrollView = new EditorGUILayout.ScrollViewScope( m_scrollPlatform, EditorStyles.helpBox ) ) {
				m_scrollPlatform = scrollView.scrollPosition;
				using( new EditorGUILayout.VerticalScope( EditorStyles.helpBox ) ) {
					foreach( var e in values ) {
						bool b = false;
						if( e == BuildTargetGroup.Unknown ) continue;
						EditorGUI.BeginChangeCheck();

						if( e == BuildTargetGroup.iOS ) {
							if( iOSCheckd ) continue;
							b = Toggle( e, "iOS", self.supportPlatform[ (int) e ] );
							iOSCheckd = true;
						}
						else if( e == BuildTargetGroup.WSA ) {
							if( metroCheckd ) continue;
							b = Toggle( e, "WSA", self.supportPlatform[ (int) e ] );
							metroCheckd = true;
						}
						else {
							b = Toggle( e, e.ToString(), self.supportPlatform[ (int) e ] );
						}

						if( EditorGUI.EndChangeCheck() ) {
							self.supportPlatform[ (int) e ] = b;
							SettingsProject.Save();
						}
					}
				}
			}
		}


		void DrawTable( string name, int index, SymbolDataArray array, bool dropdown ) {
			var backgroundColor = GUI.backgroundColor;
			var col1 = ColorUtils.RGB( 169, 201, 255 );
			var col2 = ColorUtils.RGB( 212, 212, 212 );
			bool once = dropdown;
			using( new EditorGUILayout.VerticalScope() ) {
				EditorGUILayout.LabelField( name, s_Styles.ShurikenModuleTitle );
				foreach( var s in array.datas ) {
					if( s.toggle ) {
						GUI.backgroundColor = col1;
					}
					else {
						GUI.backgroundColor = col2;
					}
					using( new EditorGUILayout.HorizontalScope( EditorStyles.helpBox ) ) {
						bool _toggle = EditorGUILayout.ToggleLeft( " " + s.name, s.toggle );
						if( s.toggle != _toggle ) {
							s.toggle = _toggle;
							m_hasDirty = true;
						}

						GUILayout.FlexibleSpace();
						var color = GUI.color;


						Rect curRc = Rect.zero;
						foreach( var i in currentSuppotPlatform ) {
							GUI.color = new Color( 1.0f, 1.0f, 1.0f, 1.00f );
							if( !s.platform[ i ] ) {
								GUI.color = new Color( 1.0f, 1.0f, 1.0f, s_Styles.ColorAlpha );
							}
							bool btn = false;
							var cont = EditorHelper.TempContent( PlatformUtils.GetIcon( i ) );
							var rc = GUILayoutUtility.GetRect( cont, s_Styles.ButtonIcon, GUILayout.Width( s_Styles.IconSize ), GUILayout.Height( s_Styles.IconSize ) );
							var rc2 = rc;
							rc2.y -= 2;
							rc2.height += 4;
							if( 0 <= index && currentSuppotPlatform[ index ] == i )
								EditorGUI.DrawRect( rc2, col1 );
							if( GUI.Button( rc, cont, s_Styles.ButtonIcon ) ) {
								btn = true;
							}
							if( once && 0 <= index && currentSuppotPlatform[ index ] == i ) {
								curRc = GUILayoutUtility.GetLastRect();
							}
							//EditorGUI.DrawRect( GUILayoutUtility.GetLastRect(), new Color( 0, 0, 1, 0.25f ) );
							if( btn ) {
								s.platform[ i ] = !s.platform[ i ];
								SettingsProject.Save();

								m_hasDirty = true;
							}
						}
						GUI.color = color;

						if( once ) {
							var rc = curRc;
							rc.y -= s_Styles.DropDownOffset;

							GUI.Label( rc, new GUIContent( EditorGUIUtility.IconContent( "icon dropdown" ) ) );
							once = false;
						}
					}
				}

			}
			GUI.backgroundColor = backgroundColor;
			GUILayout.Space( 4 );
		}



		void DrawSettingMode() {

			int index = currentSuppotPlatform.FindIndex( x => x == (int) UnityEditorUserBuildSettings.activeBuildTargetGroup );
			if( index < 0 ) {
				EditorGUILayout.HelpBox( S._Nothingcanbespecifiedforthecurrentbuildtarget, MessageType.Warning );
			}
			GUILayout.Space( 8 );
			DrawTable( S._Project_ScriptingDefineSymbols_, index, self.m_projectSymbols, true );
			DrawTable( S._EditorShared_ScriptingDefineSymbols_, index, self.m_editorSymbols, true );

			GUILayout.Space( 8 );

			using( new EditorGUILayout.HorizontalScope() ) {
				GUILayout.FlexibleSpace();
				EditorGUI.BeginDisabledGroup( !m_hasDirty );
				if( GUILayout.Button( SS._Apply ) ) {
					OnApplyButton();
				}
				EditorGUI.EndDisabledGroup();

				if( GUILayout.Button( SS._Clear ) ) {
					PlayerSettings.SetScriptingDefineSymbolsForGroup( UnityEditorUserBuildSettings.activeBuildTargetGroup, "" );
				}

				//if( GUILayout.Button( "Check" ) ) {
				//	Debug.Log( $"Standalone: {PlayerSettings.GetScriptingDefineSymbolsForGroup( BuildTargetGroup.Standalone ) }" );
				//	Debug.Log( $"WebGL: {PlayerSettings.GetScriptingDefineSymbolsForGroup( BuildTargetGroup.WebGL ) }" );
				//}
			}
			DrawGUIPreview();

			DrawBuiltinSymbols();
		}


		void DrawBuiltinSymbols() {
			GUILayout.Space( 8 );
			using( new EditorGUILayout.HorizontalScope() ) {
				GUILayout.FlexibleSpace();
				s_showBuiltin = GUILayout.Toggle( s_showBuiltin, S._ShowBuilt_inSymbols, s_Styles.Button );
			}
			if( !s_showBuiltin ) return;

			var defines = EditorUserBuildSettings.activeScriptCompilationDefines;
			Array.Sort( defines );

			var bs = (GUIStyle) "button";
			var bssize = bs.CalcSize( EditorHelper.TempContent( SS._Copytoclipboard ) );
			using( var scrollView = new EditorGUILayout.ScrollViewScope( mScrollPos, EditorStyles.helpBox ) ) {
				mScrollPos = scrollView.scrollPosition;
				// 定義されているシンボルの一覧を表示します
				foreach( var define in defines ) {
					//EditorGUILayout.BeginHorizontal( GUILayout.Height( 20 ) );
					using( new EditorGUILayout.HorizontalScope( EditorStyles.helpBox ) ) {
						// Copy ボタンが押された場合
						if( GUILayout.Button( SS._Copytoclipboard, GUILayout.Width( bssize.x )/*, GUILayout.Height( 20 )*/ ) ) {
							// クリップボードにシンボル名を登録します
							EditorGUIUtility.systemCopyBuffer = define;
						}

						EditorGUILayout.SelectableLabel( define, GUILayout.Height( 20 ) );
					}
				}
			}
		}


		void DrawGUIPreview() {
			EditorGUILayout.LabelField( S._PreviewScriptingDefineSymbols, s_Styles.BoldLabel );
			EditorGUI.BeginDisabledGroup( true );
			EditorGUILayout.TextField( PlayerSettings.GetScriptingDefineSymbolsForGroup( UnityEditorUserBuildSettings.activeBuildTargetGroup ) );
			EditorGUI.EndDisabledGroup();
		}


		void DrawGUI() {
			using( new EditorGUILayout.HorizontalScope() ) {
				var cont = EditorHelper.TempContent( $"{S._ActiveBuildTarget}: {EditorUserBuildSettings.activeBuildTarget}" );
				var size = s_Styles.BoldLabel.CalcSize( cont );
				EditorGUILayout.LabelField( cont, s_Styles.BoldLabel, GUILayout.Width( size.x ) );
				GUILayout.FlexibleSpace();
				m_editMode = GUILayout.Toggle( m_editMode, S._EditSettings, s_Styles.Button );
				if( m_editMode == false ) {
					InitSupportPlatform();
				}
			}

			if( m_editMode == false ) {
				DrawSettingMode();
			}
			else {
				DrawEditMode();
			}
		}


		public void OnDrawGUI() {
			if( s_Styles == null ) s_Styles = new Styles();

			using( new EditorGUI.DisabledGroupScope( EditorApplication.isCompiling ) ) {
				DrawGUI();
			}
		}
	}
}


