﻿using UnityEngine;
using System.Collections;

public class ScriptContainer : MonoBehaviour {

	private TextAsset currentScript;
	private string pathToDialogScripts = "Event Scripts/";
	private string[] dialogKeys = {
		"~playerName~",
		"#SPKR#",
		""
	};

	public string[] dialogLines;
	public string currentSpeaker;
	static ScriptContainer instance = null;

	public string GetCurrentSpeaker() {
		return currentSpeaker;
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (gameObject);
		} else {
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	}

	void OnLevelWasLoaded(int level) {
		if (level != 2 && level != 0) {
			LoadScript (level);
			ParseScriptIntoLines ();
			FilterScriptForKeys (dialogLines);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadScript(int level) {
		switch (level) {
		case 1:
			currentScript = Resources.Load(pathToDialogScripts+"Introduction") as TextAsset;
			break;
		}
	}

	private void ParseScriptIntoLines() {
		dialogLines = currentScript.text.Split ('\n');
	}

	//TODO Create more filters in order to correct:
	//	Replaceing keys with player created name
	private string[] FilterScriptForKeys (string[] scriptLines) {
		//For each dialog line
		for(int i = 0; i < scriptLines.Length; i++) {
			//Scan for each key
			foreach (string key in dialogKeys) {
				//If it conatins a key check which key it is and filter via
				//Replace or Remove and do something.

				/*~playerName~
				 * 		Key is replaced with entered protagonist's name
				 * */
				if(scriptLines[i].Contains(key)){
					if (key == dialogKeys[0]) {
						//scriptLines[i] = scriptLines[i].Replace(key, charName);
					}
				}
			}
		}
		return scriptLines;
	}

	public string FilterKeysInLine(string line) {
		return line;
	}

	public string FilterKeyInLine(string key, string line) {

		if (key == dialogKeys [1]) {//#SPKR 
			if (line.Contains (dialogKeys [1])) {
				//Remove the key
				line = line.Replace (dialogKeys [1], "");
				//Change the talker to the found speaker
				currentSpeaker = line.Substring (0, line.IndexOf (':'));
				//Remove the speaker
				line = line.Replace (currentSpeaker + ":", "");
			}
		} else {
			Debug.LogError(key+" is not valid a valid dialog key.");
		}
		return line;
	}
}
