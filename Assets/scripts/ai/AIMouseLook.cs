using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// Class for handling rotation/mouse input for the AI
/// </summary>
public class AIMouseLook : MouseLook {

	private NeuralNetwork network;
	private GameObject goal;

	/// <summary>
	/// Initializes a new instance of the <see cref="AIMouseLook"/> class.
	/// </summary>
	/// <param name="network">the INITIALIZED neural network to use to predict
	/// which direction to turn</param>
	public AIMouseLook(NeuralNetwork network, GameObject goalParam) {
		this.network = network;
		//this.MaximumX = 30;
		//this.MinimumX = 30;
		this.goal = goalParam;
	}

	/// <summary>
	/// Overwrites the parent rotation input; basically this is called every frame to 
	/// get the inputs to the first person controller 
	/// </summary>
	protected override void GetRotationInput (bool usingJoystick)
	{
		GameStateSummary gss = new GameStateSummary(this.goal);
		gss.InitializeFromGame(GameObject.Find("AIController(Clone)"), GameObject.Find("AIFirstPersonCharacter"));
		ObservedAction action = this.network.GetPredictedAction (gss);
		// uncomment to print current action as CSV
		//Debug.Log (action);
		this.yInput = action.yRotInput; //* 5.0f;
		//this.xInput = action.xRotInput; //* 5.0f;
		this.xInput = 0f;
	}

}
