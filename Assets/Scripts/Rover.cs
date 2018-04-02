using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : Mover {

	public GameObject target;
	public float threshold = 2f;

	// controls how anxiously we steer towards our wander target
	public float wanderWeight = 0.125f;

	protected override void Start () {
		base.Start ();
		velocity = new Vector3 (Random.Range (0, maxSpeed) - maxSpeed / 2f, 0, Random.Range (0, maxSpeed) - maxSpeed / 2f);
	}

	protected override void CalcSteering() {
		Vector3 fp = FuturePosition (1f);
		float xWeight = Mathf.Abs (fp.x) > 20f ? 1f + Mathf.Abs (fp.x) - 20f : 0f;
		float zWeight = Mathf.Abs (fp.z) > 20f ? 1f + Mathf.Abs (fp.z) - 20f : 0f;


		Vector3 wallForce = AvoidWall ();
		ApplyForce ((xWeight + zWeight) * wallForce);

		// try playing with wanderWeight and see what happens!
		ApplyForce (Wander () * wanderWeight);
	}

	protected override void OnRenderObject() {
		base.OnRenderObject ();

		if (target) {
			ColorHelper.green.SetPass (0);

			GL.Begin (GL.LINES);
			GL.Vertex (transform.position);
			GL.Vertex (target.transform.position);
			GL.End ();
		}
	}
}
