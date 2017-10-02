using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followChaser : MonoBehaviour {
	public GameObject leader;
	public float spd;
	public float radius;
	public float coneLength;
	public float coneArc;

	//we will line renderer to display our cone check visually
	private LineRenderer LR;

	public void init() {
		//adopt a slightly faster speed than the leader so that we do not fall out of formation
		spd = leader.GetComponent<followPath>().spd + 1;
		transform.position = leader.transform.position;

		//initialize our line renderer
		LR = gameObject.AddComponent<LineRenderer>();

		//set some default values for the line renderer settings
		LR.startWidth = .02f;
		LR.endWidth = .02f;

		LR.material = new Material(Shader.Find("Unlit/Color"));
		LR.material.color = Color.yellow;

		LR.numPositions = 4;
	}

	void Update() {
		//approach the leader without passing it
		GM.lookAt2d(this.gameObject, leader.transform.position);
		float remDist = Vector3.Distance(transform.position, leader.transform.position);
		float moveDist = spd * Time.deltaTime;
		transform.Translate(Vector3.up * (moveDist > remDist ? remDist : moveDist));

		//poll through all other flock units to check for collisions
		GameObject[] others = GameObject.FindGameObjectsWithTag("flockUnit");
		for (int i = 0; i < others.Length; ++i) {
			//don't check for a collision with yourself
			if (others[i] == this.gameObject) {
				continue;
			}
			GM.moveOutsideCollision(this.gameObject, others[i]);
		}

		//once we've finalized our position and orientation for the frame, update our line renderer
		//start by setting the first and last points of the cone render to our starting position
		LR.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		LR.SetPosition(3, new Vector3(transform.position.x, transform.position.y, transform.position.z));

		//get the two additional points of our cone
		Quaternion curRot = transform.rotation;
		Vector3 curPos = transform.position;
		for (int i = 0; i < 2; ++i) {
			//set our rotation to half of our arc radius and move forward by our arc length
			transform.rotation = curRot * Quaternion.Euler(0, 0, coneArc * (i == 0 ? -1 : 1));
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
