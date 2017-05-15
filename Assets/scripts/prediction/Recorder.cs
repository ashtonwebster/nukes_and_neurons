using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

public class Recorder {

	public float logPeriod = 0f;
	//private float timeSinceLog = 0f;
	public System.IO.StreamWriter file;
	public GameObject goal;
	private GameObject self;
			
	public Recorder(GameObject goalParam, GameObject selfParam){
		this.goal = goalParam;
		this.self = selfParam;
	}

	/// <summary>
	/// Initializes file in .../training_data/filename
	/// </summary>
	/// <param name="filename">Filename.</param>
	public void InitializeFile(string filename)  {
		this.file = new System.IO.StreamWriter (System.Environment.CurrentDirectory + "/training_data/" + filename);

		// combine the features header and the target movement header
		file.WriteLine (GameStateSummary.GetHeader () + ObservedAction.GetHeader ());
	}

	/// <summary>
	/// Writes 
	/// </summary>
	/// <param name="debug">If set to <c>true</c>, write all recorded </param>
	public void WriteToFile(bool debug=false) { 
		TrainingPair trainingPair = new TrainingPair (this.goal);

		trainingPair.InitializeFromGame (this.self, this.self.transform.FindChild ("FirstPersonCharacter").gameObject);

		// write to file with features and target movements
		if (debug) { 
			//debug.Log (trainingPair.ToString ());
		}
		file.WriteLine (trainingPair.ToString());
		file.Flush ();
	}

}