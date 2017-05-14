using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Cubiquity;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : GenericFirstPersonController {
	public Transform BombPrefab, GrenadePrefab, TripMinePrefab;

	public float max_health = 100;
	public Slider healthSlider;
	public Slider publicHealthBar;

	protected float health = 100;
	public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
	public float flashSpeed = 5f;
	public Image damageImage;
	private bool damaged;
	protected bool isFiring;
	protected double nextAllowedFiringTime = 0;

	public enum bomb_types { BOMB=0, GRENADE=1, TRIPMINE=2, STICKY=3 };
	protected int currentWeapon = (int) bomb_types.BOMB;
	private ColoredCubesVolume coloredCubesVolume;
	private bool exploding = false;
	public ParticleSystem prefabExplosion;
	private ParticleSystem explosionSystem;
	private Color32[] colors;
	private bool dying = false;
	private GameObject world;
	public int playerNum;
	private Transform mycam;

	// Use this for initialization
	protected override void Start () {
		// call generic first person start
		base.Start ();
		// doesn't seem to run if not defined here
		this.colors = new Color32[] { new Color32(255, 0, 0, 255), new Color32(255, 69, 0, 255), new Color32(255, 140, 0, 255), new Color32(255, 255, 0, 255) }; 
//		setAmmo ();
		world = GameObject.Find ("World");
		coloredCubesVolume = world.GetComponent<ColoredCubesVolume>();
		mycam = GetComponent<GenericFirstPersonController> ().m_Camera.transform;

		//Initialization i = world.GetComponent<Initialization> ();
	}

	Vector3 getSpawnPos() {
		PickVoxelResult pickResult;
		Vector3 spawnLoc = new Vector3 (Random.Range (0, 50), 100, Random.Range (0, 50));
		Picking.PickFirstSolidVoxel(coloredCubesVolume, spawnLoc, Vector3.down, 100f, out pickResult);
		Vector3 temp =  pickResult.worldSpacePos;
		temp.y += 2;
		return temp;
	}
	public void respawn() {
//		setAmmo ();

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
			world.SendMessage ("respawn", playerNum);
		}
	}
	public void die() {
		exploding = true;
		explosionSystem = Instantiate (prefabExplosion, transform.position, transform.rotation);

	}

	public void doDamage(float damage) {
		damaged = true;
		health -= damage;
		Debug.Log(string.Format("player {0} healthslider {1}", this.playerNum, healthSlider));
		healthSlider.value = health;
		publicHealthBar.value = health;
		if (health <= 0) {
			dying = true;
		}
	}

	private void ThrowBomb() 
	{
		Vector3 pos = transform.position;
		pos.y += 1 + GetComponent<Collider> ().bounds.size.y;
		// add bomb speed to current velocity of object
		float speed = this.GetComponent<Rigidbody> ().velocity.magnitude + (this.m_IsWalking ? 2 : 4);
		Transform bombToThrow;
		//Debug.Log ("Throwing:");
		//Debug.Log (currentWeapon);
		switch (currentWeapon) {
		case (int) bomb_types.BOMB:
			bombToThrow = BombPrefab;
			break;
		case (int) bomb_types.GRENADE:
			bombToThrow = GrenadePrefab;
			break;
		case (int) bomb_types.TRIPMINE:
			bombToThrow = TripMinePrefab;
			break;
		default:
			bombToThrow = BombPrefab;
			break;
		}
		Transform bomb = Instantiate (bombToThrow, pos, Quaternion.identity);

		bomb.GetComponent<Rigidbody>().AddForce (speed * mycam.transform.forward * 250 * Random.Range(0.8f, 1.2f));
	}

	double GetEpochTime() {
		TimeSpan t = DateTime.UtcNow - new DateTime(2017, 4, 24);
		return (float) t.TotalSeconds;
	}
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		if (dying) {
			// display "Bond" death image for a few frames and then die?
			damageImage.color = Color.Lerp (damageImage.color, flashColor, flashSpeed/2 * Time.deltaTime);
			if (damageImage.color == flashColor) {
				die ();
			}
			return;
		}

		if (transform.position.y < -15) {
			dying = true;
		}

		if (damaged) {
			damageImage.color = flashColor;
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;

		if (this.isFiring && this.nextAllowedFiringTime <= this.GetEpochTime()) {
			ThrowBomb ();
			this.nextAllowedFiringTime = this.GetEpochTime() + this.firingCooldown;
		}

	}
}
