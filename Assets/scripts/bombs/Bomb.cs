using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

public class Bomb : MonoBehaviour {

	// Use this for initialization
	public float radius = 5.0F;
	public float power = 10.0F;
	private GameObject bombObj;
	public ParticleSystem prefabExplosion;
	private ParticleSystem explosionSystem;
	private bool exploding;
	protected float damage = 100;
	public GameObject coloredCubes;
	protected ColoredCubesVolume coloredCubesVolume;

	private readonly Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 69, 0, 255), new Color32(255, 140, 0, 255), new Color32(255, 255, 0, 255) }; 

	protected void Start()
	{
		coloredCubes = GameObject.Find ("World");
		coloredCubesVolume = coloredCubes.GetComponent<ColoredCubesVolume>();
		exploding = false;
	}
	void Awake()
	{
		coloredCubes = GameObject.Find ("World");
		coloredCubesVolume = coloredCubes.gameObject.GetComponent<ColoredCubesVolume>();
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
		Explode();
	}
	public void Explode() {
		Explode(damage);
	}

	public void Explode(float gdamage) {
		Vector3 explosionPos = transform.position;
		coloredCubes.SendMessage ("Attack", explosionPos); 

		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		Debug.Log (colliders.Length);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody>();
			if (rb != null) {
				rb.AddExplosionForce (power*5, explosionPos, radius * 5, 3.0F, ForceMode.Force);
				float proximity = (transform.position - hit.transform.position).magnitude;
				float effect = 1 - (proximity / radius);
				hit.SendMessage ("doDamage", effect*gdamage, SendMessageOptions.DontRequireReceiver); 
			} 
		}

		exploding = true;
		explosionSystem = Instantiate (prefabExplosion, transform.position, transform.rotation);
	}
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -100) {
			Explode ();
		}
	}
}
