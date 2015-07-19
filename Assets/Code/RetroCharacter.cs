using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Character))]
public class RetroCharacter : MonoBehaviour {

	public bool enableDebugging = false;
	public Projectile projectilePrefab;
	public Transform projectileTransform;

	const float linearSpeed  = 40f;
	const float angularSpeed = 270f;

	const float airborneLinearSpeed = 0f;
	const float airborneAngularSpeed = angularSpeed;

	string rotationAxis;
	string movementAxis;
	KeyCode shootButton;
	Vector3 moveDirection = new Vector3(0f, 0f, -1f);
	CharacterController characterController;
	Character character;
	JumpNode.JumpFunction jumpFunction;
	JumpNodeTrigger ignoreJumpNode;

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
		var speed = (characterController.isGrounded ? airborneLinearSpeed : linearSpeed)
			* Input.GetAxis(movementAxis);
		transform.Rotate(new Vector3(0f, (characterController.isGrounded ? airborneAngularSpeed : angularSpeed)
			* Time.deltaTime * Input.GetAxis(rotationAxis), 0f));

		if(jumpFunction != null) {
			if(jumpFunction.IsDone()) {
				jumpFunction = null;
			} else {
				transform.position = jumpFunction.UpdateStep();
			}
		} else {
			characterController.SimpleMove(transform.TransformVector(
					character.BaseVelocity + moveDirection*speed));
			GetComponent<Animator>().SetFloat("Speed",
				characterController.isGrounded ? Mathf.Abs(speed) : 0f);
		}

		if (Input.GetKeyDown(shootButton)) {
			ProjectileSystem.ShootProjectile(
				projectilePrefab, projectileTransform.position, projectileTransform.forward, characterController);
		}
	}

	void selectJoystick() {
		string[] names = Input.GetJoystickNames();
		int j;
		for (j=0; j<names.Length; ++j) {
			if (! names[j].Contains("SPEED-LINK")) continue;
		}
		if (j >= names.Length) j = 0;
		Debug.Log("Using joystick " + (j+1).ToString() + ", \"" + names[j] + "\", for Retro character.");

		rotationAxis = "Joy" + (j+1).ToString() + "Axis0";
		movementAxis = "Joy" + (j+1).ToString() + "Axis1";

		int joystickDelta = (int)KeyCode.Joystick2Button0 - (int)KeyCode.Joystick1Button0;
		shootButton = (KeyCode)((int)KeyCode.Joystick1Button2 + joystickDelta*j);
	}
}
