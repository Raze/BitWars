using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Character))]
public class RetroCharacter : MonoBehaviour {

	public bool enableDebugging = false;
	public Projectile projectilePrefab;
	public Transform projectileTransform;

	const float linearSpeed  = 20f;
	const float angularSpeed = 180f;
	const float airborneLinearSpeed = linearSpeed;
	const float airborneAngularSpeed = angularSpeed;
	Vector3 moveDirection = new Vector3(0f, 0f, 1f);
	Vector3 jumpVelocity = new Vector3(0f, 40f, 0f);

	string rotationAxis;
	string movementAxis;
	KeyCode shootButton1, shootButton2;
	KeyCode jumpButton1, jumpButton2;

	CharacterController characterController;
	Character character;
	JumpNode.JumpFunction jumpFunction;
	JumpNodeTrigger ignoreJumpNode;
	Vector3 localVelocity = Vector3.zero;

	void OnEnable() {
		character = GetComponent<Character>();
		characterController = GetComponent<CharacterController>();
	}

	// Use this for initialization
	void Start () {
		selectJoystick();
	}

	void OnTriggerEnter(Collider other) {
		var jumpNode = other.GetComponent<JumpNodeTrigger>();
		if(jumpNode != null && jumpNode != ignoreJumpNode) {
			jumpFunction = jumpNode.CreateJump();
			ignoreJumpNode = jumpNode.oppositeTrigger;
		}
	}

	void OnTriggerExit(Collider other) {
		var jumpNode = other.GetComponent<JumpNodeTrigger>();
		if(jumpNode == ignoreJumpNode) {
			ignoreJumpNode = null;
		}
	}

	// Update is called once per frame
	void Update () {
		if(jumpFunction != null) {
			// Continue an automatic jump.
			if(jumpFunction.IsDone()) {
				jumpFunction = null;
			} else {
				transform.position = jumpFunction.UpdateStep();
			}
		}

		// Apply velocity.
		var grounded = characterController.isGrounded;
		var speed = (grounded ? linearSpeed : airborneLinearSpeed)*Input.GetAxis(movementAxis);
		transform.Rotate(new Vector3(
			0f, (grounded ? angularSpeed : airborneAngularSpeed)*Time.deltaTime*Input.GetAxis(rotationAxis), 0f));
		if (jumpFunction == null) {
			var ambientVelocity = character.BaseVelocity + localVelocity;
			Vector3 motion = transform.TransformVector(
				(ambientVelocity + moveDirection*speed)*Time.deltaTime);
			if (ambientVelocity.magnitude < 0.01)
				characterController.SimpleMove(motion);
			else
				characterController.Move(motion);
		}
		GetComponent<Animator>().SetFloat("Speed", grounded ? Mathf.Abs(speed) : 0f);

		// Apply accelleration due to gravity.
		if (grounded && Vector3.Dot(localVelocity, Physics.gravity) > 0) {
			localVelocity = Vector3.zero;
		} else {
			localVelocity += Physics.gravity * Time.deltaTime;
		}

		if (Input.GetKeyDown(shootButton1) || Input.GetKeyDown(shootButton2)) {
			// Effect the shoot button.
			ProjectileSystem.ShootProjectile(
				projectilePrefab, projectileTransform.position, projectileTransform.forward, characterController);
		}
		
		if (grounded && (Input.GetKeyDown(jumpButton1) || Input.GetKeyDown(jumpButton2))) {
			// Effect the jump button.
			localVelocity += jumpVelocity;
		}
	}

	void selectJoystick() {
		string[] names = Input.GetJoystickNames();
		int j;
		for (j=0; j<names.Length; ++j) {
			if (names[j].Contains("SPEED-LINK")) break;
		}
		if (j >= names.Length) j = 0;
		Debug.Log("Using joystick " + (j+1).ToString() + ", \"" + names[j] + "\", for Retro character.");

		rotationAxis = "Joy" + (j+1).ToString() + "Axis0";
		movementAxis = "Joy" + (j+1).ToString() + "Axis1";

		int joystickDelta = (int)KeyCode.Joystick2Button0 - (int)KeyCode.Joystick1Button0;
		jumpButton1 = (KeyCode)((int)KeyCode.Joystick1Button0 + joystickDelta*j);
		jumpButton2 = (KeyCode)((int)KeyCode.Joystick1Button1 + joystickDelta*j);
		shootButton1 = (KeyCode)((int)KeyCode.Joystick1Button2 + joystickDelta*j);
		shootButton2 = (KeyCode)((int)KeyCode.Joystick1Button3 + joystickDelta*j);
	}

	void OnGUI() {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.red;
		GUILayout.BeginArea( new Rect( 0f, 0f, Screen.width, Screen.height ) );
		GUILayout.Label ("grounded: " + characterController.isGrounded.ToString(), style);
		GUILayout.Label ("localVelocity: " + localVelocity.ToString(), style);
		GUILayout.EndArea ();
	}
}
