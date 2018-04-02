using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour {

	CharacterController characterController;
	Vector3 acceleration;
	protected Vector3 velocity;

	// a sphere in the world that we'll aim at when wandering
	public GameObject wanderTarget;

	// the distance away from our future position to position the wanderTarget
	public float wanderRadius = 1.5f;

	// the current direction/rotation of the wander target for smoothing (radians!)
	protected float wanderRotation = 0f;

	// the rate at which the wander target randomly moves (higher is more erratic)
	public float wanderRate = 0.1f;

	public float mass = 1f;
	public float maxSpeed = 0.5f;
	public float maxTurn = 0.25f;
	public float radius = 0.5f;
	public GameObject futurePosIndicator;

	protected virtual void Start () {
		this.characterController = GetComponent<CharacterController> ();

		// give it a random starting rotation so they don't all wander the same direction at the start!
		wanderRotation = Random.Range (0f, Mathf.PI * 2f);

		velocity = Vector3.zero;
		acceleration = Vector3.zero;
	}
		
	protected abstract void CalcSteering ();

	protected Vector3 Seek(Vector3 targetPos) {
		Vector3 toTarget = targetPos - this.transform.position;
		Vector3 desiredVelocity = toTarget.normalized * maxSpeed;
		Vector3 steeringForce = desiredVelocity - velocity;

		return VectorHelper.Clamp (steeringForce, maxTurn);
	}
		
	protected Vector3 Flee(Vector3 targetPos) {
		// not yet implemented
		return Vector3.zero;
	}
		
	protected Vector3 Arrive(Vector3 targetPos, float threshold, float radii) {
		Vector3 toTarget = targetPos - this.transform.position;
		Vector3 desiredVelocity;

		if (toTarget.magnitude - radii < threshold) {
			float percentFromCenter = (toTarget.magnitude - radii) / threshold;
			float fractionOfMaxSpeed = percentFromCenter * maxSpeed;

			desiredVelocity = toTarget.normalized * fractionOfMaxSpeed;
		} else {
			desiredVelocity = toTarget.normalized * maxSpeed;
		}
			
		Vector3 steeringForce = desiredVelocity - velocity;

		return VectorHelper.Clamp (steeringForce, maxTurn);
	}

	public void ApplyForce(Vector3 force) {
		// tired of having things steer up or down from their current location?
		// set force.y to 0, and it will never be tempted to accelerate up or down!
		// one y to rule them all!
		force.y = 0;
		acceleration += force / mass;
	}

	public Vector3 FuturePosition(float seconds) {
		return this.transform.position + velocity * seconds;
	}

	public Vector3 AvoidWall() {
		return Seek (Vector3.zero);
	}

	public Vector3 Wander() {
		// change the wanderRotation value by a random amount
		// this keeps us walking in generally the same direction from frame to frame
		wanderRotation += Random.Range (-wanderRate, wanderRate);

		// start at our future position
		Vector3 future = FuturePosition (3f);

		// construct a unitRotation vector from our wanderRotation
		Vector3 unitRotation = VectorHelper.AngleToUnit (wanderRotation);

		// the wander position is our future position, plus a vector of magnitude wanderRadius pointing in the direction of our unitRotation
		Vector3 finalWanderPosition = future + (unitRotation * wanderRadius);

		// make sure it's drawn at the same level as our character
		finalWanderPosition.y = transform.position.y;

		if (wanderTarget) {
			// put it in position
			wanderTarget.transform.position = finalWanderPosition;
		}

		// seek the result
		return Seek(finalWanderPosition);
	}
		
	void LateUpdate () {

		CalcSteering ();

		velocity += acceleration;
		velocity = VectorHelper.Clamp (velocity, maxSpeed);
		this.characterController.Move (velocity * Time.deltaTime);

		this.futurePosIndicator.transform.position = FuturePosition (3f);

		acceleration = Vector3.zero;
	}

	protected virtual void OnRenderObject() {
		ColorHelper.black.SetPass (0);

		GL.Begin (GL.LINES);
		GL.Vertex (transform.position);
		GL.Vertex (transform.position + velocity.normalized);
		GL.End ();

		// always good to do a sanity check for things that might not exist!
		if (wanderTarget) {
			ColorHelper.red.SetPass (0); // in red

			GL.Begin (GL.LINES); // draw lines
			GL.Vertex (FuturePosition (3f)); // from the future position
			GL.Vertex (wanderTarget.transform.position); // to the wanderTarget's position
			GL.End (); // tada!
		}
	}
}
