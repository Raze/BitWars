using UnityEngine;
using System.Collections;

public class RetroPlatformTest : MonoBehaviour {

	Platform p1, p2;

	// Use this for initialization
	void Start () {
		p1 = GameObject.Find("Platform 1").GetComponent<Platform>();
		p2 = GameObject.Find("Platform 2").GetComponent<Platform>();

		Platform.swapFinished += (pi, pj) => Invoke("swap", 3);
		Platform.swap(p1, p2);
	}

	void swap() {
		var s = p1; p1 = p2; p2 = s;
		Platform.swap(p1, p2);
	}
}
