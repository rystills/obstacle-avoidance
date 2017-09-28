using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followChaser : MonoBehaviour {
	public GameObject leader;
	public float spd;
	
	void init() {
		//adopt a slightly faster speed than the leader so that we do not fall out of formation
		spd = leader.GetComponent<followPath>().spd + 1;
		transform.position = leader.transform.position;
	}

	void Update () {
		//approach the leader without passing it
		GM.lookAt2d(this.gameObject, leader.transform.position);
		float remDist = Vector3.Distance(transform.position, leader.transform.position);
		float moveDist = spd * Time.deltaTime;
		transform.Translate(Vector3.up * (moveDist > remDist ? remDist : moveDist));

	}
}
