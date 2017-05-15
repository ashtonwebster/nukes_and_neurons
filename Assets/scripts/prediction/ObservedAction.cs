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
	public float sprintButtonDown;
	public float fireButtonDown;
	public float jumpButtonDown;

	public ObservedAction() {

	}

	public void InitializeFromGame() { 
		this.horizontalPan = CrossPlatformInputManager.GetAxis ("Horizontal");
		this.forwardPan = CrossPlatformInputManager.GetAxis ("Vertical");
		this.yRotInput = CrossPlatformInputManager.GetAxis("Mouse X");
		this.xRotInput = CrossPlatformInputManager.GetAxis("Mouse Y");
		this.sprintButtonDown = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.0f;
		// only true if pressed during the current frame
		this.fireButtonDown = Input.GetMouseButtonDown(0) ? 1.0f : 0.0f;
		this.jumpButtonDown = Input.GetKey (KeyCode.Space) ? 1.0f : 0.0f;
	}

	public void InitializeFromSaved(string savedRow) { 
		string[] splits = savedRow.Split (',');
		this.horizontalPan = float.Parse (splits [0]);
		this.forwardPan = float.Parse (splits [1]);
		this.xRotInput = float.Parse (splits [2]);
		this.yRotInput = float.Parse (splits [3]);
		this.sprintButtonDown = float.Parse (splits [4]);
		this.fireButtonDown = float.Parse (splits [5]);
		this.jumpButtonDown = float.Parse (splits [6]);

	}

	public static string GetHeader() {
		return "horizontalPan,forwardPan,xRotInput,yRotInput,sprintButtonDown,fireButtonDown,jumpButtonDown";
	}

	public string PrettyPrint() {
		return string.Format ("h_pan {0}, f_pan {1}, yrot {2}, xrot {3}, sprint {4}, fire {5} jump {6}",
			horizontalPan, forwardPan, xRotInput, yRotInput, sprintButtonDown, fireButtonDown,jumpButtonDown);
	}
		

	public override string ToString ()
	{
		return string.Format ("{0},{1},{2},{3},{4},{5},{6}", horizontalPan, forwardPan,
			xRotInput, yRotInput, sprintButtonDown, fireButtonDown,jumpButtonDown);
	}

}
