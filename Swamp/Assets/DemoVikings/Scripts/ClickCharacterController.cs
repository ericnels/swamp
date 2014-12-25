using UnityEngine;
using System.Collections;

public class ClickCharacterController : MonoBehaviour {
	
	private CharacterMotor motor;
	private Vector3 targetPosition;
	private Vector3 directionVector;
	private Camera mainCamera;
	
	public float walkMultiplier = 1f;
	public bool defaultIsWalk = false;
	public float smooth = 0.0005F;
	
	void Start () {
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		if (motor==null) Debug.Log("Motor is null!!");
	}


	void Update ()
	{		
		// see if user pressed the mouse down
		if (Input.GetMouseButtonDown (0))
		{
			mainCamera = Camera.current;
			
			// We need to actually hit an object
			RaycastHit hit;
			if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),  out hit, 100))
				return;
			// We need to hit something (with a collider on it)
			if (!hit.transform)
				return;
			
			// Get input vector from kayboard or analog stick and make it length 1 at most
			targetPosition = hit.point;
			directionVector = hit.point - transform.position;
			directionVector.y = 0;
			if (directionVector.magnitude>1)
				directionVector = directionVector.normalized;
		}
		
		if (walkMultiplier!=1)
		{
			if ( (Input.GetKey("left shift") || Input.GetKey("right shift") || Input.GetButton("Sneak")) != defaultIsWalk ) {
				directionVector *= walkMultiplier;
			}
		}
		
		// Apply direction
		Vector3 diff = targetPosition - transform.position;
		motor.inputMoveDirection = diff.normalized;
		motor.inputMoveDirection = Vector3.forward;
		transform.position = Vector3.MoveTowards (transform.position, targetPosition, smooth);
		if (diff.magnitude < .1f)
		{
			transform.position = targetPosition;
			motor.inputMoveDirection = Vector3.zero;
		}
	}
	
	Camera FindCamera ()
	{
		if (camera)
			return camera;
		else
			return Camera.main;
	}
	
}