using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
	 * perform a cone collision check against all specified objects, avoid them if necessary
	 * @param a: the object from which to perform a cone check
	 * @param objs: the list of objects to check against
	 */
	public static void avoidConeCollisions(GameObject a, IEnumerable<GameObject> objs) {
		foreach (GameObject o in objs) {
			coneCheck(a, o); 
		}
	}

	/**
	 * determine whether or not a cone extended from object a contains object b
	 * @param a: the object from which to extend the cone
	 * @param b: the object to check against the cone
	 * @returns whether b is contained in the cone extending from a (true) or not (false)
	 */
	public static bool coneCheck(GameObject a, GameObject b) {
		return true;
	}

	/**
	 * move a GameObject backwards until it is no longer colliding with the other specified GameObject
	 * assumes circular colliders to keep things as simple as possible
	 * @param a: the object to move backwards
	 * @param b: the object to check for collisions against
	 */
	public static void moveOutsideCollision(GameObject a, GameObject b) {
		//check the intersection between objects a and b
		float intersect = checkCollision(a, b);
		if (intersect != -1) {
			//move a backwards by the magnitude of the intersection between a and b
			a.transform.Translate(Vector3.up * -intersect);
		}
	}

	/**
	 * checks for a collision between objects a and b, returning their intersection if a collision is found
	 * @param a: the first gameObject to check
	 * @param b: the second gameObject to check
	 * @returns the magnitude of the intersection between a and b if a collision is found, or -1 if no collision is found
	 */
	public static float checkCollision(GameObject a, GameObject b) {
		//grab the radii and positions of both objects
		float aRad = a.GetComponent<followChaser>().radius;
		float bRad = b.GetComponent<followChaser>().radius;

		//check if the distance between the objects indicates a collision between their radii
		float dist = Vector3.Distance(a.transform.position, b.transform.position);
		//if objects' radii are intersecting, return the magnitude of that intersection
		return (dist < aRad + bRad ? (aRad + bRad) - dist : -1);
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