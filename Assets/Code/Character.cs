using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	
	CharacterController charCtrl;
	bool onPlatform = false;
	Platform containingPlatform = null;

	public event System.Action onDeath;
	public event System.Action onRespawn;

	[SerializeField]
	float maxHealth;

	[SerializeField]
	Transform spawnPoint;

	public float respawnTime = 3f;
	public UnityEngine.UI.Text respawnLabel;
	[SerializeField]
	UnityEngine.UI.Text healthLabel;

	float currentHealth;

	private Vector3 baseVelocity;
	public Vector3 BaseVelocity { get { return baseVelocity; } }

	const float rayMaxDistance = 10f;

	// Use this for initialization
	void Start () {
		charCtrl = GetComponent<CharacterController>();
		baseVelocity = Vector3.zero;
		currentHealth = maxHealth;
		healthLabel.text = "Health: " + currentHealth;
		respawnLabel.gameObject.SetActive( false );
		transform.position = spawnPoint.position;
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

		if( transform.position.y < -10f ) {
			die();
			return;
		}
	}

	public void damage( float damage ) {
		currentHealth -= damage;
		healthLabel.text = "Health: " + currentHealth;
		if( currentHealth <= 0f ) {
			die();
		}
	}

	public void die() {
		if( onDeath != null ) {
			onDeath();
		}

		containingPlatform = null;
		onPlatform = false;
		transform.parent = null;
		baseVelocity = Vector3.zero;
		healthLabel.gameObject.SetActive( false );

		Game.instance.respawn( this );
	}

	public void respawn() {
		currentHealth = maxHealth;
		healthLabel.text = "Health: " + currentHealth;
		transform.position = spawnPoint.position;

		healthLabel.gameObject.SetActive( true );

		if( onRespawn != null ) {
			onRespawn();
		}
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
			if( obj.transform.parent == null ) {
				break;
			}
			obj = obj.transform.parent.gameObject;
		}

		return null;
	}
}
