using UnityEngine;
using System.Collections;


public class OrthoThirdPersonControllerNET : MonoBehaviour
{
	public Rigidbody target;
		// The object we're steering
	public float speed = .2f, walkSpeedDownscale = 2.0f, turnSpeed = 2.0f, mouseTurnSpeed = 0.3f, jumpSpeed = 15.0f;
		// Tweak to ajust character responsiveness
	public LayerMask groundLayers = -1;
		// Which layers should be walkable?
		// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset = 0.7f;
		// Tweak so check starts from just within target footing
	public JumpDelegate onJump = null;
		// Assign to this delegate to respond to the controller jumping

	
	
	private const float inputThreshold = 0.01f,
		groundDrag = 10.0f,
		directionalJumpFactor = 0.7f;
		// Tweak these to adjust behaviour relative to speed
	private const float groundedDistance = 0.5f;
		// Tweak if character lands too soon or gets stuck "in air" often
		
	
	private bool grounded, walking;

    public bool isRemotePlayer = true;

	//(DEPRECATED)bools set trigger volumes that allow player to change axis of movement
	public bool canTurnHorizontal=false;
	public bool canTurnVertical=false;

	public bool isHorizontal=true;
	public bool searchNight=false;

	private Quaternion faceRight;
	private Quaternion faceLeft;
	private Quaternion faceUp;
	private Quaternion faceDown;

	public Vector3 AISpawnPoint;

	private int numSearchersInVacinity=0;
	
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
		faceRight = Quaternion.Euler(0f, 180f, 0f);
		faceLeft = Quaternion.Euler(0f, 0f, 0f);
		faceUp = Quaternion.Euler(0f, 90f, 0f);
		faceDown = Quaternion.Euler(0f, 270f, 0f);


