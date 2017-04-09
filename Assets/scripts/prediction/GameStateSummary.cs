using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The features observed at a particular point in time
/// </summary>
public class GameStateSummary {

	private GameObject selfObj;
	private GameObject child;
	private GameObject goal;

	public float XZAngleToObj;
	public float YZAngletoObj;
	public float distToObj;

	public GameStateSummary() { 

		goal = GameObject.FindGameObjectWithTag ("goal_state");
	}

	public static string GetHeader() {
		return 	"XZangleToObj,YZangleToObj,DistToObj";
	}


	public void InitializeFromGame(GameObject selfObj, GameObject childObj) { 
		this.selfObj = selfObj;
		this.child = childObj;
		this.XZAngleToObj = this.GetXZangleToObj ();
		this.YZAngletoObj = this.GetYZangleToObj ();
		this.distToObj = this.GetDistToObj ();
	}

	public void InitializeFromSaved(string savedRow) { 
		string[] splits = savedRow.Split (',');
		this.XZAngleToObj = float.Parse (splits [0]);
		this.YZAngletoObj = float.Parse (splits [1]);
		this.distToObj = float.Parse (splits [2]);
	}


	public override string ToString() {
		return string.Format ("{0},{1},{2}", XZAngleToObj, YZAngletoObj, distToObj);
			

	}

	float GetXZangleToObj() {
		// project the vector of the direction we are facing to the floor
		Vector3 forwardProjectedOnFloor = Vector3.ProjectOnPlane (this.selfObj.transform.forward, Vector3.up);
		// project the vector to the goal to the floor
		Vector3 vectorToGoalProjected = Vector3.ProjectOnPlane (this.goal.transform.localPosition - this.selfObj.transform.localPosition, Vector3.up);
		// get angle between vectors (i.e. angle on XZ plane)
		// Unity does the absolute value of the angle which is stupid so we have to find the direction...
		float absoluteAngle = Vector3.Angle (forwardProjectedOnFloor, vectorToGoalProjected);
		bool isAnglePositive = Vector3.Cross (vectorToGoalProjected, forwardProjectedOnFloor).y >= 0;
		return isAnglePositive ? absoluteAngle : -1 * absoluteAngle;
	}

	/// <summary>
	/// Gets angle to object in the YZ plane by projecting the vectors on this plane.  This is 
	/// a bit confusing because there are two objects in the FPS controller which separately control
	/// the up-down rotation and left right rotation!
	/// </summary>
	/// <returns>The Y zangle to object.</returns>
	/// <param name="obj">Object to get angle to</param>
	float GetYZangleToObj() {
		Vector3 forwardProjectedToObj = Vector3.ProjectOnPlane (this.child.transform.forward, this.selfObj.transform.right);
		Vector3 vectorToGoalProjected = Vector3.ProjectOnPlane (this.goal.transform.localPosition - 
			(this.selfObj.transform.localPosition + this.child.transform.localPosition), 
			selfObj.transform.right);
		// Unity only gets absolute anlge so you have to get the orientation from the cross product
		float absoluteAngle = Vector3.Angle (forwardProjectedToObj, vectorToGoalProjected);
		bool isAnglePositive = Vector3.Cross (vectorToGoalProjected, forwardProjectedToObj).x >= 0;
		return isAnglePositive ? absoluteAngle : -1 * absoluteAngle;
	}

	float GetDistToObj() { 
		return Vector3.Distance (this.selfObj.transform.localPosition, this.goal.transform.localPosition);
	}

}
