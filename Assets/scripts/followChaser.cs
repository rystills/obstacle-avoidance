using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followChaser : MonoBehaviour {
	public GameObject leader;
	public float spd;
	public float radius;
	public float coneLength;
	public float coneArc;

	void init() {
		//adopt a slightly faster speed than the leader so that we do not fall out of formation
		spd = leader.GetComponent<followPath>().spd + 1;
		transform.position = leader.transform.position;

		//create a triangle representing this unit's cone (for collision checking)
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		//create the verts, tris, and uvs necessary for our triangle
		Vector3[] verts = new Vector3[3];
		verts[0] = new Vector3(0,-coneArc/2,0);
		verts[1] = new Vector3(coneLength,0,0);
		verts[2] = new Vector3(0,coneArc/2,0);
		mesh.vertices = verts;

		int[] tris = new int[3];
		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		mesh.triangles = tris;

		Vector2[] uvs = new Vector2[3];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(1, 0);
		uvs[2] = new Vector2(1, 1);
		mesh.uv = uvs;
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
			GM.moveOutsideCollision(this.gameObject, others[i]);
		}
	}
}
