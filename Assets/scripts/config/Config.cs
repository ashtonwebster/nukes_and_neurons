using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

/// <summary>
/// Config is a singleton and should be instantiated using Config.Instance.
/// It references the file relative to the root of the project at <project>/config/CONFIG.json.
/// </summary>
public class Config {

	private static Config _instance;

	public static Config Instance {
		get {
			if (_instance == null) {
				_instance = new Config ();
			}
			return _instance;
		}
	}

	public JSONNode node;

	private Config () { 
		string jsonString = File.ReadAllText (System.Environment.CurrentDirectory + "/config/CONFIG.json");
		this.node = JSON.Parse(jsonString);
	}

}
