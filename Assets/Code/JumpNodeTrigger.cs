using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpNodeTrigger : MonoBehaviour {
	public JumpNode node;
	public JumpNodeTrigger oppositeTrigger;

	public bool connectToClosest = false;

	static List<JumpNodeTrigger> emptyNodes = new List<JumpNodeTrigger>();

	void Start() {
		RaycastHit hit;
		if( Physics.Raycast( transform.position, Vector3.down, out hit ) ) {
			transform.position = hit.point + Vector3.up * 0.01f;
		}

		if( connectToClosest ) {
			emptyNodes.Add( this );
		}
	}

	public JumpNode.JumpFunction CreateJump() {
		if( oppositeTrigger != null ) {
			return new JumpNode.JumpFunction( this, oppositeTrigger );
		} else {
			return null;
		}
	}

	public static JumpNodeTrigger FindClosestNode( Vector3 pos ) {
		pos.y = 0f;
		JumpNodeTrigger closest = null;
		float closestDistance = Constants.instance.jumpNodeConnectDistance;
		for( int i = 0; i < emptyNodes.Count; ++i ) {
			var node = emptyNodes[i];
			if( node.oppositeTrigger == null ) {
				var nodePos = node.transform.position;
				nodePos.y = 0f;
				var distance = Vector3.Distance( pos, nodePos );
				if( distance < closestDistance ) {
					closestDistance = distance;
					closest = node;
				}
			}
		}
		return closest;
	}
}
