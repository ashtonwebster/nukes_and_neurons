using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class HumanController : GenericFirstPersonController {

	private Recorder _recorder = new Recorder();

	public bool isRecording;
	public string recordingFilename;


	// call the parent constructor
	public HumanController() : base() { 
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		if (isRecording) {
			_recorder.InitializeFile (recordingFilename);
		}

	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		if (Input.GetMouseButtonDown (0)) {
			this.isFiring = true;
			//this.resetView = true;
		} else {
			this.isFiring = false;
		}

		if (isRecording) {
			_recorder.WriteToFile (debug: true);
		}
	}
}
