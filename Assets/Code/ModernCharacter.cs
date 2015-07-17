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

	Vector3 aim;


	void Update() {
		controller.SetCurrentState( GamePad.GetState( PlayerIndex.One ) );

		var velocity = controller.leftStick * movementSpeed;
		var currentAim = new Vector3(controller.rightStick.x, 0f, controller.rightStick.y).normalized;

		transform.position += new Vector3(velocity.x, 0f, velocity.y) * Time.deltaTime;
		cam.transform.position = transform.position + Vector3.up * 10f;
		if( currentAim != Vector3.zero ) {
			transform.forward = new Vector3(currentAim.y, 0f, -currentAim.x);
			aim = currentAim;
		}

		if( controller.a.down ) {
			ProjectileSystem.ShootProjectile( projectile, transform.position, aim );
		}
	}
}
