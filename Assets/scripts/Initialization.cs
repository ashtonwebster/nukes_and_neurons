using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;
using UnityStandardAssets.Characters.FirstPerson;

public class Initialization : MonoBehaviour {

	public GameObject player1Prefab, player2Prefab;
	public bool isPlayer2Recording;
	public GameObject world;
	private ColoredCubesVolume coloredCubesVolume;
	public int numPlayers = 2;
	private GameObject player1, player2;
	// Use this for initialization
	void Start () {
		coloredCubesVolume = world.GetComponent<ColoredCubesVolume>();
		Invoke ("spawn", 2); 
	}

	Vector3 getSpawnPos() {
		PickVoxelResult pickResult;
		Vector3 spawnLoc = new Vector3 (Random.Range (0, 50), 100, Random.Range (0, 50));
		Picking.PickFirstSolidVoxel(coloredCubesVolume, spawnLoc, Vector3.down, 100f, out pickResult);
		Vector3 temp =  pickResult.worldSpacePos;
		temp.y += 2;
		return temp;
	}

	public void initPlayer1() {
		this.player1 = Instantiate(player1Prefab, getSpawnPos(), Quaternion.identity);
		this.player1.GetComponentInChildren<Camera> ().rect = new Rect (0, 0, 0.5f, 1);
		this.player1.GetComponent<Player> ().setColoredCubes (world);
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
	}

	void spawn() {
		this.initPlayer1 ();
		if (numPlayers == 2) {
			this.initPlayer2 ();
		}
	}

	void respawn(int playerNumber) {
		//Debug.Log ("Respawning player!");
		//Debug.Log (player);
		if (playerNumber == 1) {
			this.initPlayer1 ();
		} else {
			this.initPlayer2 ();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

}
