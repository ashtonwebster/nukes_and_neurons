using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;
using UnityStandardAssets.Characters.FirstPerson;

public class Initialization : MonoBehaviour {

	public GameObject player1Prefab, player2Prefab;
	private ColoredCubesVolume coloredCubesVolume;
	public int numPlayers = 2;
	// Use this for initialization
	void Start () {
		coloredCubesVolume = GetComponent<ColoredCubesVolume>();
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

	void spawn() {
		GameObject player1 = Instantiate(player1Prefab, getSpawnPos(), Quaternion.identity);
		if (numPlayers == 2) {
			player1.GetComponentInChildren<Camera> ().rect = new Rect (0, 0, 0.5f, 1);
			GameObject player2 = Instantiate (player2Prefab, getSpawnPos (), Quaternion.identity);
			player2.GetComponentInChildren<Camera> ().rect = new Rect (0.5f, 0, 1, 1);
			player2.GetComponent<GenericFirstPersonController> ().usingJoystick = false;
			player2.GetComponent<Player> ().playerNum = 2;
		}
	}

	void respawn(int player) {
		Debug.Log ("Respawning player!");
		Debug.Log (player);
		if (player == 1) {
			GameObject player1 = Instantiate (player1Prefab, getSpawnPos(), Quaternion.identity);
			player1.GetComponentInChildren<Camera> ().rect = new Rect (0, 0, 0.5f, 1);
		} else {
			GameObject player2 = Instantiate (player2Prefab, getSpawnPos (), Quaternion.identity);
			player2.GetComponentInChildren<Camera> ().rect = new Rect (0.5f, 0, 1, 1);
			player2.GetComponent<GenericFirstPersonController> ().usingJoystick = false;
			player2.GetComponent<Player> ().playerNum = 2;

		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
