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
				

	// Use this for initialization
	void Start () {
		goalState = Instantiate (goalState);
		goalState.transform.localPosition = new Vector3 (Random.value * 10, 4, Random.value * 10);
	}
	
	// Update is called once per frame
	void Update () {
		// restart on click
		if (Input.GetMouseButtonDown (0)) {
			GameObject.Destroy (goalState);
			this.Start ();
			//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
	}
}
