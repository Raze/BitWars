using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwapTrigger : MonoBehaviour {
	public UnityEngine.UI.Text label;

	public CaptureTrigger capture;
	public Platform swapWith;
	public GameObject contentsPrefab;

	void Start() {
		GameObject contents = Instantiate(contentsPrefab);
		contents.transform.parent = transform;
		contents.transform.localPosition = Vector3.zero;
	}

	void OnTriggerEnter( Collider other ) {
		bool swap = false;
		switch( capture.platform.ownedBy ) {
		case Team.modern:
			swap = other.GetComponent<RetroCharacter>() != null;
			break;
		case Team.retro:
			swap = other.GetComponent<ModernCharacter>() != null;
			break;
		}
		if( swap ) {
			Platform.swap( swapWith, capture.platform );
			capture.Lock();
		}
	}
}
