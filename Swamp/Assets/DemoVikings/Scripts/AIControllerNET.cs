using UnityEngine;
using System.Collections;

public class AIControllerNET : MonoBehaviour {

	public enum AIState
	{
		RunningLeft,
		RunningRight,
		Shooting,
		Cowering,
		Jumping
	}

	public float panic=1f; //will turn AI red for violent / low fear, and green for non-violent/ high fear
	// scale of 0-1, 0 is violent, 1 is terrified, .5 is neutral

	public Rigidbody target;
	// The object we're steering
	public float speed = 1f, walkSpeedDownscale = 2.0f, turnSpeed = 2.0f, mouseTurnSpeed = 0.3f, jumpSpeed = 15.0f;
	// Tweak to ajust character responsiveness
	public LayerMask groundLayers = -1;
	// Which layers should be walkable?
	// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset = 0.7f;
	// Tweak so check starts from just within target footing

	// Turn this on if you want mouse lock controlled by this script
	public JumpDelegate onJump = null;
	// Assign to this delegate to respond to the controller jumping
	
	bool facingLeft;
	
	public AIState state;
	
	private const float inputThreshold = 0.01f,
	groundDrag = 10.0f,
	directionalJumpFactor = 0.7f;
	// Tweak these to adjust behaviour relative to speed
	private const float groundedDistance = 0.5f;
	// Tweak if character lands too soon or gets stuck "in air" often
	
	
	private bool grounded, walking;
	
	private bool isRemotePlayer = true;
	
	private Quaternion faceRight;
	private Quaternion faceLeft;
	public bool Grounded
		// Make our grounded status available for other components
	{
		get
		{
			return grounded;
		}
	}
	
	public void SetIsRemotePlayer(bool val)
	{
		isRemotePlayer = val;
	}
	
	void Reset ()
		// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
		// If target is not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponent<Rigidbody> ();
		}
	}

	/*float SidestepAxisInput
		// If the right mouse button is held, the horizontal axis also turns into sidestep handling
	{ 
		get
		{
			if (Input.GetMouseButton (1))
			{
				float sidestep = -(Input.GetKey(KeyCode.Q)?1:0) + (Input.GetKey(KeyCode.E)?1:0);
				float horizontal = Input.GetAxis ("Vertical");
				
				return Mathf.Abs (sidestep) > Mathf.Abs (horizontal) ? sidestep : 0;
			}
			else
			{
				float sidestep = -(Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? 1 : 0);
				return sidestep;
			}
		}
	}*/
	
	
	void Start ()
		// Verify setup, configure rigidbody
	{
		Setup ();
		// Retry setup if references were cleared post-add
		
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		target.freezeRotation = true;
		// We will be controlling the rotation of the target, so we tell the physics system to leave it be
		walking = false;
		facingLeft=true;
		faceRight = Quaternion.Euler(0f, 180f, 0f);
		faceLeft = Quaternion.Euler(0f, 0f, 0f);
		state=AIState.RunningLeft;
	}

	void ColorPanic()
	{
		GameObject meshParent = transform.FindChild("Viking/BaseHuman").gameObject;

		meshParent.renderer.material.color = new Color(panic,1-panic,0f);

	}
	
	void Update ()
	{

		float rotationAmount;

		//turn to face right if not already facing right
		if (state == AIState.RunningRight && facingLeft) //and not any other key that might combine for diff angle
		{
			
			if (Quaternion.Angle(faceRight, target.transform.rotation)>.5f)
			{
				rotationAmount = Quaternion.Angle(faceRight, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			else
			{
				facingLeft= false;
			}
			
		}

		//turn to face left if not already facing left
		if (state == AIState.RunningLeft && !facingLeft)
		{
			if (Quaternion.Angle(faceLeft, target.transform.rotation)>.5f)
			{
				rotationAmount = Quaternion.Angle(faceLeft, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			else
			{
				facingLeft=true;
			}
			
		}

		if (panic>1f)
			panic=1f;

		ColorPanic();
		
		
		/*if (Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.Plus))
		{
			walking = !walking;
		}*/

	}


	void FixedUpdate ()
		// Handle movement here since physics will only be calculated in fixed frames anyway
	{
		
		grounded = Physics.Raycast (
			target.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
			);
		// Shoot a ray downward to see if we're touching the ground
		
		
		if (grounded)
		{
			target.drag = groundDrag;
			// Apply drag when we're grounded
			
			if (state==AIState.Jumping)
				// Handle jumping
			{
				target.AddForce (
					jumpSpeed * target.transform.up*10f +
					target.velocity.normalized * directionalJumpFactor,
					ForceMode.VelocityChange
					);
				// When jumping, we set the velocity upward with our jump speed
				// plus some application of directional movement
				
				if (onJump != null)
				{
					onJump ();
				}
			}
			else
				// Only allow movement controls if we did not just jump
			{
				Vector3 movement = 3f * target.transform.forward;
				
				float appliedSpeed = 1f;
				// Scale down applied speed if in walk mode
				
				/*if (Input.GetAxis ("Horizontal") < 0.0f)
				// Scale down applied speed if walking backwards
				{
					appliedSpeed /= walkSpeedDownscale;
				}*/
				
				if (state == AIState.RunningLeft || state == AIState.RunningRight)
					// Only apply movement if we have sufficient input
				{
					target.AddForce (movement.normalized * appliedSpeed* (facingLeft ? 1: -1), ForceMode.VelocityChange);
				}
				else
					// If we are grounded and don't have significant input, just stop horizontal movement
				{
					target.velocity = new Vector3 (0.0f, target.velocity.y, 0.0f);
					return;

				}
			}
		}
		else
		{
			target.drag = 0.0f;
			// If we're airborne, we should have no drag
		}
	}
}
