using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpNodeTrigger : MonoBehaviour {
	public JumpNode node;
	public JumpNodeTrigger oppositeTrigger;

	void Start() {
		RaycastHit hit;
		if( Physics.Raycast( transform.position, Vector3.down, out hit ) ) {
			transform.position = hit.point + Vector3.up * 0.01f;
		}
	}

	public JumpNode.JumpFunction CreateJump() {
		if( oppositeTrigger != null ) {
			return new JumpNode.JumpFunction( this, oppositeTrigger );
		} else {
			return null;
		}
	}
}
