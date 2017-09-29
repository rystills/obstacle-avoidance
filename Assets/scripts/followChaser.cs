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
		//poll through all other flock units to check for collisions
		GameObject[] others = GameObject.FindGameObjectsWithTag("flockUnit");
		for (int i = 0; i < others.Length; ++i) {
			//don't check for a collision with yourself
			if (others[i] == this.gameObject) {
				continue;
			}
			this.gameObject.SetActive(true);
			others[i].SetActive(true);
			GM.moveOutsideCollision(this.gameObject, others[i]);
		}
	}
}
