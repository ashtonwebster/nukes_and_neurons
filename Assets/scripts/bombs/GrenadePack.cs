using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, 20, 0) * Time.deltaTime);
		transform.Rotate(Time.deltaTime, 0, 1);
		transform.Rotate(Time.deltaTime, 0, 2);

	}

	void onCollisionEnter(Collision other) {
		Debug.Log (other.gameObject.tag);
		if (other.gameObject.tag == "Player") {
			other.gameObject.SendMessage ("addAmmo", "grenade");
			//Destroy (GameObject);
		}
	}
	void onColliderEnter(Collider other) {
		Debug.Log (other.gameObject.tag);
		if (other.gameObject.tag == "Player") {
			other.gameObject.SendMessage ("addAmmo", "grenade");
			//Destroy (GameObject);
		}
	}
	void onTriggerEnter(Collider other) {
		Debug.Log (other.gameObject.tag);
		if (other.gameObject.tag == "Player") {
			other.gameObject.SendMessage ("addAmmo", "grenade");
			//Destroy (GameObject);
		}
	}
}
