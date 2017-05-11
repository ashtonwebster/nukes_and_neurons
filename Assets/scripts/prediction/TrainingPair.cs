using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPair  {
	public ObservedAction observedAction;
	public GameStateSummary gameStateSummary;
	private GameObject goal;

	public TrainingPair(GameObject goalParam) 
	{ 
		this.goal = goalParam;

	}

	public void InitializeFromGame(GameObject selfObj, GameObject childObj) { 
		this.observedAction = new ObservedAction ();
		this.observedAction.InitializeFromGame ();

		this.gameStateSummary = new GameStateSummary (this.goal);
		this.gameStateSummary.InitializeFromGame (selfObj, childObj);
	}

	public void InitializeFromSaved(string s) {
		string[] splits = s.Split ('|');

		this.gameStateSummary = new GameStateSummary (this.goal);
		this.gameStateSummary.InitializeFromSaved (splits [0]);

		this.observedAction = new ObservedAction ();
		this.observedAction.InitializeFromSaved (splits [1]);
	}
		

	public override string ToString ()
	{
		return string.Format ("{0}|{1}", this.gameStateSummary.ToString(), this.observedAction.ToString());
	}
}
