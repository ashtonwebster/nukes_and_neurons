﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System.IO;
using System.Linq;

public class NeuralNetwork {


	public double SigmoidAlpha;
	public int NumNodesFirstLayer;
	private ActivationNetwork network;
	private ISupervisedLearning teacher;

	private const float ALPHA = 0.5f;

	public NeuralNetwork(int numNodesFirstLayer = 2, 
		double sigmoidAlpha = 0.5) {
		this.SigmoidAlpha = sigmoidAlpha;
		this.NumNodesFirstLayer = numNodesFirstLayer;

		this.InitNetwork ();
	}

	public void InitNetwork() {
		// currently network has 1 input, 5 hidden nodes, and 1 output
		this.network = new ActivationNetwork(new BipolarSigmoidFunction(ALPHA),
			2, 5, 2);
		
		this.teacher = new ParallelResilientBackpropagationLearning(this.network);
	}

	public void TrainAINetwork(string inputPath) {
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
			TrainingPair tp = new TrainingPair ();
			line = line.Trim ();
			tp.InitializeFromSaved (line);
			trainingList.Add(tp);
			if (tp.observedAction.xRotInput != 0.0f || tp.observedAction.yRotInput != 0.0f) {
				// only add non-zero examples for now
				input.Add (new List<double> () { tp.gameStateSummary.XZAngleToObj, tp.gameStateSummary.YZAngletoObj });
				output.Add (new List<double> () { tp.observedAction.yRotInput, tp.observedAction.xRotInput } );
			}
		}
		file.Close ();
		Debug.Log (string.Format ("Training List Length: {0}", input.Count));

		for (int i = 0; i < 1000; i++) { 
			double error = this.teacher.RunEpoch (NestedListToArray(input), NestedListToArray(output));
			if (i % 50 == 0) { 
				Debug.Log (string.Format("iteration: {0}, Error: {1}", i, error));
			}
		}


//		TrainingPair tp1 = new TrainingPair();
//		tp1.InitializeFromSaved ("70.62878,113.9955,3.618909|0,0,-0.2,-0.75,0,0");
//		Debug.Log("Original: 70.62878,113.9955,3.618909|0,0,-0.2,-0.75,0,0");
//		Debug.Log ("Parsed : " + tp1.ToString());
	}

	public double[][] NestedListToArray(List<List<double>> list) { 
		return list.Select (Enumerable.ToArray).ToArray();	
	}

	public ObservedAction GetPredictedAction(GameStateSummary gss) { 
		double[] input = { gss.XZAngleToObj, gss.YZAngletoObj } ;
		double[] output = this.network.Compute(input);
		ObservedAction action = new ObservedAction ();
		action.yRotInput = (float) output [0];
		action.xRotInput = (float)output [1];
		return action;
	}

	/// <summary>
	/// Test network for illustrative purposes
	/// </summary>
	public void RunXORNetwork(){
		// initialize input and output values
		double[][] input = null;
		double[][] output = null;
		double error = -1;

		input = new double[4][] {
			new double[] {0, 0},
			new double[] {0, 1},
			new double[] {1, 0},
			new double[] {1, 1}
		};
		output = new double[4][] {
			new double[] {0},
			new double[] {1},
			new double[] {1},
			new double[] {0}
		};

		ActivationNetwork xornetwork = new ActivationNetwork(new SigmoidFunction(ALPHA),
			input[0].Length, this.NumNodesFirstLayer, output[0].Length);

		var teacher = new ParallelResilientBackpropagationLearning(xornetwork);

		for (int i = 0; i < 500; i++) {
			error = teacher.RunEpoch (input, output);

		}
		Debug.Log (error);
		Debug.Log (xornetwork.Compute (input [0])[0]);

	}

}