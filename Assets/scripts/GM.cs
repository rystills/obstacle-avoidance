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
		if (ab.Intersects(bb)) {
			initialCollision = true;
			Vector3 origAngle = a.transform.eulerAngles;
			a.transform.eulerAngles += 180f * Vector3.up;
			int i = 0;
			while (ab.Intersects(bb)) {
				//a.transform.Translate(1 * Vector3.up);
				float dir = a.transform.rotation.eulerAngles.z;
				ab.center = new Vector3(ab.center.x + Mathf.Cos(dir) * .01f, ab.center.y + ab.center.y * Mathf.Sin(dir) * .01f, ab.center.z);
				++i;
				/*if (++i == 50) {
					Debug.Log("oh well");
					a.transform.eulerAngles = origAngle;
					return true;
				}*/
			}
			a.transform.Translate(Vector3.up * i * .001f);
			a.transform.eulerAngles = origAngle;
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