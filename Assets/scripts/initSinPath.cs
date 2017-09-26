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

	//setup a reference to this object's points compoment, so we can insert the points produced by our sin curve
	private pathPoints PP;

	//Create a list of points which constitute a sin path with the desired fidelity and number of iterations
	void Start () {
		//initialize our line renderer
		LR = gameObject.AddComponent<LineRenderer>();

		//grab a reference to the points component
		PP = gameObject.GetComponent<pathPoints>();

		//calculate the camrea extents so we can fine-tune the path layout
		float hHeight = Camera.main.orthographicSize;
		float hWidth = hHeight * Screen.width / Screen.height;

		//create some test points
		PP.points.Add(new Vector3(0, 0, 0));
		PP.points.Add(new Vector3(hWidth, hHeight, 0));
		PP.points.Add(new Vector3(hWidth, -hHeight, 0));

		//copy our points into the line renderer so that they appear on-screen
		LR.SetVertexCount(PP.points.Count);
		LR.SetPositions(PP.points.ToArray());		
	}
}
