using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpNode : MonoBehaviour {
	[SerializeField]
	JumpNodeTrigger node1 = null;
	[SerializeField]
	JumpNodeTrigger node2 = null;

	[SerializeField]
	LineRenderer lineRenderer = null;

	[SerializeField]
	int iterations = 30;

	AnimationCurve heightByDistance = new AnimationCurve();

	public class JumpFunction {
		public readonly JumpNodeTrigger start;
		public readonly JumpNodeTrigger destination;
		public readonly Vector3 startPos;
		public readonly Vector3 destinationPos;

		AnimationCurve heightByDistance = new AnimationCurve();
		public readonly float distance;
		public readonly float time;
		float elapsedTime;

		public JumpFunction( JumpNodeTrigger start, JumpNodeTrigger destination) {
			this.start = start;
			this.destination = destination;
			startPos = start.transform.position;
			destinationPos = destination.transform.position;
			elapsedTime = 0f;


			JumpNode.CalculateCurve( startPos, destinationPos, heightByDistance, out distance );
			time = distance / Constants.instance.jumpSpeed;
		}

		public Vector3 UpdateStep() {
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / time);
			var pos = Vector3.Lerp( startPos, destinationPos, t );
			pos.y = heightByDistance.Evaluate( t );

			return pos;
		}

		public bool IsDone() {
			return elapsedTime >= time;
		}
	}

	void Start() {
		node1.oppositeTrigger = node2;
		node2.oppositeTrigger = node1;
		node1.node = this;
		node2.node = this;
	}

	Vector3 PositionAt( float t ) {
		var pos1 = node1.transform.position;
		var pos2 = node2.transform.position;

		var pos = Vector3.Lerp( pos1, pos2, t );
		pos.y = heightByDistance.Evaluate( t );

		return pos;
	}

	static void CalculateCurve(Vector3 pos1, Vector3 pos2, AnimationCurve curve, out float distance) {
		{
			var t1 = pos1;
			t1.y = 0f;
			var t2 = pos2;
			t2.y = 0f;
			distance = Vector3.Distance( t1, t2 );
		}

		curve.Clear();
		curve.AddKey( 0f, pos1.y );
		curve.AddKey( 0.5f, distance * Constants.instance.jumpCurveHeightDifference + Mathf.Max( pos1.y, pos2.y ) );
		curve.AddKey( 1f, pos2.y );
	}

	void UpdateRenderer() {
		lineRenderer.SetVertexCount( iterations );

		int count = iterations - 1;
		for( int i = 0; i <= count; ++i ) {
			float t = (float)i / (float)count;
			var pos = PositionAt( t );
			lineRenderer.SetPosition( i, pos );
		}
	}

	void Update() {
		var pos1 = node1.transform.position;
		var pos2 = node2.transform.position;
		float distance;
		CalculateCurve(pos1, pos2, heightByDistance, out distance);
		UpdateRenderer();
	}
}
