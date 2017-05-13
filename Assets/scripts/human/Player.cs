using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cubiquity;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
	public Transform BombPrefab, GrenadePrefab, TripMinePrefab;

	public float max_health = 100;
	public Slider healthSlider;
	public Slider publicHealthBar;

	protected float health = 100;
	public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
	public float flashSpeed = 5f;
	public Image damageImage;
	private bool damaged;

	private bool isWalking = true;
	public int ammo_amount = 5;
	public int num_grenades;
	public int num_bombs;
	public int num_tripmines;
	public int num_sticky;
	public enum bomb_types { BOMB=0, GRENADE=1, TRIPMINE=2, STICKY=3 };
	private int currentWeapon = (int) bomb_types.BOMB;
	private ColoredCubesVolume coloredCubesVolume;
	private bool usingJoystick = true;
	private bool exploding = false;
	public ParticleSystem prefabExplosion;
	private ParticleSystem explosionSystem;
	private readonly Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 69, 0, 255), new Color32(255, 140, 0, 255), new Color32(255, 255, 0, 255) }; 
	private bool dying = false;
	private GameObject world;
	public int playerNum = 1;
	private Transform mycam;
	// Use this for initialization
	void Start () {
		setAmmo ();
		world = GameObject.Find ("World");
		coloredCubesVolume = world.GetComponent<ColoredCubesVolume>();
		usingJoystick = GetComponent<GenericFirstPersonController> ().usingJoystick;
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

	public void setAmmo() {
		num_grenades = ammo_amount;
		num_bombs = ammo_amount;
		num_tripmines = ammo_amount;
		num_sticky = ammo_amount;
	}

	public void addAmmo(string bombType) {
		int amount = ammo_amount;
		if (bombType == "bomb") {
			num_bombs += amount;
		} else if (bombType == "grenade") {
			num_grenades += amount;
		} else if (bombType == "tripmine") {
			num_tripmines += amount;
		} else if (bombType == "sticky") {
			num_sticky += amount;
		} 
		Debug.Log (num_grenades);
	}
		
	public void respawn() {
		setAmmo ();

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
		//GetComponent<Rigidbody>().isKinematic = true;
		//GetComponent<Rigidbody>().useGravity = false;
		exploding = true;
		explosionSystem = Instantiate (prefabExplosion, transform.position, transform.rotation);

	}

	public void doDamage(float damage) {
		damaged = true;
		health -= damage;
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
		float speed = isWalking ? 2 : 4;
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

		bomb.GetComponent<Rigidbody>().AddForce (speed * mycam.transform.forward * 250 * Random.Range(0.8f, 1.2f));
	}

	// Update is called once per frame
	void Update () {

		if (dying) {
			damageImage.color = Color.Lerp (damageImage.color, flashColor, flashSpeed/2 * Time.deltaTime);
			if (damageImage.color == flashColor) {
				die ();
			}
			return;
		}

		if (transform.position.y < -15) {
			dying = true;
		}

		if (!usingJoystick) {
			isWalking = (!Input.GetKey (KeyCode.LeftShift));
		} else {
			isWalking = (!Input.GetButton ("Joy Shift"));
		}
		if (damaged) {
			damageImage.color = flashColor;
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
		if ((!usingJoystick && Input.GetMouseButtonDown (0)) || (usingJoystick && Input.GetButtonDown("Joy Shoot"))) {
			ThrowBomb ();
		}

		if (!usingJoystick) {
			if (Input.GetKey (KeyCode.Alpha1)) {
				currentWeapon = (int)bomb_types.BOMB;
				Debug.Log ("Switching to BOMB!");
			}
			if (Input.GetKey (KeyCode.Alpha2)) {
				currentWeapon = (int)bomb_types.GRENADE;
				Debug.Log ("Switching to GRENADE!");
			}
			if (Input.GetKey (KeyCode.Alpha3)) {
				currentWeapon = (int)bomb_types.TRIPMINE;
				Debug.Log ("Switching to TRIPMINE!");
			}
		} else {
			if (Input.GetButtonDown ("Joy Toggle Forward")) {
				currentWeapon = (currentWeapon + 1) % 3;
			}
//			Debug.Log (currentWeapon);

		}
	}
}
