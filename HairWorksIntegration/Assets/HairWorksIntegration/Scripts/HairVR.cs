using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
public class HairVR : MonoBehaviour {
	private int currentEye = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPreRender(){
			//Method called twice. Thus left eye == 0 and right eye == 1
			currentEye = 1 - currentEye;
		//if right eye
		if (currentEye == 1) {
			HairInstance.BeginRenderVR (this.GetComponent<Camera> ());
			foreach (var a in HairInstance.GetInstances()) {
				a.Render ();
			}
			HairInstance.EndRender ();
		}
	}
}
	