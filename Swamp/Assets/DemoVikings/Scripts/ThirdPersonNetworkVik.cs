using UnityEngine;
using System.Collections;


public class ThirdPersonNetworkVik : Photon.MonoBehaviour
{
	OrthographicCameraNET cameraScript;
	OrthoThirdPersonControllerNET controllerScript;
	AIControllerNET controllerScriptAI;

    private bool appliedInitialUpdate;
	private bool isAI;
    void Awake()
    {
		bool? isPlayerController = GetComponent<OrthoThirdPersonControllerNET>()==null;
		isAI = isPlayerController ?? true;

		if (isAI)
		{
			cameraScript = null;
			controllerScript=null;
			controllerScriptAI = GetComponent<AIControllerNET>();
		}
		else
		{
			cameraScript = GetComponent<OrthographicCameraNET>();
			controllerScript = GetComponent<OrthoThirdPersonControllerNET>();
		}
		
    }
    void Start()
    {
        //TODO: Bugfix to allow .isMine and .owner from AWAKE!
		if (isAI)
		{
			controllerScriptAI.enabled=true;
			controllerScriptAI.SetIsRemotePlayer(true);
		}

        else if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
            cameraScript.enabled = true;
            controllerScript.enabled = true;
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(-5, 2, 0);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
			Camera.main.transform.parent=null;
			controllerScript.SetIsRemotePlayer(!photonView.isMine);

        }
        else
        {           
            cameraScript.enabled = false;
            controllerScript.enabled = true;
			controllerScript.SetIsRemotePlayer(!photonView.isMine);
        }
        

        gameObject.name = gameObject.name + photonView.viewID;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
           // stream.SendNext((int)controllerScript._characterState);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigidbody.velocity); 

        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            rigidbody.velocity = (Vector3)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
                rigidbody.velocity = Vector3.zero;
            }
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //We know there should be instantiation data..get our bools from this PhotonView!
		if (photonView.instantiationData != null)
		{
	        object[] objs = photonView.instantiationData; //The instantiate data..
	        bool[] mybools = (bool[])objs[0];   //Our bools!

	        //disable the axe and shield meshrenderers based on the instantiate data
	        MeshRenderer[] rens = GetComponentsInChildren<MeshRenderer>();
	        rens[0].enabled = false;//Axe
	        rens[1].enabled = false;//Shield
		}

    }

}