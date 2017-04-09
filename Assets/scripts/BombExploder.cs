using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExploder : MonoBehaviour {

	// Use this for initialization
	public float radius = 5.0F;
	public float power = 10.0F;

	void Start()
	{
		Debug.Log ("I am initialized!!");


	}
	void OnColliderEnter(Collider other) {
		Debug.Log ("Exploding!!");
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
