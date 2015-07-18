using UnityEngine;
using System.Collections;

public class PlatformTest : MonoBehaviour {

	Platform p1, p2, p3, p4;

	// Use this for initialization
	void Start () {
		p1 = GameObject.Find("Platform 1").GetComponent<Platform>();
		p2 = GameObject.Find("Platform 2").GetComponent<Platform>();
		p3 = GameObject.Find("Platform 3").GetComponent<Platform>();
		p4 = GameObject.Find("Platform 4").GetComponent<Platform>();
		Platform.swap(p1, p2);
		Platform.swap(p3, p4);
	}

	/*
	// Update is called once per frame
	void OnGUI () {
		GUILayout.BeginArea( new Rect( 0f, 0f, Screen.width, Screen.height ) );
		if (p1.floating) {
			GUILayout.Label("Platform 1: " + (Time.time-p1.floatStartTime).ToString() + " / "
		    	   						   + p1.totalFloatDuration.ToString() + ".");
		}
		if (p2.floating) {
			GUILayout.Label("Platform 2: " + (Time.time-p2.floatStartTime).ToString() + " / "
		    	            			   + p2.totalFloatDuration.ToString() + ".");
		}
		GUILayout.EndArea();
	}
	*/
}
