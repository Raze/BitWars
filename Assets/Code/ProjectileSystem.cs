using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ProjectileSystem {

	static Dictionary<GameObject, Stack<Projectile>> projectilePools = new Dictionary<GameObject, Stack<Projectile>>();

	static Transform projectileParent = null;

	public static Projectile ShootProjectile(Projectile prefab, Vector3 pos, Vector3 aim, Collider shooter) {
		var pool = GetPool( prefab );
		if( pool.Count > 0 ) {
			var instance = pool.Pop();
			instance.SetActive( true );
			instance.transform.position = pos;
			instance.transform.forward = aim;
			instance.SetAim(aim);
			instance.SetShooter( shooter );
			return instance;
		} else {
			return null;
		}
	}

	public static void FreeProjectile( Projectile activeProjectile ) {
		var pool = GetPool( activeProjectile.GetPrefab() );
		activeProjectile.SetActive( false );
		pool.Push( activeProjectile );
	}

	static Stack<Projectile> GetPool( Projectile prefab ) {
		Stack<Projectile> pool;
		if( !projectilePools.TryGetValue( prefab.gameObject, out pool ) ) {
			pool = CreatePool( prefab );
			projectilePools.Add( prefab.gameObject, pool );
		}
		return pool;
	}

	static Stack<Projectile> CreatePool( Projectile prefab ) {
		var pool = new Stack<Projectile>( prefab.instances );
		var parent = GetParent();
		for( int i = 0; i < prefab.instances; ++i ) {
			var instance = Object.Instantiate(prefab);
			instance.transform.parent = parent;
			instance.SetPrefab( prefab );
			instance.SetActive( false );
			pool.Push( instance );
		}
		return pool;
	}

	static Transform GetParent() {
		if(projectileParent == null) {
			var obj = new GameObject( "Projectiles" );
			projectileParent = obj.transform;
		}
		return projectileParent;
	}
}