		AISpawnPoint = GameObject.Find("SearchPartySpawn").transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag.Equals("searchparty"))
		{
			numSearchersInVacinity++;
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag.Equals("searchparty"))
		{
			numSearchersInVacinity--;
		}
	}
	public void Monsterify() {
		PhotonPlayer playa = PhotonNetwork.player; // camina en la playa
		if (searchNight)
		{
			//transform.Find("Viking").localScale= (numSearchersInVacinity<10) ? (2f-.1f*numSearchersInVacinity)*transform.localScale : 1f*transform.localScale;
			DoScale(transform.Find("Viking").localScale, (numSearchersInVacinity<10) ? (2f-.1f*numSearchersInVacinity)*transform.localScale : 1f*transform.localScale, .35f );
			GameObject meshParent = transform.FindChild("Viking/BaseHuman").gameObject;
			float colorFactor = (numSearchersInVacinity < 10) ? (numSearchersInVacinity)*.1f : 1f;
			
			Color updatedColor = new Color(colorFactor,colorFactor,colorFactor);
			meshParent.renderer.material.color = (Color.Lerp(meshParent.renderer.material.color, updatedColor, .2f));
		}

		//photonView.RPC ("MonsterifyRPC", PhotonTargets.All, PhotonNetwork.player.ID);
	}

	void MonsterifyRPC()
	{

		if (searchNight)
		{
			//transform.Find("Viking").localScale= (numSearchersInVacinity<10) ? (2f-.1f*numSearchersInVacinity)*transform.localScale : 1f*transform.localScale;
			DoScale(transform.Find("Viking").localScale, (numSearchersInVacinity<10) ? (2f-.1f*numSearchersInVacinity)*transform.localScale : 1f*transform.localScale, .35f );
			GameObject meshParent = transform.FindChild("Viking/BaseHuman").gameObject;
			float colorFactor = (numSearchersInVacinity < 10) ? (numSearchersInVacinity)*.1f : 1f;
			
			Color updatedColor = new Color(colorFactor,colorFactor,colorFactor);
			meshParent.renderer.material.color = (Color.Lerp(meshParent.renderer.material.color, updatedColor, .2f));
		}
	}
	
	
	void Update ()
	// Handle rotation here to ensure smooth application.
	{
        if (isRemotePlayer) return;

		Monsterify();

		float rotationAmount;
		
		/*if (Input.GetMouseButton (1) && (!requireLock || controlLock || Screen.lockCursor))
		// If the right mouse button is held, rotation is locked to the mouse
		{
			if (controlLock)
			{
				Screen.lockCursor = true;
			}
			
			rotationAmount = Input.GetAxis ("Mouse X") * mouseTurnSpeed * Time.deltaTime;
		}
		else
		{
			if (controlLock)
			{
				Screen.lockCursor = false;
			}
			

		}*/

		//rotationAmount = Input.GetAxis ("Vertical") * turnSpeed * Time.deltaTime;
		
		if (Input.GetKey(KeyCode.D)) //and not any other key that might combine for diff angle
		{
			//if facing left or is allowed to switch axis
			//Debug.Log(Mathf.Abs(Quaternion.Angle(faceLeft, target.transform.rotation)-180));
			//if (Quaternion.Angle(faceLeft, target.transform.rotation)<1f || canTurnHorizontal)
			{
				isHorizontal=true;
				rotationAmount = Quaternion.Angle(faceRight, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			
		}

		else if (Input.GetKey(KeyCode.A))
		{
			//if (Quaternion.Angle(faceRight, target.transform.rotation)<1f || canTurnHorizontal)
			{
				isHorizontal=true;
				rotationAmount = Quaternion.Angle(faceLeft, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			
		}

		else if (Input.GetKey(KeyCode.W))
		{
			//if (Quaternion.Angle(faceDown, target.transform.rotation)<1f || canTurnVertical)
			{
				isHorizontal=false;
				rotationAmount = Quaternion.Angle(faceUp, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			
		}

		else if (Input.GetKey(KeyCode.S))
		{
			//if (Quaternion.Angle(faceUp, target.transform.rotation)<1f || canTurnVertical)
			{
				isHorizontal=false;
				rotationAmount = Quaternion.Angle(faceDown, target.transform.rotation);
				target.transform.RotateAround (target.transform.position,target.transform.up, rotationAmount);
			}
			
		}

		
		if (Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.Plus))
		{
			walking = !walking;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			//spawn ai
			searchNight=true;
			PhotonNetwork.Instantiate("CharprefabAI", AISpawnPoint, Quaternion.identity, 0);
		}
	}
	
	
	float SidestepAxisInput
	// If the right mouse button is held, the horizontal axis also turns into sidestep handling
	{ 
		get
		{
			/*if (Input.GetMouseButton (1))
			{
				float sidestep = -(Input.GetKey(KeyCode.Q)?1:0) + (Input.GetKey(KeyCode.E)?1:0);
                float horizontal = Input.GetAxis ("Vertical");
				
				return Mathf.Abs (sidestep) > Mathf.Abs (horizontal) ? sidestep : 0;
			}
			else
			{
                float sidestep = -(Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? 1 : 0);
                return sidestep;
			}*/
			return 0;
		}
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

        if (isRemotePlayer) return;


		if (grounded)
		{
			target.drag = groundDrag;
				// Apply drag when we're grounded
			
			if (Input.GetButton ("Jump"))
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
				Vector3 movement = (isHorizontal) ? Input.GetAxis ("Horizontal") * target.transform.forward : Input.GetAxis ("Vertical") * target.transform.forward;
				
				float appliedSpeed = walking ? speed / walkSpeedDownscale : speed;
					// Scale down applied speed if in walk mode
				
				/*if (Input.GetAxis ("Horizontal") < 0.0f)
				// Scale down applied speed if walking backwards
				{
					appliedSpeed /= walkSpeedDownscale;
				}*/

				if (movement.magnitude > inputThreshold)
				// Only apply movement if we have sufficient input
				{
					target.AddForce (movement.normalized * appliedSpeed, ForceMode.VelocityChange);
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
	
	
	void OnDrawGizmos ()
	// Use gizmos to gain information about the state of your setup
	{
		if (target == null)
		{
			return;
		}
		
		Gizmos.color = grounded ? Color.blue : Color.red;
		Gizmos.DrawLine (target.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.position + target.transform.up * -(groundedCheckOffset + groundedDistance));
	}

	public void DoScale(Vector3 start, Vector3 end, float totalTime) {
		StartCoroutine(CR_DoScale(start, end, totalTime));
	}
	IEnumerator CR_DoScale(Vector3 start, Vector3 end, float totalTime) {
		float t = 0;
		do {
			transform.Find("Viking").localScale = Vector3.Lerp(start, end, t / totalTime);
			yield return null;
			t += Time.deltaTime;
		} while (t < totalTime);
			transform.Find("Viking").localScale = end;
		yield break;
	}
}
