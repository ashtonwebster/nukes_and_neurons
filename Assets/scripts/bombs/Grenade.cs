using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grenade : Bomb { 
	public float seconds = 1f;
	public Grenade ()
	{
		
	}

	public override void Start() {
		Invoke("Explode", seconds);
	}

	public override void OnCollisionEnter(Collision other) {} //Do nothing for grenade


}


