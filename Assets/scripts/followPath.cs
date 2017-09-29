using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPath : MonoBehaviour {
	//store a reference to the path we will follow, and its points component
	public GameObject path;
	private pathPoints PP;
	public float spd;
	public int curPt = 0;

	public void init() {
		//grab the point list from our path and move to point 0
		PP = path.GetComponent<pathPoints>();
		transform.position = PP.points[curPt];
		GM.lookAt2d(gameObject,PP.points[curPt + 1]);
	}

	void Update() {
		//move forward on the path, updating direction at each point
		float dist = spd * Time.deltaTime;
		while (dist > 0) {
			//get the distance to the next point to see if we can reach it this step
			float remDist = Vector3.Distance(transform.position, PP.points[curPt+1]);
			if (dist >= remDist) {
				//we can reach the next point; move there and subtract the distance from our remaining movement speed
				transform.position = PP.points[++curPt];
				dist -= remDist;
				//wrap around if we reached the final point
				if (curPt == PP.points.Count-1) {
					curPt = -1;
				}
				//update our direction to face the next point
				GM.lookAt2d(gameObject,PP.points[curPt + 1]);
			}
			else {
				//we can't reach the next point; simply move forward as much as we can
				transform.Translate(Vector3.up * dist);
				dist = 0;
			}
		}
	}
}
