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
	public GameObject worldPrefab;
	public GameObject cityPrefab;
	public GameObject humanController, aiController;

	private GameObject world;

	private GameObject menu1, menu2;
	private GameObject menuController;

	private bool slerpMenusDown = false;
	private bool slerpMenusUp = false;
	private bool initMenus = true;
	private string menuStep = "init";
	// Use this for initialization
	void Start () {
		menuController = Instantiate (menuControllerPrefab, new Vector3 (30, 60, -30), Quaternion.identity);
	}

	void destroy(GameObject menuToDestroy) {
		Destroy (menuToDestroy);
	}

	int handleDualMenus(GameObject menu1Prefab, GameObject menu2Prefab) {

		//First, instantiate the menus
		if (initMenus) {
			this.menu1 = Instantiate (menu1Prefab, new Vector3 (-20, 100, 0), Quaternion.Euler (0, 315, 0));
			this.menu2 = Instantiate (menu2Prefab, new Vector3 (80, 100, 0), Quaternion.Euler (0, 45, 0));
			slerpMenusDown = true;
			menuController.GetComponent<Player> ().canThrowBombs = true; //reenable bomb throwing
			initMenus = false;
		}

		//First, lerp the menus down to the user. 
		if (slerpMenusDown) {
			this.menu1.transform.position = Vector3.Lerp (this.menu1.transform.position, new Vector3 (15, 50, 0), Time.deltaTime);
			this.menu2.transform.position = Vector3.Lerp (this.menu2.transform.position, new Vector3 (45, 50, 0), Time.deltaTime);
		} else { //If we're done, lerp them out
			if (this.menu1 != null) {
				this.menu1.transform.position = Vector3.Lerp (this.menu1.transform.position, new Vector3 (-20, 100, 0), Time.deltaTime/2);
			}
			if (this.menu2 != null) {
				this.menu2.transform.position = Vector3.Lerp (this.menu2.transform.position, new Vector3 (80, 100, 0), Time.deltaTime/2);
			}
		}

		//This is critical for the exploding to work - depending on which way the user is looking, set the coloredCubes to that one
		if (this.menu1 != null && this.menu2 != null && !this.menu1.GetComponent<VoxelDestroy>().isDestroyed && !this.menu2.GetComponent<VoxelDestroy>().isDestroyed) {
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

		//If the player has decided and destroyed one, disable bomb throwing until next menu appears
		if (this.menu1.GetComponent<VoxelDestroy> ().isDestroyed || this.menu2.GetComponent<VoxelDestroy> ().isDestroyed) {
			menuController.GetComponent<Player> ().canThrowBombs = false;
			slerpMenusDown = false;
		}
		//!this.menu1.GetComponent<VoxelDestroy> ().isDestroyed && this.menu2.GetComponent<VoxelDestroy> ().isDestroyed
		return 0;
	}

	void Update () {
		int choice;
		if (menuStep == "init") {
			choice = handleDualMenus (trainMenuPrefab, playMenuPrefab);
			if (choice == 1) {
				menuStep = "train";
				initMenus = true;
			} else if (choice == 2) {
				menuStep = "play";
				initMenus = true;
			}
		} else if (menuStep == "play") {
			choice = handleDualMenus (v1MenuPrefab, aiMenuPrefab);
			if (choice != 0) {
				menuStep = "gameMode";
				world = Instantiate (worldPrefab);
				world.GetComponent<Initialization>().player2Prefab = humanController;
				Destroy (menuController);
			}
			if (choice == 1) { //1v1 chosen
				world.GetComponent<Initialization>().player1Prefab = humanController;
			} else if (choice == 2) { //ai chosen
				world.GetComponent<Initialization>().player1Prefab = aiController;
			}
		}
			
			//world = Instantiate (cityPrefab);
		//}
	}
}
