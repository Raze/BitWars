using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour {
	public static HashSet<Platform> allPlatforms = new HashSet<Platform>();

	public delegate void PlatformDelegate(Platform p);
	public delegate void PlatformDelegate2(Platform p1, Platform p2);

	public static event PlatformDelegate anyStartedFloating;
	public static event PlatformDelegate anyStoppedFloating;
	public static event PlatformDelegate2 swapStarted;
	public static event PlatformDelegate2 swapFinished;

	// Start floating p1 and p2 such that their positions will eventually be exchanged.
	// p2 will float above p1. Returns an event that fires after _both_ platforms have
	// finished moving.
	public static void swap(GameObject p1, GameObject p2) {
		swap(p1.GetComponent<Platform>(), p2.GetComponent<Platform>());
	}

	public static void swap(Platform p1, Platform p2) {
		p1.floatToPoint(p2.transform.position);
		p2.floatToPoint(p1.transform.position);
		if (swapStarted != null) swapStarted(p1, p2);

		PlatformDelegate onFinish = (p) => {
			if (!p1.floating && !p2.floating && swapFinished != null) swapFinished(p1, p2);
		};
		p1.stoppedFloating += onFinish;
		p2.stoppedFloating += onFinish;
	}

	static bool floatSlotOccupied(int slot) {
		foreach (var p in allPlatforms) {
			if (p.floating && p.floatSlot == slot) return true;
		}
		return false;
	}

	public int pointValue = 1;
	public float hFloatSpeed = 1f;
	public float vFloatSpeed = 1f;

	public event PlatformDelegate startedFloating;
	public event PlatformDelegate stoppedFloating;

	[System.NonSerialized] public bool floating = false;
	[System.NonSerialized] public float totalFloatDuration;
	[System.NonSerialized] public float floatStartTime;

	protected int floatSlot;
	protected float floatHeight;
	protected Vector3 floatOrigin;
	protected Vector3 floatTarget;

	private Vector3 forward;
	private float vFloatTime;

	const float floatHeightStep = 1;
	private Vector3 up = new Vector3(0f, 1f, 0f);

	protected void floatToPoint(Vector3 target) {
		Assert.IsFalse(floating);
		floatOrigin = transform.position;
		floatTarget = target;

		floatSlot = 0;
		while (floatSlotOccupied(floatSlot)) floatSlot += 1;
		floatHeight = floatSlot*floatHeightStep;

		vFloatTime = floatHeight/vFloatSpeed;
		totalFloatDuration = 2*vFloatTime + (floatTarget - floatOrigin).magnitude/hFloatSpeed;
		floatStartTime = Time.time;
		forward = (floatTarget - floatOrigin).normalized;

		floating = true;
		if (startedFloating != null) startedFloating(this);
		if (anyStartedFloating != null) anyStartedFloating(this);
	}

	private void floatToPointDone() {
		floating = false;
		if (stoppedFloating != null) stoppedFloating(this);
		if (anyStoppedFloating != null) anyStoppedFloating(this);
	}

	void OnEnable() {
		allPlatforms.Add(this);
	}
	
	void OnDisable() {
		allPlatforms.Remove(this);
	}
	
	void Update () {
		if (floating) updateFloating();
	}

	void updateFloating() {
		var elapsed = Time.time - floatStartTime;
		if (elapsed < vFloatTime) {
			// Lifting off.
			transform.position = floatOrigin + up*elapsed*vFloatSpeed;
		} else if (totalFloatDuration-elapsed > vFloatTime) {
			// Floating horizontally.
			transform.position = floatOrigin + up*floatHeight + forward*(elapsed-vFloatTime)*hFloatSpeed;
		} else if (elapsed < totalFloatDuration) {
			// Landing.
			transform.position = floatTarget + up*(totalFloatDuration-elapsed)*hFloatSpeed;
		} else {
			// Landed.
			transform.position = floatTarget;
			floatToPointDone();
		}
	}
}