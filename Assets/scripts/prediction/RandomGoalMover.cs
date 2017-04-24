using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomGoalMover : MonoBehaviour {

	GameObject _goal = null;

	GameObject goalState {
		get { 
			if (_goal == null) {
				_goal = (GameObject)Resources.Load ("prefabs/goal_cube");
			}
			return _goal;
		}
		set { _goal = value; }
	}

	public GameObject player;
				

	// Use this for initialization
	void Start () {
		goalState = Instantiate (goalState);
		//goalState.transform.localPosition = new Vector3 (Random.value * 10, Random.value * 10 + 4, Random.value * 10);
		goalState.transform.localPosition = new Vector3 (Random.value * 10, 4, Random.value * 10);
		//player.transform.GetChild (0).rotation = Quaternion.identity;
		//player.transform.rotation = Quaternion.identity;
		//player.transform.localPosition = new Vector3 (1.93f, 5.76f, -9.93f);
		//player.transform.parent.rotation = Quaternion.identity;
		//player.transform.rotation = Quaternion.Euler (Random.value * 360f, Random.value * 360f, 0f);
		//player.transform.Rotate(new Vector3(Random.value * 360f, Random.value * 360f, 0f));
	}
	
	// Update is called once per frame
	void Update () {
		// restart on click (place the goal in a new position)
		if (Input.GetMouseButtonDown (0)) {
			GameObject.Destroy (goalState);


			this.Start ();
		}
	}
}
