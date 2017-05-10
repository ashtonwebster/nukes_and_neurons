using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grenade : Bomb { 
	public float seconds = 1f;
	public Grenade ()
	{
		
	}

	public void Start() {
		Invoke("Explode", seconds);
	}

	void OnCollisionEnter(Collision other) {} //Do nothing for grenade


}


