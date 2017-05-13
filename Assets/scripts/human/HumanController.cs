using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
public class HumanController : GenericFirstPersonController {



	public bool isRecording;
	public string recordingFilename;
	public GameObject goal;
	private Recorder _recorder;

	// call the parent constructor
	public HumanController(GameObject goalParam) : base() { 
		this.goal = goalParam;
		this._recorder = new Recorder(this.goal);

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
		this.isJumping = CrossPlatformInputManager.GetButtonDown("JumpKey");
		this.isFiring = Input.GetMouseButtonDown (0);

		if (isRecording) {
			_recorder.WriteToFile (debug: true);
		}
	}
}
