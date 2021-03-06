﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class followChaser : MonoBehaviour {
	public GameObject leader;
	public float maxSpd;
	public float spd;
	public float radius;
	public float coneLength;
	public float coneArc;
	public int coneHitsThisFrame = 0;
	public GameObject closestPredictedUnit = null;

	private float? colPredX = null;
	private float? colPredY = null;

	//keep track of a timer for evading predicted collisions
	public float evadeTimer = 0;

	//we will use a line renderer to display our cone check and collision prediction
	private LineRenderer coneLR;
	private LineRenderer predLR;
	private GameObject colPredDrawer;

	public void init() {
		//adopt a slightly faster speed than the leader so that we do not fall out of formation
		spd = maxSpd;
		transform.position = leader.transform.position;

		//instantiate a child object who will be responsible for rendering our predicted collision location when necessary
		colPredDrawer = new GameObject();
		colPredDrawer.name = "collision prediction renderer";
		colPredDrawer.transform.parent = this.transform;

		//initialize our line renderers and set their render order to be in front of the path but behind the UI
		coneLR = gameObject.AddComponent<LineRenderer>();
		coneLR.sortingOrder = 1;
		predLR = colPredDrawer.AddComponent<LineRenderer>();
		predLR.sortingOrder = 3;

		//set some default values for the line renderer settings
		coneLR.startWidth = .02f;
		coneLR.endWidth = .02f;
		predLR.startWidth = .15f;
		predLR.endWidth = .15f;

		coneLR.material = new Material(Shader.Find("GUI/Text Shader"));
		coneLR.material.color = Color.yellow;
		predLR.material = new Material(Shader.Find("GUI/Text Shader"));
		predLR.material.color = this.leader.GetComponent<followPath>().path.GetComponent<pathPoints>().debugColor;

		coneLR.numPositions = 4;
		predLR.numPositions = 20;
	}

	void Update() {
		//get the distance from the path leader to avoid moving too far
		float remDist = Vector3.Distance(transform.position, leader.transform.position);
		float moveDist = spd * Time.deltaTime;

		int iter = 0;
		if (GM.mode == "cone check") {
			spd = maxSpd;
			colPredX = colPredY = null;
			coneLR.enabled = true;
			GM.lookAt2d(gameObject, leader.transform.position);
			//check if looking at the leader resulted in any cone collisions
			GM.avoidConeCollisions(gameObject, GameObject.FindGameObjectsWithTag("flockUnit").Where
				(x => x.GetComponent<followChaser>().leader != this.leader).ToList());

			//if looking at the leader resulted in one or more cone checks, start rotating right until we have resolved all collisions
			//cap iterations at 100 as a failsafe, in case we find ourselves surrounded on all directions
			while (coneHitsThisFrame != 0 && ++iter <= 100) {
				transform.rotation *= Quaternion.Euler(0, 0, coneHitsThisFrame * 4);
				GM.avoidConeCollisions(gameObject, GameObject.FindGameObjectsWithTag("flockUnit").Where
				(x => x.GetComponent<followChaser>().leader != this.leader).ToList());
			}
		}
		else if (GM.mode == "collision prediction") {
			//don't do anything if we are in the process of evading the predicted collision location
			if (evadeTimer > 0) {
				evadeTimer -= Time.deltaTime;
			}
			else if (this.leader.GetComponent<followPath>().curPt <= 8) {
				//if we are on our way back to the start of the path, don't worry about collisions as we will fly in line anyway
				GM.lookAt2d(gameObject, leader.transform.position);
				colPredX = colPredY = null;
			}
			else {
				spd = maxSpd;
				colPredX = colPredY = null;
				GM.lookAt2d(gameObject, leader.transform.position);
				coneLR.enabled = false;
				//take into account our speed and the speed of those around us to predict the closest future 'collision'
				Vector3 colPos = GM.predictNearestCollision(gameObject, GameObject.FindGameObjectsWithTag("flockUnit").Where
					(x => x.GetComponent<followChaser>().leader != this.leader).ToList());

				//if we didn't predict a collision with anybody, we have nothing more to do
				if (closestPredictedUnit != null) {
					float colDist = Vector3.Distance(colPos, transform.position);
					//ignore collisions that are too far away
					if (colDist < 2) {
						colPredX = colPos.x;
						colPredY = colPos.y;

						//rotate a bit to dodge the most immediate collision
						transform.rotation *= Quaternion.Euler(0, 0, 18);
						evadeTimer = 2.2f;
						spd = leader.GetComponent<followPath>().spd;
					}
				}
			}
		}

		//now that we've finalized our angle to avoid collisions, we can safely move forward
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
		coneLR.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
		coneLR.SetPosition(3, new Vector3(transform.position.x, transform.position.y, transform.position.z));

		coneLR.material.color = (iter > 0 ? Color.red : Color.yellow);

		//get the two additional points of our cone
		Quaternion curRot = transform.rotation;
		Vector3 curPos = transform.position;
		for (int i = 0; i < 2; ++i) {
			//set our rotation to half of our arc radius and move forward by our arc length
			transform.rotation = curRot * Quaternion.Euler(0, 0, coneArc/2 * (i == 0 ? -1 : 1));
			transform.Translate(Vector3.up * coneLength);
			//add this new position to the line renderer
			coneLR.SetPosition(i+1, new Vector3(transform.position.x, transform.position.y, transform.position.z));
			//move back to our starting position
			transform.position = curPos;
		}

		//move our collision prediction line renderer into place, and calculate some points around it
		if (colPredX != null) {
			predLR.enabled = true;
			for (int i = 0; i < predLR.numPositions; ++i) {
				float angle = (float)i / (predLR.numPositions - 2) * 2 * Mathf.PI;
				float px = colPredX.Value + Mathf.Cos(angle) * .06f;
				float py = colPredY.Value + Mathf.Sin(angle) * .06f;
				predLR.SetPosition(i, new Vector3(px, py, 0));
			}
		}
		else {
			predLR.enabled = false;
		}

		//reset our rotation now that we've located all of the cone points
		transform.rotation = curRot;
	}
}
