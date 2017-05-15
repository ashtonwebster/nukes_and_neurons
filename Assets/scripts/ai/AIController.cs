using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AIController : Player {

	//this may ignore the public settings...
	public AIMouseLook _m_AIMouseLook;
	private NeuralNetwork network;

	public GameObject goalState;

	protected override void Start() { 
		// train the model based on previous input
		this.network = new NeuralNetwork(this.goalState);
//		network.TrainAINetwork (inputPath: System.Environment.CurrentDirectory + "/training_data/training_everything.txt",
//			serializePath: System.Environment.CurrentDirectory + "/training_data/serialized_model.bin");
		string trainFile = Config.Instance.node["neural_train_file"].Value;
		string serializedFile = Config.Instance.node ["neural_serialized_file"].Value;
		network.TrainAINetwork (inputPath: System.Environment.CurrentDirectory + "/training_data/" + trainFile,
			serializePath: System.Environment.CurrentDirectory + "/training_data/" + serializedFile);
		_m_AIMouseLook = new AIMouseLook (network, this.goalState);
		
		base.Start ();
	}

	protected override MouseLook m_MouseLook {
		get {
			return _m_AIMouseLook;
		}
	}

	protected override void GetInput(out float speed) {
		// Read input
		//manually setting
		//float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		GameStateSummary gss = new GameStateSummary(this.goalState);
		gss.InitializeFromGame(this.gameObject, GameObject.Find("AIFirstPersonCharacter"));
		ObservedAction action = this.network.GetPredictedAction (gss);
		Debug.Log (string.Format ("{0}|{1}", gss.PrettyPrint(), action.PrettyPrint()));
		this.isJumping = action.jumpButtonDown > 0.5; //TODO: tune these magic thresholds
		m_IsWalking = !(action.sprintButtonDown > 0.5); 
		float horizontal = action.horizontalPan;
		//float vertical = CrossPlatformInputManager.GetAxis("Vertical");
		float vertical = action.forwardPan;
		this.isFiring = (action.fireButtonDown > 0.15f);

		bool waswalking = this.m_IsWalking;



		// set the desired speed to be walking or running
		speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
		m_Input = new Vector2(horizontal, vertical);

		// normalize input if it exceeds 1 in combined length:
		if (m_Input.sqrMagnitude > 1)
		{
			m_Input.Normalize();
		}

		// handle speed change to give an fov kick
		// only if the player is going to a run, is running and the fovkick is to be used
		if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
		{
			StopAllCoroutines();
			StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
		}
	}
}
