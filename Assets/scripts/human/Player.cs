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
	public float throwSpeed = 2f;
//	public int ammo_amount = 5;
//	public int num_grenades;
//	public int num_bombs;
//	public int num_tripmines;
//	public int num_sticky;
	public enum bomb_types { BOMB=0, GRENADE=1, TRIPMINE=2, STICKY=3 };
	protected int currentWeapon = (int) bomb_types.BOMB;
	public ColoredCubesVolume coloredCubesVolume;
	private bool exploding = false;
	public ParticleSystem prefabExplosion;
	private ParticleSystem explosionSystem;
	private Color32[] colors;
	private bool dying = false;
	public GameObject world;
	public int playerNum;
	private Transform mycam;
	public GameObject coloredCubes;

	// Use this for initialization
	protected override void Start () {
		// call generic first person start
		base.Start ();
		// doesn't seem to run if not defined here
		this.colors = new Color32[] { new Color32(255, 0, 0, 255), new Color32(255, 69, 0, 255), new Color32(255, 140, 0, 255), new Color32(255, 255, 0, 255) }; 
//		setAmmo ();
		/*coloredCubes = GameObject.Find ("World");
		if (coloredCubes != null) {
			coloredCubesVolume = coloredCubes.GetComponent<ColoredCubesVolume> ();
		}*/
		mycam = GetComponent<GenericFirstPersonController> ().m_Camera.transform;

		//Initialization i = world.GetComponent<Initialization> ();
	}

	public void setColoredCubes(GameObject coloredCubes) {
		Debug.Log (coloredCubes);
		this.coloredCubes = coloredCubes;
		this.coloredCubesVolume = coloredCubes.GetComponent<ColoredCubesVolume>();
		Debug.Log (this.coloredCubesVolume);
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
			coloredCubes.SendMessage ("respawn", playerNum);
		}
	}
	public void die() {
		exploding = true;
		explosionSystem = Instantiate (prefabExplosion, transform.position, transform.rotation);

	}

	public void doDamage(float damage) {
		damaged = true;
		health -= damage;
		if (healthSlider != null) {
			healthSlider.value = health;
			publicHealthBar.value = health;
		}
		if (health <= 0) {
			dying = true;
		}
	}

	private void ThrowBomb() 
	{
		Vector3 pos = transform.position;
		pos.y += 1 + GetComponent<Collider> ().bounds.size.y;
		float speed = throwSpeed * (this.m_IsWalking ? 1 : 2);
		Transform bombToThrow;
		Debug.Log ("Throwing:");
		Debug.Log (currentWeapon);
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
		bomb.gameObject.GetComponent<Bomb> ().setColoredCubes(coloredCubes);
		bomb.GetComponent<Rigidbody>().AddForce (speed * mycam.transform.forward * Random.Range(0.8f, 1.2f), ForceMode.Impulse);
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

//		if (!usingJoystick) {
//			this.m_IsWalking = (!Input.GetKey (KeyCode.LeftShift));
//		} else {
//			this.m_IsWalking = (!Input.GetButton ("Joy Shift"));
//		}
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

//		if (!usingJoystick) {
//			if (Input.GetKey (KeyCode.Alpha1)) {
//				currentWeapon = (int)bomb_types.BOMB;
//				Debug.Log ("Switching to BOMB!");
//			}
//			if (Input.GetKey (KeyCode.Alpha2)) {
//				currentWeapon = (int)bomb_types.GRENADE;
//				Debug.Log ("Switching to GRENADE!");
//			}
//			if (Input.GetKey (KeyCode.Alpha3)) {
//				currentWeapon = (int)bomb_types.TRIPMINE;
//				Debug.Log ("Switching to TRIPMINE!");
//			}
//		} else {
//			if (Input.GetButtonDown ("Joy Toggle Forward")) {
//				currentWeapon = (currentWeapon + 1) % 3;
//			}
////			Debug.Log (currentWeapon);


	}
}
