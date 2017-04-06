using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Neuro;
using Accord.Neuro.Learning;

public class NeuralNetwork {


	public double SigmoidAlpha;
	public int NumNodesFirstLayer;
	private ActivationNetwork network;

	private const float ALPHA = 0.5f;
	private const int NUM_LAYERS = 2;
	private const int NUM_NODES_FIRST_LAYER = 2;
	private const int NUM_NODES_OUTPUT_LAYER = 1;


	public NeuralNetwork(int numNodesFirstLayer = 2, 
		double sigmoidAlpha = 0.5) {
		this.SigmoidAlpha = sigmoidAlpha;
		this.NumNodesFirstLayer = numNodesFirstLayer;

	}

	public void TrainAINetwork(string inputPath) {
		//this.network = new ActivationNetwork(
			//new SigmoidFunction(this.SigmoidAlpha),
			//5,


		
	}

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
