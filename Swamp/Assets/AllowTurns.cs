using UnityEngine;
using System.Collections;

public class AllowTurns : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		OrthoThirdPersonControllerNET controller = other.gameObject.GetComponent<OrthoThirdPersonControllerNET>();
		if (controller != null)
		{
			controller.canTurnVertical=true;
			controller.canTurnHorizontal=true;
		}
	}

	void OnTriggerExit(Collider other) {
		OrthoThirdPersonControllerNET controller = other.gameObject.GetComponent<OrthoThirdPersonControllerNET>();
		if (controller != null)
		{
			controller.canTurnVertical=false;
			controller.canTurnHorizontal=false;
		}
	}
}
