using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager class for storing global functions
public class GM : MonoBehaviour {
	private GM m_Instance;
	public GM Instance { get { return m_Instance; } }

	void Awake() {
		m_Instance = this;
	}

	void OnDestroy() {
		m_Instance = null;
	}

	/**
	 * move a GameObject backwards until it is no longer colliding with the other specified GameObject
	 * @param a: the object to move backwards
	 * @param b: the object to check for collisions against
	 * @returns whether a was initially colliding with b (true) or not (false)
	 */
	public static bool moveOutsideCollision(GameObject a, GameObject b) {
		Bounds ab = a.GetComponent<Renderer>().bounds;
		Bounds bb = b.GetComponent<Renderer>().bounds;
		bool initialCollision = false;
		while (ab.Intersects(bb)) {
			initialCollision = true;
			Vector3 pos = a.transform.position;
			float dir = a.transform.rotation.eulerAngles.z;
			Debug.Log(pos.x + Mathf.Cos(dir) * 10);
			a.transform.position = new Vector3(pos.x + Mathf.Cos(dir) * 10, pos.y + pos.y * Mathf.Sin(dir) * 10, pos.z);
			return true;
		}
		return initialCollision;
	}

	/**
	 *	rotate to face towards the desired point
	 *	@param go: the game object to rotate
	 *	@param loc: the location in space towards which we should face
	 */
	public static void lookAt2d(GameObject go, Vector3 loc) {
		Vector3 diff = loc - go.transform.position;
		diff.Normalize();

		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		go.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
	}

	/**
	 * Overloads lookAt2d to support a Vector2d location
	 */
	public static void lookAt2d(Vector2 loc) {
		lookAt2d(new Vector3(loc.x, loc.y, 0));
	}
}