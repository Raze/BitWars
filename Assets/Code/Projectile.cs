using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
	public int instances = 30;

	public float lifeTime = 5f;

	public float speed = 10f;

	public float damage = 5f;

	Vector3 aim;
	Vector3 velocity;

	Vector3 lastPosition;

	float expireTime = -1f;

	Projectile prefab = null;

	public Collider shooter {
		get;
		private set;
	}

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

	public void SetShooter( Collider shooter ) {
		this.shooter = shooter;
		//Physics.IgnoreCollision( shooter, GetComponent<Collider>() );
	}

	public void SetAim( Vector3 aim ) {
		this.aim = aim;
		velocity = this.aim * speed;
	}

	protected void Update() {
		if( Time.time > expireTime ) {
			ProjectileSystem.FreeProjectile( this );
			return;
		}

		lastPosition = transform.position;
		transform.position += velocity * Time.deltaTime;

		var delta = transform.position - lastPosition;
		var ray = new Ray(lastPosition, delta);

		RaycastHit hit;
		if( Physics.Raycast( ray, out hit, delta.magnitude ) ) {
			if( hit.collider != shooter ) {
				var character = hit.collider.GetComponent<Character>();
				if( character != null ) {
					character.damage( damage );
				}
				ProjectileSystem.FreeProjectile( this );
				return;
			}
		}
	}
}
