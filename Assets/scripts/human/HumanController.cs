using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class HumanController : GenericFirstPersonController {

	private Recorder _recorder = new Recorder();


	// call the parent constructor
	public HumanController() : base() { 
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		_recorder.InitializeFile ();

	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		_recorder.WriteToFile (debug: true);
	}
}
