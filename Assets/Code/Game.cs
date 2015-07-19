using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Team {
	retro,
	modern,
}

public class Game : MonoBehaviour {
	int retroScore = 0;
	int modernScore = 0;

	public UnityEngine.UI.Text retroScoreLabel;
	public UnityEngine.UI.Text modernScoreLabel;

	public static Game instance {
		get;
		private set;
	}

	void Awake() {
		instance = this;
		Platform.swapFinished += ( a, b ) => {
			var t = a.ownedBy;
			a.setOwner(b.ownedBy);
			b.setOwner(t);
		};
	}

	void Update() {
		CalculateScore();
		retroScoreLabel.text = "Score: " + retroScore;
		modernScoreLabel.text = "Score: " + modernScore;
	}

	public void respawn( Character character ) {
		character.gameObject.SetActive( false );
		StartCoroutine( respawnCharacter( character ) );
	}

	IEnumerator respawnCharacter( Character character ) {
		float wait = character.respawnTime;
		var label = character.respawnLabel;
		if( label != null ) {
			label.gameObject.SetActive( true );
		}
		for(float t = 0f; t <= wait; t += Time.deltaTime) {
			if( label != null ) {
				label.text = "Respawn in {0}".Fmt((wait - t).ToString("0.0"));
			}
			yield return null;
		}
		if( label != null ) {
			label.gameObject.SetActive( false );
		}
		character.gameObject.SetActive( true );
		character.respawn();
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
