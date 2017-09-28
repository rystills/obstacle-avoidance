using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createFlocks : MonoBehaviour {
	public List<GameObject> paths;
	public GameObject chaserPrefab;
	public GameObject flockUnitPrefab;
	public float flockSpd;
	public float flockMaxSpd;

	void Start () {
		//create a chaser and flock unit on each path
		for (int i = 0; i < paths.Count; ++i) {
			//first we create the chaser
			Vector3 startPos = paths[i].GetComponent<pathPoints>().points[0];
			GameObject chaser = (GameObject)Instantiate(chaserPrefab, startPos, Quaternion.identity);
			//manually initialize the chaser's followPath component with the desired speed
			followPath fp = chaser.GetComponent<followPath>();
			fp.path = paths[i];
			fp.spd = flockSpd;
			fp.curPt = 0;
			fp.init();

			//now we create as many flock units as desired
			GameObject flockUnit = (GameObject)Instantiate(flockUnitPrefab, startPos, Quaternion.identity);
			followChaser fc = flockUnit.GetComponent<followChaser>();
			fc.leader = chaser;
			fc.spd = flockMaxSpd;
			fp.init();
		}	
	}
}
