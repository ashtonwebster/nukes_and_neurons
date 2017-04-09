using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ObservedAction {
	public float horizontalPan;
	public float forwardPan;
	// note that xRotInput and yRotInput become scaled by sensitivity during the actual rotation
	public float xRotInput;
	public float yRotInput;
	public bool sprintButtonDown;
	public bool fireButtonDown;

	public ObservedAction() {

	}

	public void InitializeFromGame() { 
		this.horizontalPan = CrossPlatformInputManager.GetAxis ("Horizontal");
		this.forwardPan = CrossPlatformInputManager.GetAxis ("Vertical");
		this.yRotInput = CrossPlatformInputManager.GetAxis("Mouse X");
		this.xRotInput = CrossPlatformInputManager.GetAxis("Mouse Y");
		this.sprintButtonDown = Input.GetKey(KeyCode.LeftShift);
		// only true if pressed during the current frame
		this.fireButtonDown = Input.GetMouseButtonDown(0);
	}

	public void InitializeFromSaved(string savedRow) { 
		string[] splits = savedRow.Split (',');
		this.horizontalPan = float.Parse (splits [0]);
		this.forwardPan = float.Parse (splits [1]);
		this.xRotInput = float.Parse (splits [2]);
		this.yRotInput = float.Parse (splits [3]);
		this.sprintButtonDown = splits [4].Equals ("1");
		this.fireButtonDown = splits [5].Equals("1");

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
