using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {
	public static string Fmt( this string fmt, params object[] args ) {
		return string.Format( fmt, args );
	}

	public static void Clear( this AnimationCurve curve ) {
		while( curve.length > 0 ) {
			curve.RemoveKey( 0 );
		}
	}
}