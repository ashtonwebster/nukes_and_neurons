using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

public class Recorder : MonoBehaviour {

	GameObject goal;
	public float logPeriod = 0f;
	private float timeSinceLog = 0f;
	private System.IO.StreamWriter file;

	// Use this for initialization
	void Start () {
		file = new System.IO.StreamWriter (@"/tmp/test2.txt", true);
		goal = GameObject.Find ("goal_cube");
		file.WriteLine(ObservedAction.GetHeader ());


	}
	
	// Update is called once per frame
	void Update () {
		this.timeSinceLog += Time.deltaTime;
		//if (timeSinceLog > logPeriod) {
//			float dist = Vector3.Distance (this.transform.position, this.goal.transform.position);
//			Debug.Log (dist);

			//Debug.Log(new ObservedAction().ToString());
			file.WriteLine(new ObservedAction().ToString());
			file.Flush ();

			this.timeSinceLog = 0f;
		//}
	}

	public class ObservedAction {
		public float horizontalPan;
		public float forwardPan;
		// note that xRotInput and yRotInput become scaled by sensitivity during the actual rotation
		public float xRotInput;
		public float yRotInput;
		public bool sprintButtonDown;
		public bool fireButtonDown;

		public ObservedAction() {
			this.horizontalPan = CrossPlatformInputManager.GetAxis ("Horizontal");
			this.forwardPan = CrossPlatformInputManager.GetAxis ("Vertical");
			this.yRotInput = CrossPlatformInputManager.GetAxis("Mouse X");
            this.xRotInput = CrossPlatformInputManager.GetAxis("Mouse Y");
			this.sprintButtonDown = Input.GetKey(KeyCode.LeftShift);
			// only true if pressed during the current frame
			this.fireButtonDown = Input.GetMouseButtonDown(0);
		}

		public static string GetHeader() {
			return "horizontalPan,forwardPan,xRotInput,yRotInput,sprintButtonDown,fireButtonDown";
		}

		public override string ToString ()
		{
			return string.Format ("{0},{1},{2},{3},{4},{5}", horizontalPan, forwardPan,
				xRotInput, yRotInput, sprintButtonDown ? 1 : 0, fireButtonDown ? 1: 0);
		}

	}

}
