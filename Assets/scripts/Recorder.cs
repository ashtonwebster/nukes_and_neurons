using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

public class Recorder : MonoBehaviour {

	GameObject child;
	string HEADER = "XZangleToObj,YZangleToObj,DistToObj,";
	public float logPeriod = 0f;
	//private float timeSinceLog = 0f;
	private System.IO.StreamWriter file;

	private GameObject _goal = null;

	public GameObject goal {
		get { 
			if (_goal == null) {
				_goal = GameObject.FindGameObjectWithTag ("goal_state");
			}
			return _goal; 
		}
		set { _goal = value; }
	}
			

	// Use this for initialization
	void Start () {
		file = new System.IO.StreamWriter (@"/tmp/test2.txt", true);
		goal = GameObject.FindGameObjectWithTag ("goal_state");
		child = GameObject.Find ("FirstPersonCharacter");
		// combine the features header and the target movement header
		file.WriteLine(HEADER + ObservedAction.GetHeader ());


	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(new ObservedAction().ToString());

		// write to file with features and target movements
		file.WriteLine(string.Format("{0},{1},{2},{3}", 
			GetXZangleToObj(this.goal),
			GetYZangleToObj(this.goal),
			GetDistToObj(this.goal),
			new ObservedAction().ToString()));
		file.Flush ();

//		Debug.Log (string.Format("XZrot: {0}, YZrot: {1} Dist {2}", GetXZangleToObj (this.goal),
//			GetYZangleToObj(this.goal),
//			GetDistToObj(this.goal)));

	}
		
	float GetXZangleToObj(GameObject obj) {
		// project the vector of the direction we are facing to the floor
		Vector3 forwardProjectedOnFloor = Vector3.ProjectOnPlane (this.transform.forward, Vector3.up);
		// project the vector to the goal to the floor
		Vector3 vectorToGoalProjected = Vector3.ProjectOnPlane (obj.transform.localPosition - this.transform.localPosition, Vector3.up);
		// get angle between vectors (i.e. angle on XZ plane)
		return Vector3.Angle (forwardProjectedOnFloor, vectorToGoalProjected);
	}

	/// <summary>
	/// Gets angle to object in the YZ plane by projecting the vectors on this plane.  This is 
	/// a bit confusing because there are two objects in the FPS controller which separately control
	/// the up-down rotation and left right rotation!
	/// </summary>
	/// <returns>The Y zangle to object.</returns>
	/// <param name="obj">Object to get angle to</param>
	float GetYZangleToObj(GameObject obj) {
		Vector3 forwardProjectedToObj = Vector3.ProjectOnPlane (child.transform.forward, this.transform.right);
		Vector3 vectorToGoalProjected = Vector3.ProjectOnPlane (obj.transform.localPosition - (this.transform.localPosition + child.transform.localPosition), 
			this.transform.right);
		return Vector3.Angle (forwardProjectedToObj, vectorToGoalProjected);
	}

	float GetDistToObj(GameObject obj) {
		return Vector3.Distance (this.transform.localPosition, obj.transform.localPosition);
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
