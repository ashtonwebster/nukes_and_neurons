using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;
using UnityStandardAssets.Characters.FirstPerson;

public class Initialization : MonoBehaviour {

	public GameObject player1Prefab, player2Prefab;
	public bool isPlayer2Recording;
	private GameObject world;
	private ColoredCubesVolume coloredCubesVolume;
	public int numPlayers = 2;
	private GameObject player1, player2;
	public string currentWorldName;

	// Use this for initialization
	void Start () {
		world = gameObject;
		coloredCubesVolume = gameObject.GetComponent<ColoredCubesVolume>();
	}

	public void beginDelayedSpawn() {
		Invoke ("spawn", 2); 
	}

	public void setPrefab(int playerNum, GameObject prefab) {
		if (playerNum == 1) {
			this.player1Prefab = prefab;
		} else if (playerNum == 2) {
			this.player2Prefab = prefab;
		}
	}

	Vector3 getSpawnPos() {
		PickVoxelResult pickResult;
		Vector3 spawnLoc = new Vector3 (Random.Range (50, 100), 100, Random.Range (50, 100));
		Picking.PickFirstSolidVoxel(coloredCubesVolume, spawnLoc, Vector3.down, 100f, out pickResult);
		Vector3 temp =  pickResult.worldSpacePos;
		temp.y += 2;
		return temp;
	}

	public void teardown() {
		Destroy (this.player1);
		Destroy (this.player2);
		Destroy (gameObject);
	}

	public void setGravity(GameObject player) {
		if (this.currentWorldName == "StarWars") {
			player.GetComponent<GenericFirstPersonController> ().m_GravityMultiplier = 0.5f;
		}
	}

	public void initPlayer1() {
		this.player1 = Instantiate(player1Prefab, getSpawnPos(), Quaternion.identity);
		this.player1.GetComponentInChildren<Camera> ().rect = new Rect (0, 0, 0.5f, 1);
		this.player1.GetComponent<Player> ().setColoredCubes (world);
		setGravity (this.player1);
		if (this.player2 != null) {
			this.player2.GetComponent<HumanController> ().recorder.goal = this.player1;
		}

	}

	public void initPlayer2() { 
		this.player2 = Instantiate (player2Prefab, getSpawnPos (), Quaternion.identity);
		this.player2.GetComponent<Player> ().setColoredCubes (world);
		this.player2.GetComponent<HumanController> ().Initialize (goalParam: this.player1, selfParam: player2.gameObject);
		this.player2.GetComponent<HumanController> ().isRecording = this.isPlayer2Recording;
		this.player2.GetComponent<HumanController> ().usingJoystick = false;
		this.player2.GetComponentInChildren<Camera> ().rect = new Rect (0.5f, 0, 1, 1);
		player2.GetComponent<Player> ().playerNum = 2;
		setGravity (this.player2);
	}

	void spawn() {
		this.initPlayer1 ();
		if (numPlayers == 2) {
			this.initPlayer2 ();
		}
		this.setAIGoal ();

	}

	void respawn(int playerNumber) {
		//Debug.Log ("Respawning player!");
		//Debug.Log (player);
		if (playerNumber == 1) {
			this.initPlayer1 ();
		} else {
			this.initPlayer2 ();
		}
		this.setAIGoal ();

	}

	void setAIGoal() {
		AIController ai = this.player1.GetComponent<AIController> ();
		if (ai != null) {
			// if p1 is the AI, set the goal state
			ai.goalState = this.player2;
			ai._m_AIMouseLook.goal = this.player2;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

}
