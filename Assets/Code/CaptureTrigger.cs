using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaptureTrigger : MonoBehaviour {
	[SerializeField]
	float captureTime;

	float currentTime = 0f;

	public Platform platform;

	[SerializeField]
	SwapTrigger[] swapTriggers;

	ModernCharacter modern;
	RetroCharacter retro;

	bool unlocked = false;

	void OnTriggerEnter( Collider other ) {
		var modern = other.GetComponent<ModernCharacter>();
		var retro = other.GetComponent<RetroCharacter>();
		if( modern != null ) {
			this.modern = modern;
		}
		if( retro != null ) {
			this.retro = retro;
		}
	}

	void OnTriggerExit( Collider other ) {
		var modern = other.GetComponent<ModernCharacter>();
		var retro = other.GetComponent<RetroCharacter>();
		if( modern != null ) {
			this.modern = null;
		}
		if( retro != null ) {
			this.retro = null;
		}
	}

	void Update() {
		bool isInside = false;
		bool isAlone = false;

		if( !platform.floating ) {
			switch( platform.ownedBy ) {
			case Team.modern:
				isInside = retro != null;
				isAlone = modern != null;
				break;
			case Team.retro:
				isInside = modern != null;
				isAlone = retro != null;
				break;
			}
		}

		if( isInside && !unlocked ) {
			if( isAlone ) {
				currentTime += Time.deltaTime;
				if( currentTime > captureTime ) {
					Unlock();
				}
			}
		} else {
			currentTime = 0f;
		}
	}

	public void Lock() {
		unlocked = false;
		for( int i = 0; i < swapTriggers.Length; ++i ) {
			var trigger = swapTriggers[i];
			trigger.gameObject.SetActive( false );
		}
	}

	void Unlock() {
		unlocked = true;

		int i = 0;
		foreach( var platform in Platform.allPlatforms ) {
			if( platform.ownedBy != this.platform.ownedBy && !platform.floating ) {
				var trigger = swapTriggers[i];
				trigger.capture = this;
				trigger.swapWith = platform;
				trigger.label.text = platform.pointValue.ToString();
				trigger.gameObject.SetActive( true );
				i += 1;
				if( i >= swapTriggers.Length ) {
					break;
				}
			}
		}
	}
}
