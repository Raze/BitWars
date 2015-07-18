using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwapTrigger : MonoBehaviour {
	public UnityEngine.UI.Text label;

	public CaptureTrigger capture;
	public Platform swapWith;


	void OnTriggerEnter( Collider other ) {
		bool swap = false;
		switch( capture.platform.ownedBy ) {
		case Team.modern:
			swap = other.GetComponent<ModernCharacter>();
			break;
		case Team.retro:
			swap = other.GetComponent<RetroCharacter>();
			break;
		}
		if( swap ) {
			Platform.swap( capture.platform, swapWith );
			capture.Lock();
		}
	}
}
