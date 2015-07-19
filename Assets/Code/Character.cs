using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	
	CharacterController charCtrl;
	bool onPlatform = false;
	Platform containingPlatform = null;

	private Vector3 baseVelocity;
	public Vector3 BaseVelocity { get { return baseVelocity; } }

	const float rayMaxDistance = 10f;

	// Use this for initialization
	void Start () {
		charCtrl = GetComponent<CharacterController>();
		baseVelocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update() {
		var prevPlatform = containingPlatform;
		containingPlatform = charCtrl.isGrounded ? getPlatformBelow() : null;
		onPlatform = (containingPlatform != null);
		var newParent = onPlatform ? containingPlatform.transform : null;
		if (!charCtrl.isGrounded && containingPlatform == null && prevPlatform != null) {
			baseVelocity = prevPlatform.Velocity;
		} else if (charCtrl.isGrounded) {
			baseVelocity = Vector3.zero;
		}
		transform.parent = newParent;
	}

	void OnGUI() {
		GUILayout.BeginArea( new Rect( 0f, 0f, Screen.width, Screen.height ) );
		GUILayout.Label("Grounded:       " + charCtrl.isGrounded.ToString());
		GUILayout.Label("On platform:    " + onPlatform.ToString());
		//GUILayout.Label("Platform below: " + (getPlatformBelow() != null).ToString());
		GUILayout.EndArea();
	}

	Platform getPlatformBelow() {
		RaycastHit hit;
		if (!Physics.Raycast(new Ray(transform.position, -transform.up), out hit, rayMaxDistance))
			return null;

		var obj = hit.collider.gameObject;
		while (obj != null) {
			var platform = obj.GetComponent<Platform>();
			if (platform != null) return platform;
			obj = obj.transform.parent.gameObject;
		}

		return null;
	}
}
