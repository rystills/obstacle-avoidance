using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createFlocks : MonoBehaviour {
	public List<GameObject> paths;
	public GameObject chaserPrefab;
	public GameObject flockUnitPrefab;
	public float chaserSpd;
	public float flockSpd;

	void Start () {
		//create a chaser and flock unit on each path
		for (int i = 0; i < paths.Count; ++i) {
			//first we create the chaser
			Vector3 startPos = paths[i].GetComponent<pathPoints>().points[0];
			GameObject chaser = (GameObject)Instantiate(chaserPrefab, startPos, Quaternion.identity);
			//manually initialize the chaser's followPath component with the desired properties
			followPath fp = chaser.GetComponent<followPath>();
			fp.path = paths[i];
			fp.spd = chaserSpd;
			fp.curPt = 0;
			fp.init();

			for (int r = 0; r < 1; ++r) {
				//now we create as many flock units as desired
				GameObject flockUnit = (GameObject)Instantiate(flockUnitPrefab, startPos, Quaternion.identity);
				//initialize the flock unit's followChaser component with the desired properties 
				followChaser fc = flockUnit.GetComponent<followChaser>();
				fc.leader = chaser;
				fc.spd = flockSpd;
				fp.init();
				//ofset each flock unit by a different amount, so that they do not begin on top of each other
				Vector3 curPos = fc.transform.position;
				fc.transform.position = new Vector3(curPos.x + .4f*(5-r), curPos.y, curPos.z);
				//give each flock unit a tag so we can poll through them for collisions
				flockUnit.tag = "flockUnit";
			}
		}	
	}
}
