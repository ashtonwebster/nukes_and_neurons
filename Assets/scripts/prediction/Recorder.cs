using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

public class Recorder {

	public float logPeriod = 0f;
	//private float timeSinceLog = 0f;
	private System.IO.StreamWriter file;
			
	public void InitializeFile()  {
		this.file = new System.IO.StreamWriter (System.Environment.CurrentDirectory + "/training_data/test2.txt");
		// combine the features header and the target movement header
		file.WriteLine (GameStateSummary.GetHeader () + ObservedAction.GetHeader ());
	}

	/// <summary>
	/// Writes 
	/// </summary>
	/// <param name="debug">If set to <c>true</c>, write all recorded </param>
	public void WriteToFile(bool debug=false) { 
		TrainingPair trainingPair = new TrainingPair ();
		trainingPair.InitializeFromGame (GameObject.Find("FPSController"), GameObject.Find("FirstPersonCharacter"));

		// write to file with features and target movements
		if (debug) { 
			Debug.Log (trainingPair.ToString ());
		}
		file.WriteLine (trainingPair.ToString());
		file.Flush ();
	}

}