using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExploder : MonoBehaviour {

	// Use this for initialization
	public float radius = 5.0F;
	public float power = 10.0F;
	public GameObject bombObj;
	public ParticleSystem prefabExplosion, explosionSystem;
	private bool exploding;

	private readonly Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 69, 0, 255), new Color32(255, 140, 0, 255), new Color32(255, 255, 0, 255) }; 

	void Start()
	{
		exploding = false;
	}
	void LateUpdate() {
		if (exploding) {
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[explosionSystem.main.maxParticles];
			int numParticlesAlive = explosionSystem.GetParticles (particles);
			for (int i = 0; i < numParticlesAlive; i++) {
				particles [i].startColor = colors[Random.Range(0, colors.Length)];
			}
			explosionSystem.SetParticles (particles, numParticlesAlive);
			exploding = false;
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision other) {
		Debug.Log (other.gameObject.name);
		Vector3 explosionPos = transform.position;

		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
		}
		Explode();
	}
	void Explode() {
		Destroy (bombObj);
		exploding = true;
		explosionSystem = Instantiate (prefabExplosion, transform.position, transform.rotation);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
