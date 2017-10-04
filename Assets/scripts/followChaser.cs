using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class followChaser : MonoBehaviour {
	public GameObject leader;
	public float spd;
	public float radius;
	public float coneLength;
	public float coneArc;
	public int coneHitsThisFrame = 0;
	private float stepAngVel = .01f;

	//we will line renderer to display our cone check visually
	private LineRenderer LR;

	public void init() {
		//adopt a slightly faster speed than the leader so that we do not fall out of formation
		spd = leader.GetComponent<followPath>().spd + 1;
		transform.position = leader.transform.position;

		//initialize our line renderer and set its render order to be in front of the path display
		LR = gameObject.AddComponent<LineRenderer>();
		LR.sortingOrder = 1;

		//set some default values for the line renderer settings
		LR.startWidth = .02f;
		LR.endWidth = .02f;

		LR.material = new Material(Shader.Find("Unlit/Color"));
		LR.material.color = Color.yellow;

		LR.numPositions = 4;
	}

	void Update() {
		//get the distance from the path leader to avoid moving too far
		float remDist = Vector3.Distance(transform.position, leader.transform.position);
		float moveDist = spd * Time.deltaTime;

		coneHitsThisFrame = 0;
		while (coneHitsThisFrame == 0) {
			//get our current and desired rotations, as well as the total desired change in angle
			Quaternion startRot = transform.rotation;
			GM.lookAt2d(gameObject, leader.transform.position);

			GM.avoidConeCollisions(gameObject, GameObject.FindGameObjectsWithTag("flockUnit").Where
	(x => x.GetComponent<followChaser>().leader != this.leader).ToList());
			if (coneHitsThisFrame == 0) {
				break;
			}

			Quaternion wantRot = transform.rotation;
			float rotChange = Quaternion.Angle(startRot, wantRot);

			//rotate at most maxAngVel to face wantRot
			transform.rotation = Quaternion.Lerp(startRot, wantRot, stepAngVel);

			//perform a cone check for collisions with objects not on our path, adjusting our rotation to avoid them
			GM.avoidConeCollisions(gameObject, GameObject.FindGameObjectsWithTag("flockUnit").Where
				(x => x.GetComponent<followChaser>().leader != this.leader).ToList());
		}

		transform.Translate(Vector3.up * (moveDist > remDist ? remDist : moveDist));

		//poll through all other flock units to check for collisions
		GameObject[] others = GameObject.FindGameObjectsWithTag("flockUnit");
		for (int i = 0; i < others.Length; ++i) {
			//don't check for a collision with yourself
			if (others[i] == gameObject) {
				continue;
			}
			GM.moveOutsideCollision(gameObject, others[i]);
		}

		//once we've finalized our position and orientation for the frame, update our line renderer
		//start by setting the first and last points of the cone render to our starting position
		LR.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		LR.SetPosition(3, new Vector3(transform.position.x, transform.position.y, transform.position.z));

		LR.material.color = (coneHitsThisFrame > 0 ? Color.red : Color.yellow);

		//get the two additional points of our cone
		Quaternion curRot = transform.rotation;
		Vector3 curPos = transform.position;
		for (int i = 0; i < 2; ++i) {
			//set our rotation to half of our arc radius and move forward by our arc length
			transform.rotation = curRot * Quaternion.Euler(0, 0, coneArc/2 * (i == 0 ? -1 : 1));
			transform.Translate(Vector3.up * coneLength);
			//add this new position to the line renderer
			LR.SetPosition(i+1, new Vector3(transform.position.x, transform.position.y, transform.position.z));
			//move back to our starting position
			transform.position = curPos;
		}

		//reset our rotation now that we've located all of the cone points
		transform.rotation = curRot;
	}
}
