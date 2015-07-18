﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class EventManager : MonoBehaviour {

	static EventManager instance;
	public int scriptLineNumber;
	string currentLine;

	#region Flags
	public static bool isInMainMenus;
	public static bool isWaitingForInput;
	public static bool isEndOfScript;
	public static bool isWaitingForTimer = false;
	#endregion
	#region Components
	public DialogPanel dialogPanel;
	public Text dialogText;
	public MusicManager musicManager;
	public ScriptContainer scriptContainer;
	#endregion	
	#region WAIT timers
	public static float waitTime;
	float timeWaited;
	#endregion

	#region Unity LifeCycle Events
	// Use this for initialization
	void Start () {
		if (instance != null && instance != this) {
			Destroy (gameObject);
		} else {
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}

		CheckIfInMainMenus ();
	}


	void OnLevelWasLoaded(int level) {
		CheckIfInMainMenus ();

		dialogPanel = FindObjectOfType<DialogPanel> ();
		dialogPanel.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isWaitingForTimer) {
			if(!isInMainMenus) {
				ReadEventLine ();
			}
		}
	}
	#endregion

	public void progressEvent() {
		print ("Trying to execute.");
			if (isEndOfScript) {
				//Move to next scene
				print ("End of script was true?");
			}
			else if (isWaitingForInput) {
			print ("I was waiting for input.");
				dialogText.text = "";
				dialogPanel.SetStateOfDialogue (dialogPanel.GetStateOfDialogue () + 1);;
				isWaitingForInput = false;
			} else {
			print ("Ending the line automatically.");
				dialogPanel.DisplayDialogLine (false);
			}
	}

	void CheckIfInMainMenus() {
		int[] MainMenus = {
			0,
			2
		};
		for (int i = 0; i < MainMenus.Length; i++) {
			if(MainMenus[i] == Application.loadedLevel) {
				isInMainMenus = true;
				break;
			}
			else if (i >= MainMenus.Length - 1) {
				isInMainMenus = false;
			}
		}
		Debug.Log ("I'm in a main menu: "+isInMainMenus);

	}

	public void LoadLevel(string name) {
		Application.LoadLevel (name);
	}

	public void LoadNextLevel() {
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	void ReadEventLine() {
		currentLine = scriptContainer.dialogLines [scriptLineNumber];
		bool lineContainsAKey = scriptContainer.DoesLineContainKey (currentLine);

		if (currentLine != null && currentLine != "" && lineContainsAKey) {
			Debug.Log (currentLine);
			string key = scriptContainer.GetKeyInLine (scriptContainer.dialogLines [scriptLineNumber]);
			ReactToKey(key);
		} else {
			Debug.Log ("Normal Dialog: "+currentLine);
			dialogPanel.UpdateSpeaker();
		}
		scriptLineNumber++;
	}

	void ReactToKey(string key) {
		switch(key) {
		case "#WAIT#":
			isWaitingForTimer = true;
			currentLine = scriptContainer.FilterKeyInLine("#WAIT#",currentLine);
			StartCoroutine(WaitTimer());
			break;
		case "#SPKR#":
			Debug.Log ("Replacing speaker...");
			currentLine = scriptContainer.FilterKeyInLine("#SPKR#",currentLine);
			break;
		case "#STRT_Dialog#":
			Debug.Log ("Bringing up the DialogPanel.");
			currentLine = scriptContainer.FilterKeyInLine("#STRT_Dialog#",currentLine);
			dialogPanel.gameObject.SetActive(true);
			break;
		case "#END_Dialog#":
			Debug.Log ("Closing the DialogPanel.");
			currentLine = scriptContainer.FilterKeyInLine("#END_Dialog#",currentLine);
			dialogPanel.gameObject.SetActive(false);
			break;
		}
	}

	IEnumerator WaitTimer() {
		while(timeWaited < waitTime) {
			timeWaited += Time.deltaTime;
			yield return 0;
		}
		Debug.Log ("I waited "+timeWaited+" seconds.");
		isWaitingForTimer = false;
		waitTime = 0;
		timeWaited = 0;
	}

}