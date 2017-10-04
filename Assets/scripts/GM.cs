using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//GameManager class for storing global functions
public class GM : MonoBehaviour {
	private GM m_Instance;
	public GM Instance { get { return m_Instance; } }

	//public variables for global use
	public static string mode = "cone check";

	void Awake() {
		m_Instance = this;
	}

	void OnDestroy() {
		m_Instance = null;
	}

	/**
	 * toggle the simulation mode between 'cone check' and 'collision prediction'
	 */
	public void toggleMode() {
		mode = (mode == "cone check" ? "collision prediction": "cone check");
		GameObject.FindGameObjectWithTag("modeToggleButton").GetComponentInChildren<Text>().text = "Mode: " + mode;
	}
	
	/**
	 * perform a cone collision check against all specified objects, avoiding them if necessary
	 * @param a: the object from which to perform a cone check
	 * @param objs: the list of objects to check against
	 */
	public static void avoidConeCollisions(GameObject a, List<GameObject> objs) {
		//perform a cone check on each passed in object
		followChaser fc = a.GetComponent<followChaser>();
		fc.coneHitsThisFrame = 0;
		foreach (GameObject o in objs) {
			if (coneCheck(a, o)) {
				//cone check returned a collision; turn at a rate inversely proportional to our distance from the object
				++fc.coneHitsThisFrame;
			} 
		}
	}

	/**
	 * use collision prediction to determine the nearest future collision between the input object and list of other objects
	 * @param a: the object for which we wish to predict the nearesst collision
	 * @param objs: the list of objects against which we should check for future collisions
	 */
	public static Vector3 predictNearestCollision(GameObject a, List<GameObject> objs) {
		followChaser afc = a.GetComponent<followChaser>();
		float closestPredictedDistance = -1;
		afc.closestPredictedUnit = null;
		Vector3 closestPredictedLoc = Vector3.zero;

		//iterate over all passed in objects to find the one which will collide with us the soonest
		for (var i = 0; i < objs.Count; ++i) {
			GameObject b = objs[i];
			followChaser bfc = b.GetComponent<followChaser>();

			//calculate our collision prediction between a and b now
			Vector3 dp = b.transform.position - a.transform.position;
			Vector3 vt = bfc.spd * b.transform.up;
			Vector3 vc = afc.spd * a.transform.up;
			Vector3 dv = vt - vc;

			tClosest = -(Vector3.Dot(dp, dv) / Mathf.Pow(Mathf.Abs(dv), 2));

			Vector3 pcf = a.transform.position + vc * tClosest;
			Vector3 ptf = a.transform.position + vc * tClosest;

			float colDist = Vector3.Distance(pcf,ptf);
			if (colDist < (afc.radius + bfc.radius)) {
				//check if the distance at the time of collision will cause an intersection 
				if (closestPredictedDistance == -1 || colDist < closestPredictedDistance) {
					closestPredictedDistance = colDist;
					afc.closestPredictedUnit = b;
					closestPredictedLoc = pcf;
				}
			}
		}
		return closestPredictedLoc;
	}

	/**
	 * determine whether or not a cone extended from object a contains object b
	 * @param a: the object from which to extend the cone
	 * @param b: the object to check against the cone
	 * @returns whether b is contained in the cone extending from a (true) or not (false)
	 */
	public static bool coneCheck(GameObject a, GameObject b) {
		var cone = Mathf.Cos(a.GetComponent<followChaser>().coneArc/2 * Mathf.Deg2Rad);
		var heading = (b.transform.position - a.transform.position).normalized;
		return (Vector3.Dot(a.transform.up, heading) > cone) && Vector3.Distance(a.transform.position, b.transform.position) < a.GetComponent<followChaser>().coneLength;
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