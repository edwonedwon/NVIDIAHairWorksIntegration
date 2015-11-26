using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
		if (Input.GetKeyDown (KeyCode.Escape)) {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

	
	}
}
