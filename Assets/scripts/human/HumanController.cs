using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
public class HumanController : Player {

	public bool isRecording;
	public Recorder recorder;
	public GameObject goal;


	// call the parent constructor
	// this constructor isn't called if using instantiate
	public HumanController(GameObject goalParam, GameObject selfParam) : base() { 
		this.Initialize(goalParam, selfParam);
	}

	public void Initialize(GameObject goalParam, GameObject selfParam) {
		this.goal = goalParam;
		this.recorder = new Recorder(this.goal, selfParam);
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		if (isRecording) {
			recorder.InitializeFile (Config.Instance.node ["recorder_output_file"].Value);
		}
	}
	
	// Update is called once per frame
	protected override void Update () {


		if (usingJoystick) {
			this.isFiring = Input.GetButton ("Joy Shoot");
			this.m_IsWalking = (!Input.GetButton ("Joy Shift"));
			if (Input.GetButtonDown ("Joy Toggle Forward")) {
				currentWeapon = (currentWeapon + 1) % 3;
			}
			this.isJumping = Input.GetButton ("Joy Jump");
		} else {
			this.isFiring = Input.GetMouseButtonDown (0);
			this.m_IsWalking = (!Input.GetKey (KeyCode.LeftShift));
			if (Input.GetKey (KeyCode.Alpha1)) {
				currentWeapon = (int)bomb_types.BOMB;
				Debug.Log ("Switching to BOMB!");
			}
			if (Input.GetKey (KeyCode.Alpha2)) {
				currentWeapon = (int)bomb_types.GRENADE;
				Debug.Log ("Switching to GRENADE!");
			}
			if (Input.GetKey (KeyCode.Alpha3)) {
				currentWeapon = (int)bomb_types.TRIPMINE;
				Debug.Log ("Switching to TRIPMINE!");
			}
			this.isJumping = CrossPlatformInputManager.GetButtonDown("JumpKey");
		}

		if (isRecording) {
			recorder.WriteToFile (debug: true);
		}
		// do this last because we need to update the instance variables with the input
		base.Update ();
	}
}
