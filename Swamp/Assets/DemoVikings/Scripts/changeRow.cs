using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class changeRow : MonoBehaviour {
	
	public Transform farSideDestination;
	public Transform nearSideDestination;

	void Start()
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{

			if (other.gameObject.tag.Equals("Player"))
			{
				if (!other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().availableBridges.Contains(this))
				{
				other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().availableBridges.Push(this);
				other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().canCross=true;
				}
			}
			else if (other.gameObject.tag.Equals("searchparty"))
			{
				if (!other.gameObject.GetComponent<AIControllerNET>().availableBridges.Contains(this))
				{
				other.gameObject.GetComponent<AIControllerNET>().availableBridges.Push(this);
				other.gameObject.GetComponent<AIControllerNET>().canCross=true;
				}

			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!other.isTrigger)
		{
			if (other.gameObject.tag.Equals("Player"))
			{
			other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().availableBridges.Clear();
			other.gameObject.GetComponent<OrthoThirdPersonControllerNET>().canCross=false;
			}
			else if (other.gameObject.tag.Equals("searchparty"))
			{
			other.gameObject.GetComponent<AIControllerNET>().availableBridges.Clear();
			other.gameObject.GetComponent<AIControllerNET>().crossCooldown=false;
			other.gameObject.GetComponent<AIControllerNET>().canCross=false;
			}
		}
	}

	public void crossBridge(GameObject player)
	{

		bool? isPlayerController = player.GetComponent<OrthoThirdPersonControllerNET>()==null;
		Debug.Log(isPlayerController.ToString());
		bool isAI = isPlayerController ?? true;

		if (isAI)
		{
			AIControllerNET controller = player.GetComponent<AIControllerNET>();

			if (controller.canCross)
			{
				if (player.transform.position.x < gameObject.transform.position.x) //go to destination
				{
					float dx = Mathf.Abs(farSideDestination.transform.position.x - player.transform.position.x);
					iTween.Stop (player);
					iTween.MoveAdd (player, new Vector3 ((controller.facingLeft) ? dx: -1*dx, 0, 0), 0.5f);
					//Debug.Log(dx);
					
				}
				else //return to predestination
				{
					float dx = -1*Mathf.Abs(nearSideDestination.transform.position.x-player.transform.position.x) ;
					//Debug.Log(dx);
					iTween.Stop (player); 
					iTween.MoveAdd (player, new Vector3 ((controller.facingLeft) ? dx: -1*dx, 0, 0), 0.5f);
					
				}
			}
		}

		else // not AI
		{
			OrthoThirdPersonControllerNET controller = player.GetComponent<OrthoThirdPersonControllerNET>();

			if (controller.canCross)
			{

				if (player.transform.position.x < gameObject.transform.position.x) //go to destination
				{
					float dx = Mathf.Abs(farSideDestination.transform.position.x - player.transform.position.x);
					iTween.Stop (player);
					iTween.MoveAdd (player, new Vector3 ((controller.facingLeft) ? dx: -1*dx, 0, 0), 0.5f);
					//Debug.Log(dx);

				}
				else //return to predestination
				{
					float dx = -1*Mathf.Abs(nearSideDestination.transform.position.x-player.transform.position.x) ;
					//Debug.Log(dx);
					iTween.Stop (player); 
					iTween.MoveAdd (player, new Vector3 ((controller.facingLeft) ? dx: -1*dx, 0, 0), 0.5f);

				}
			}
		}

	}


		
}
