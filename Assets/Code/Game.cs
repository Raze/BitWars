using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Team {
	retro,
	modern,
}

public class Game : MonoBehaviour {
	public int retroScore = 0;
	public int modernScore = 0;

	public static Game instance {
		get;
		private set;
	}

	void Awake() {
		Platform.swapFinished += ( a, b ) => {
			var t = a.ownedBy;
			a.ownedBy = b.ownedBy;
			b.ownedBy = t;
		};
	}

	void CalculateScore() {
		retroScore = 0;
		modernScore = 0;
		foreach( var platform in Platform.allPlatforms ) {
			if( !platform.floating ) {
				switch( platform.ownedBy ) {
				case Team.retro:
					retroScore += platform.pointValue;
					break;
				case Team.modern:
					modernScore += platform.pointValue;
					break;
				}
			}
		}
	}
}
