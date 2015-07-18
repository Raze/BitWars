using UnityEngine;
using System.Collections;

public class RetroCharacter : MonoBehaviour {

	public bool enableDebugging = false;

	const float linearSpeed  = 5f;
	const float angularSpeed = 80f;

	string rotationAxis;
	string movementAxis;

	// Use this for initialization
	void Start () {
		selectJoystick();
	}

	void selectJoystick() {
		string[] names = Input.GetJoystickNames();
		int j;
		for (j=0; j<names.Length; ++j) {
			if (! names[j].Contains("SPEED-LINK")) continue;
		}
		if (j >= names.Length) j = 0;
		rotationAxis = "Joy" + (j+1).ToString() + "Axis0";
		movementAxis = "Joy" + (j+1).ToString() + "Axis1";
		Debug.Log("Using joystick " + (j+1).ToString() + ", \"" + names[j] + "\", for Retro character.");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(
			0f, angularSpeed * Time.deltaTime * Input.GetAxis(rotationAxis), 0f));
		transform.Translate(new Vector3(
			linearSpeed * Time.deltaTime * Input.GetAxis(movementAxis), 0f, 0f));
	}

	void OnGUI () {
		if (!enableDebugging) return;
		GUILayout.BeginArea( new Rect( 0f, 0f, Screen.width, Screen.height ) );
		GUILayout.Label(rotationAxis + ": " + Input.GetAxis(rotationAxis).ToString());
		GUILayout.Label(movementAxis + ": " + Input.GetAxis(movementAxis).ToString());
		GUILayout.EndArea();
	}
}
