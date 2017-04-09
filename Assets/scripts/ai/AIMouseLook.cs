using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// Class for handling rotation/mouse input for the AI
/// </summary>
public class AIMouseLook : MouseLook {

	private NeuralNetwork network;

	/// <summary>
	/// Initializes a new instance of the <see cref="AIMouseLook"/> class.
	/// </summary>
	/// <param name="network">the INITIALIZED neural network to use to predict
	/// which direction to turn</param>
	public AIMouseLook(NeuralNetwork network) {
		this.network = network;
	}

	/// <summary>
	/// Overwrites the parent rotation input; basically this is called every frame to 
	/// get the inputs to the first person controller 
	/// </summary>
	protected override void GetRotationInput ()
	{
		GameStateSummary gss = new GameStateSummary();
		gss.InitializeFromGame(GameObject.Find ("AIController"), GameObject.Find("AIFirstPersonCharacter"));
		ObservedAction action = this.network.GetPredictedAction (gss);
		//Debug.Log (action.xRotInput);
		this.yInput = action.yRotInput;
		this.xInput = 0.0f;
	}

}
