using UnityEngine;
using System.Collections;

public class OrthographicCameraNET : MonoBehaviour {

	public Collider target;
	// The object we're looking at
	new public Camera camera;
	// The camera to control


	void Reset ()
		// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
		// If target and/or camera is not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponent<Collider> ();
		}
		
		if (camera == null)
		{
			if (Camera.main != null)
			{
				camera = Camera.main;
			}
		}
	}
	
	
	void Start ()
		// Verify setup, initialise bookkeeping
	{
		Setup ();
		// Retry setup if references were cleared post-add

		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		if (camera == null)
		{
			Debug.LogError ("No camera assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		//only look at if char is not AI or remote
		if (!target.gameObject.GetComponent<OrthoThirdPersonControllerNET>().isRemotePlayer)
			camera.transform.LookAt(target.transform.position);

	}

	void Update()
	{
		camera.transform.position = new Vector3((float)target.transform.position.x-5f, camera.transform.position.y, target.transform.position.z);
	}
}
