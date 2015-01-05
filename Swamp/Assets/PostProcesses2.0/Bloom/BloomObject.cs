using UnityEngine;
using System.Collections;

public class BloomObject : MonoBehaviour 
{
	
	public Color BloomColor = Color.white;

	float rpm = 20f;

	void FixedUpdate()
	{
		transform.Rotate(0,6f*rpm*Time.deltaTime,0);
	}
}

