using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubiquity;

public class MenuProgression : MonoBehaviour {

	public GameObject playMenuPrefab;
	public GameObject trainMenuPrefab;
	public GameObject aiMenuPrefab;
	public GameObject v1MenuPrefab;
	public GameObject menuControllerPrefab;
	public GameObject forwestWorldPrefab, cityPrefab, starwarsPrefab, flatworldPrefab;
	public GameObject humanController, aiController;

	private GameObject world;

	private GameObject menu1, menu2;
	private GameObject menuController;

	private bool slerpMenusDown = false;
	private bool lerpMenuController = false;
	private bool initMenus = true;
	private string menuStep = "init";
	private int menuChosen = 0;
	private string currentWorld = "menus";
	// Use this for initialization
	void Start () {
		createMenuController ();
	}

	void createMenuController() {
		this.menuController = Instantiate (menuControllerPrefab, new Vector3 (30, 60, -30), Quaternion.identity);
	}

	void destroy(GameObject menuToDestroy) {
		Destroy (menuToDestroy);
	}

	GameObject getRandomPlayWorld() {
		string[] worlds = {"Forest", "City", "StarWars"};
		this.currentWorld = worlds[Random.Range (0, worlds.Length)];

		if (currentWorld == "Forest") {
			return forwestWorldPrefab;
		} else if (currentWorld == "City") {
			return cityPrefab;
		} else if (currentWorld == "StarWars") {
			return starwarsPrefab;
		}
		Debug.Log ("Not sure which world we're doing...");
		return forwestWorldPrefab;

	}

	int handleDualMenus(GameObject menu1Prefab, GameObject menu2Prefab) {
		//First, instantiate the menus
		if (initMenus) {
			this.menu1 = Instantiate (menu1Prefab, new Vector3 (-20, 100, 0), Quaternion.Euler (0, 315, 0));
			this.menu2 = Instantiate (menu2Prefab, new Vector3 (80, 100, 0), Quaternion.Euler (0, 45, 0));
			slerpMenusDown = true;
			menuController.GetComponent<Player> ().canThrowBombs = true; //reenable bomb throwing
			initMenus = false;
			menuChosen = 0;
		}

		//First, lerp the menus down to the user. 
		if (slerpMenusDown) {
			this.menu1.transform.position = Vector3.Lerp (this.menu1.transform.position, new Vector3 (15, 50, 0), Time.deltaTime);
			this.menu2.transform.position = Vector3.Lerp (this.menu2.transform.position, new Vector3 (45, 50, 0), Time.deltaTime);
		} else { //If we're done, lerp them out
			if (this.menu1 != null && menuChosen == 2) {
				this.menu1.transform.position = Vector3.Lerp (this.menu1.transform.position, new Vector3 (-20, 200, 0), Time.deltaTime/4);
			}
			if (this.menu2 != null && menuChosen == 1) {
				this.menu2.transform.position = Vector3.Lerp (this.menu2.transform.position, new Vector3 (80, 200, 0), Time.deltaTime/4);
			}
		}

		//This is critical for the exploding to work - depending on which way the user is looking, set the coloredCubes to that one
		if (menuController != null && this.menu1 != null && this.menu2 != null && !this.menu1.GetComponent<VoxelDestroy>().isDestroyed && !this.menu2.GetComponent<VoxelDestroy>().isDestroyed) {
			if (menuController.transform.rotation.eulerAngles.y > 180) {
				menuController.GetComponent<Player> ().setColoredCubes (this.menu1); // TODO CHECK FOR NOT SWITCHED
			} else {
				menuController.GetComponent<Player> ().setColoredCubes (this.menu2);
			}
		}

		//If the player has selected a menu, return that choice. Destroy the other menu. 
		if (this.menu1 == null) {
			Destroy (this.menu2);
			return 1;
		} else if (this.menu2 == null) {
			Destroy (this.menu1);
			return 2;
		}
		if (this.menu1.GetComponent<VoxelDestroy> ().isDestroyed) {
			menuChosen = 1;
		} else if (this.menu2.GetComponent<VoxelDestroy> ().isDestroyed) {
			menuChosen = 2;
		}
		//If the player has decided and destroyed one, disable bomb throwing until next menu appears
		if (this.menu1.GetComponent<VoxelDestroy> ().isDestroyed || this.menu2.GetComponent<VoxelDestroy> ().isDestroyed) {
			menuController.GetComponent<Player> ().canThrowBombs = false;
			slerpMenusDown = false;
		}
		//!this.menu1.GetComponent<VoxelDestroy> ().isDestroyed && this.menu2.GetComponent<VoxelDestroy> ().isDestroyed
		return 0;
	}

	void destroyMenuController() {
		lerpMenuController = false;
		Destroy (menuController);
	}

	void Update () {
		int choice;
		if (menuStep == "init") {
			choice = handleDualMenus (trainMenuPrefab, playMenuPrefab);
			if (choice == 1) {
				menuStep = "train";
				this.world = Instantiate(flatworldPrefab);
				this.world.GetComponent<Initialization> ().player1Prefab = humanController;
				this.world.GetComponent<Initialization> ().player2Prefab = humanController;
				this.world.GetComponent<Initialization> ().isPlayer2Recording = true;
				lerpMenuController = true;
			} else if (choice == 2) {
				menuStep = "play";
				initMenus = true;
			}
		} else if (menuStep == "train") {
			if (lerpMenuController) {
				menuController.transform.position = Vector3.Lerp (menuController.transform.position, new Vector3 (10, 20, 10), Time.deltaTime * 1.5f);
				Invoke("destroyMenuController", 2);
			}
		} else if (menuStep == "play") {
			choice = handleDualMenus (v1MenuPrefab, aiMenuPrefab);
			if (choice != 0) {
				menuStep = "gameMode";
				this.lerpMenuController = true;
			}
			if (choice == 1) { //1v1 chosen
				this.world = Instantiate (getRandomPlayWorld());
				Initialization i = this.world.GetComponent<Initialization> ();
				i.currentWorldName = this.currentWorld;
				i.setPrefab (1, humanController);
				i.setPrefab (2, humanController);
				i.beginDelayedSpawn ();
			} else if (choice == 2) { //ai chosen
				this.world = Instantiate(flatworldPrefab);
				Initialization i = this.world.GetComponent<Initialization> ();
				i.currentWorldName = this.currentWorld;
				i.setPrefab (1, aiController);
				i.setPrefab (2, humanController);
				i.beginDelayedSpawn ();
			}
		} else if (menuStep == "gameMode") {
			if (lerpMenuController) {
				menuController.transform.position = Vector3.Lerp (menuController.transform.position, new Vector3 (10, 20, 10), Time.deltaTime * 1.5f);
				Invoke("destroyMenuController", 2);
			}
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			world.GetComponent<Initialization> ().teardown ();
			createMenuController ();
			initMenus = true;
			menuStep = "init";
		}
	}
}
