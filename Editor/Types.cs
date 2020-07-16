using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Hananoki;

namespace Hananoki.SymbolSettings {
	[Serializable]
	public class SymbolData {
		[NonSerialized]
		public bool toggle;
		public string name;
		public bool[] platform;
		public SymbolData( bool toggle, string name ) {
			this.toggle = toggle;
			this.name = name;
			this.platform = new bool[ 64 ];
			for( int i = 0; i < 64; i++ ) {
				this.platform[ i ] = true;
			}
		}
		public SymbolData()
			: this( false, "" ) {
		}
	}


	[Serializable]
	public class SymbolDataArray {
		[SerializeField]
		public List<SymbolData> datas = new List<SymbolData>();
	}


	public class SymbolStringList {
		public string[] project;
		public string[] editor;
	}
}
