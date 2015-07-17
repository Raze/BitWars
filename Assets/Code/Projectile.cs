using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
	public int instances = 30;

	public float lifeTime = 5f;

	public Vector3 velocity;

	float expireTime = -1f;

	Projectile prefab = null;

	public void SetPrefab( Projectile prefab ) {
		this.prefab = prefab;
	}

	public Projectile GetPrefab() {
		return prefab;
	}

	public void SetActive(bool active) {
		gameObject.SetActive( active );
		expireTime = Time.time + lifeTime;
	}

	protected void Update() {
		if( Time.time > expireTime ) {
			ProjectileSystem.FreeProjectile( this );
		}

		transform.position += velocity * Time.deltaTime;
	}
}
