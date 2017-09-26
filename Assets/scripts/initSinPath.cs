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

	//Create a list of points which constitute a sin path with the desired fidelity and number of cycles
	void Awake () {
		//initialize our line renderer
		LR = gameObject.AddComponent<LineRenderer>();

		//grab a reference to the points component
		PP = gameObject.GetComponent<pathPoints>();

		//calculate the camera extents so we can fine-tune the path layout
		float hHeight = Camera.main.orthographicSize;
		float hWidth = hHeight * Screen.width / Screen.height;

		//create the points on our sin curve (remember y = asin(b(x - c)) + d)
		for (int r = 0; r < numCycles; ++r) {
			for (int i = 0; i < pointsPerCycle; ++i) {
				//we can calculate x by shifting along the current period by the current point
				float x = transform.position.x + 2 * Mathf.PI * (period * (r+i / (float)pointsPerCycle));
				//once we have x, we plug that into our sin function to get y
				float y = transform.position.y + amplitude * Mathf.Sin(1/period * x);
				PP.points.Add(new Vector3(x,y,0));
			}	
		}

		//specify the lind renderer settings
		LR.startWidth = .5f;
		LR.endWidth = .5f;

		//copy our points into the line renderer so that they appear on-screen
		LR.numPositions = PP.points.Count;
		LR.SetPositions(PP.points.ToArray());		
	}
}
