using UnityEngine;
using System.Collections;

public class ModularTerrainCameraControl : MonoBehaviour {

	//Range for min/max values of variable
	public float _speed = 0;

	// Camera Movement
	void Update () {
		gameObject.transform.Translate(0, 0, _speed* Time.deltaTime);
	}

	public void SetSpeed(float sent_speed)
	{
		_speed = sent_speed;
	}


}
