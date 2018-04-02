using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorHelper : MonoBehaviour {

	public static Vector3 Clamp(Vector3 v, float magLimit) {
		float mag = v.magnitude;
		if (mag > magLimit) {
			v *= magLimit / v.magnitude;
		}
		return v;
	}

	// given an angle in radians
	// return a unit vector pointing in that direction on the x-z plane
	public static Vector3 AngleToUnit(float radians) {
		// hooray trigonometry!
		return new Vector3 (Mathf.Cos (radians), 0, Mathf.Sin (radians));
	}
}
