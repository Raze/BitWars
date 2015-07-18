



using UnityEngine;
using System.Collections;

public class PlatformTest : MonoBehaviour {

	Platform p1, p2, p3, p4;

	// Use this for initialization
	void Start () {
		p1 = GameObject.Find("Platform 1").GetComponent<Platform>();
		p2 = GameObject.Find("Platform 2").GetComponent<Platform>();
		p3 = GameObject.Find("Platform 3").GetComponent<Platform>();
		p4 = GameObject.Find("Platform 4").GetComponent<Platform>();

		Platform.swapFinished += (pi, pj) => Platform.swap(pi, pj);
		Platform.swap(p1, p2);
		Platform.swap(p3, p4);
	}
}
