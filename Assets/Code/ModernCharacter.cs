using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class ModernCharacter : MonoBehaviour {
	class Controller {
		GamePadState previousState;
		GamePadState currentState;

		public void SetCurrentState( GamePadState state ) {
			this.previousState = currentState;
			this.currentState = state;
		}

		public Vector2 leftStick {
			get {
				var stick = currentState.ThumbSticks.Left;
				return new Vector2( stick.X, stick.Y );
			}
		}
		public Vector2 rightStick {
			get {
				var stick = currentState.ThumbSticks.Right;
				return new Vector2( stick.X, stick.Y );
			}
		}

		public struct ButtonData {
			public readonly bool up;
			public readonly bool hold;
			public readonly bool down;

			public ButtonData( ButtonState currentState, ButtonState previousState ) {
				this.up = currentState == ButtonState.Released && previousState == ButtonState.Pressed;
				this.hold = currentState == ButtonState.Pressed;
				this.down = currentState == ButtonState.Pressed && previousState == ButtonState.Released;
			}
		}

		public ButtonData a {
			get { return new ButtonData( currentState.Buttons.A, previousState.Buttons.A ); }
		}
		public ButtonData b {
			get { return new ButtonData( currentState.Buttons.B, previousState.Buttons.B ); }
		}
		public ButtonData x {
			get { return new ButtonData( currentState.Buttons.X, previousState.Buttons.X ); }
		}
		public ButtonData y {
			get { return new ButtonData( currentState.Buttons.Y, previousState.Buttons.Y ); }
		}
	}

	Controller controller = new Controller();

	[SerializeField]
	Projectile projectile;

	[SerializeField]
	float movementSpeed;

	[SerializeField]
	Camera cam;

	[SerializeField]
	Transform head;

	[SerializeField]
	Transform headBone;

	Vector3 targetRotationAngle = Vector3.zero;

	[SerializeField]
	Vector2 rotationSpeed = new Vector2( 10f, 10f );

	[SerializeField]
	float maxXRotation = 0f;
	[SerializeField]
	float minXRotation = 0f;

	JumpNode.JumpFunction jumpFunction;

	JumpNodeTrigger ignoreJumpNode;

	CharacterController characterController;

	void OnEnable() {
		characterController = GetComponent<CharacterController>();
		head.parent = headBone;
		headBone.localPosition = Vector3.zero;
		headBone.localRotation = Quaternion.identity;
	}

	void OnTriggerEnter( Collider other ) {
		var jumpNode = other.GetComponent<JumpNodeTrigger>();
		if( jumpNode != null && ignoreJumpNode != jumpNode ) {
			jumpFunction = jumpNode.CreateJump();
			ignoreJumpNode = jumpNode.oppositeTrigger;
		}
	}

	void OnTriggerExit( Collider other ) {
		var jumpNode = other.GetComponent<JumpNodeTrigger>();
		if( jumpNode != null && ignoreJumpNode == jumpNode ) {
			ignoreJumpNode = null;
		}
	}

	void GroundUpdate() {
		controller.SetCurrentState( GamePad.GetState( PlayerIndex.One ) );

		var velocity = new Vector3(controller.leftStick.x, 0f, controller.leftStick.y) * movementSpeed;

		targetRotationAngle.x += controller.rightStick.y * rotationSpeed.y * Time.deltaTime;
		targetRotationAngle.y += controller.rightStick.x * rotationSpeed.x * Time.deltaTime;
		targetRotationAngle.x = Mathf.Clamp( targetRotationAngle.x, minXRotation, maxXRotation );

		transform.rotation = Quaternion.Euler( targetRotationAngle );

		characterController.SimpleMove((Quaternion.Euler(new Vector3(0f, targetRotationAngle.y, 0f)) * velocity));

		if( controller.a.down ) {
			ProjectileSystem.ShootProjectile( projectile, transform.position, transform.forward, characterController );
		}
	}

	void UpdateJump() {
		if( jumpFunction.IsDone() ) {
			jumpFunction = null;
		} else {
			transform.position = jumpFunction.UpdateStep();
		}
	}

	void Update() {
		if( jumpFunction != null ) {
			UpdateJump();
		} else {
			GroundUpdate();
		}
	}
}
