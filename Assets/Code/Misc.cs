using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {
	public static string Fmt( this string fmt, params object[] args ) {
		return string.Format( fmt, args );
	}
}