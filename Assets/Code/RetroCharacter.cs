using UnityEngine;
using System.Collections;

public class RetroCharacter : MonoBehaviour {

	public bool enableDebugging = false;
	public Projectile projectilePrefab;
	public Transform projectileTransform;

	const float linearSpeed  = 7.5f;
	const float angularSpeed = 120f;

	string rotationAxis;
	string movementAxis;
	KeyCode shootButton;
	Vector3 moveDirection = new Vector3(0f, 0f, -1f);
	CharacterController characterController;
	JumpNode.JumpFunction jumpFunction;
	JumpNodeTrigger ignoreJumpNode;

	void OnEnable() {
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
		var speed = linearSpeed * Input.GetAxis(movementAxis);
		transform.Rotate(new Vector3(
			0f, angularSpeed * Time.deltaTime * Input.GetAxis(rotationAxis), 0f));

		transform.Translate(moveDirection * speed * Time.deltaTime);
		GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(speed));

		if (Input.GetKeyDown(shootButton)) {
			ProjectileSystem.ShootProjectile(
				projectilePrefab, projectileTransform.position, projectileTransform.forward);
		}


		if(jumpFunction != null) {
			if(jumpFunction.IsDone()) {
				jumpFunction = null;
			} else {
				transform.position = jumpFunction.UpdateStep();
			}
		} else {
			characterController.SimpleMove(transform.TransformVector( moveDirection * speed ));

			GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(speed));
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
