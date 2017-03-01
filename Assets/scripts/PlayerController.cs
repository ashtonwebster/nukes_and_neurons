using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public int health = 100;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {

		this.rb = GetComponent<Rigidbody> ();
		this.health = 100;
	}

	void FixedUpdate () 
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		this.rb.AddForce (movement * speed);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
