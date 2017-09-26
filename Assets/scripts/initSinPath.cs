using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initSinPath : MonoBehaviour {
	//setup some public properties to be filled in the Editor
	public int pointsPerCycle;
	public int numCycles;
	public float amplitude;
	public float period;

	//we will need a line renderer to draw our path
	private LineRenderer LR;

	//Create a list of points which constitute a sin path with the desired fidelity and number of iterations
	void Start () {
		//start by initializing our line-renderer
		LR = gameObject.AddComponent<LineRenderer>();
		LR.SetPosition(0, new Vector3(50, 50, 50));
		LR.SetPosition(0, new Vector3(100, 100, 100));
		
	}
}
