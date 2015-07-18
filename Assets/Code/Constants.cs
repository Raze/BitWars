using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {
	public float jumpSpeed = 10f;
	public float jumpCurveHeightDifference = 0.3f;
	public float jumpNodeConnectDistance = 40f;

	public static Constants instance = null;

	void OnEnable() {
		instance = this;
	}
}
