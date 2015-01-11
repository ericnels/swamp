using UnityEngine;
using System.Collections;

public class changeRow : MonoBehaviour {
	public bool canCross;
	public bool hasCrossed;
	public Transform farSideDestination;
	public Transform nearSideDestination;
	public Transform lastPlayerPos;

	void Start()
	{
		hasCrossed=false;
		canCross=false;
	}

	void OnTriggerEnter(Collider other)
	{
		canCross=true;
		if (other.gameObject.tag.Equals("Player"))
		{
			other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().availableBridges.Add(this);
			lastPlayerPos=other.gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		canCross=false;
		if (other.gameObject.tag.Equals("Player"))
		{
			other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().availableBridges.Remove(this);
			lastPlayerPos=other.gameObject.transform;
		}
	}

	public void crossBridge(GameObject player)
	{
		if (canCross)
		{
			///ACCOUNT FOR WHEN PLAYER FACING LEFT / RIGHT
			/// EITHER USE AN OBJECT WHOSE LOCAL AXIS DOESN'T CHANGE OR FIND WAY TO ADJUST

			if (!hasCrossed) //go to destination
			{
				hasCrossed=true;
				float dx = Mathf.Abs(farSideDestination.transform.position.x - player.transform.position.x);
				iTween.Stop (player);
				iTween.MoveAdd (player, new Vector3 (dx, 0, 0), 0.5f);
				Debug.Log(dx);
				Debug.Log("gofar");

			}
			else //return to predestination
			{
				hasCrossed=false;
				float dx = -1*Mathf.Abs(nearSideDestination.transform.position.x-player.transform.position.x) ;
				Debug.Log(dx);
				iTween.Stop (player); 
				iTween.MoveAdd (player, new Vector3 (dx, 0, 0), 0.5f);
				Debug.Log("comenear");

			}
		}
	}
		
}
