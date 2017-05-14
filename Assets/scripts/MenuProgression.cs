using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

public class MenuProgression : MonoBehaviour {

	public GameObject playMenuPrefab;
	private GameObject playMenu;
	public GameObject menuControllerPrefab;
	private GameObject menuController;
	public GameObject worldPrefab;
	private GameObject world;

	// Use this for initialization
	void Start () {
		playMenu = Instantiate (playMenuPrefab, new Vector3 (0, 50, 0), Quaternion.identity);
		menuController = Instantiate (menuControllerPrefab, new Vector3 (30, 60, -30), Quaternion.identity);
		menuController.GetComponent<Player> ().setColoredCubes (playMenu);
	}
	
	// Update is called once per frame
	void Update () {
		if (playMenu == null && world == null) {
			world = Instantiate (worldPrefab);
		}
	}
}
