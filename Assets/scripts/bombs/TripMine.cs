﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TripMine : Bomb { 
	public float armedRadius = 5f;
	public float seconds = 3f;
	private bool armed = false;
	public TripMine ()
	{

	}
	public void Start() {
		base.Start ();
		Debug.Log("Trip mine start");
	}
	public void Awake() {
		
		Debug.Log ("Trip mine awake");
		Invoke("BombArmed", seconds);

	}

	public void BombArmed() {
		armed = true;
		Debug.Log ("Trip Mine now armed");
	}

	public void Update() {
		if (armed) {
			Collider[] colliders = Physics.OverlapSphere (transform.position, armedRadius);
			foreach (Collider c in colliders) {
				if (!c.name.Contains("Octree") && !c.name.Contains("Cube") && !c.name.Contains("TripMine")) {
					Explode(500);
					break;
				}
			}
		}
	}
	void OnCollisionEnter(Collision other) {} //Do nothing for grenade

}

