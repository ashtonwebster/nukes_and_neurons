using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
public class NeuralNetwork {


	public double SigmoidAlpha;
	public int NumNodesFirstLayer;
	private ActivationNetwork network;
	private ISupervisedLearning teacher;
	private GameObject goal;

	private const float ALPHA = 0.5f;

	public NeuralNetwork(GameObject goalParam, int numNodesFirstLayer = 2, 
		double sigmoidAlpha = 0.5) {
		this.SigmoidAlpha = sigmoidAlpha;
		this.NumNodesFirstLayer = numNodesFirstLayer;
		this.goal = goalParam;

		this.InitNetwork ();
	}

	public void InitNetwork() {
		// currently network has 3 input, 5 hidden nodes, and 7 output
		this.network = new ActivationNetwork(new BipolarSigmoidFunction(ALPHA),
			3, Config.Instance.node["num_hidden_nodes"].AsInt, 7);
		
		this.teacher = new ParallelResilientBackpropagationLearning(this.network);
	}

	public void TrainAINetwork(string inputPath, string serializePath=null) {
		if (serializePath != null && File.Exists(serializePath)) {
			using (FileStream fs = new FileStream (serializePath, FileMode.Open)) {
				this.network = (ActivationNetwork) new BinaryFormatter().Deserialize(fs);
				return;
			}
		}

		List<TrainingPair> trainingList = new List<TrainingPair>();
		System.IO.StreamReader file = 
			new System.IO.StreamReader (inputPath);
		string line;
		List<List<double>> input = new List<List<double>>();
		List<List<double>> output = new List<List<double>> ();
		// skip the header (first line);
		file.ReadLine ();
		while((line = file.ReadLine()) != null)
		{
			TrainingPair tp = new TrainingPair (this.goal);
			line = line.Trim ();
			tp.InitializeFromSaved (line);
			trainingList.Add(tp);
			if (tp.observedAction.xRotInput != 0.0f || tp.observedAction.yRotInput != 0.0f || tp.observedAction.forwardPan != 0.0f
				|| tp.observedAction.horizontalPan != 0.0f || tp.observedAction.fireButtonDown != 0.0f) {
				// only add non-zero examples for now
				input.Add (new List<double> () { 
					tp.gameStateSummary.XZAngleToObj, 
					tp.gameStateSummary.YZAngletoObj, 
					tp.gameStateSummary.distToObj});
				output.Add (new List<double> () { 
					tp.observedAction.yRotInput, 
					tp.observedAction.xRotInput,
					tp.observedAction.horizontalPan,
					tp.observedAction.forwardPan,
					tp.observedAction.fireButtonDown,
					tp.observedAction.sprintButtonDown,
					tp.observedAction.jumpButtonDown
				} );
			}
		}
		file.Close ();
		Debug.Log (string.Format ("Training List Length: {0}", input.Count));

		for (int i = 0; i < Config.Instance.node["training_epochs"].AsInt ; i++) { 
			double error = this.teacher.RunEpoch (NestedListToArray(input), NestedListToArray(output));
			if (i % 50 == 0) { 
				Debug.Log (string.Format("iteration: {0}, Error: {1}", i, error));
			}
		}

		if (serializePath != null) {
			using (FileStream fs = new FileStream(serializePath, FileMode.Create))
			{
				new BinaryFormatter().Serialize(fs, this.network);
			}    
		}
	}

	public double[][] NestedListToArray(List<List<double>> list) { 
		return list.Select (Enumerable.ToArray).ToArray();	
	}

	public ObservedAction GetPredictedAction(GameStateSummary gss) { 
		double[] input = { gss.XZAngleToObj, gss.YZAngletoObj, gss.distToObj } ;
		double[] output = this.network.Compute(input);
		ObservedAction action = new ObservedAction ();
		action.yRotInput = (float) output [0];
		action.xRotInput = (float)output [1];
		action.horizontalPan = (float)output [2];
		action.forwardPan = (float)output [3];
		action.fireButtonDown = (float)output [4];
		action.sprintButtonDown = (float)output [5];
		action.jumpButtonDown = (float)output [6];
		return action;
	}

}
