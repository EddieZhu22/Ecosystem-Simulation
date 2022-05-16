using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomWind : MonoBehaviour {

	[Range (0, 1.0f)]
	public float displacement = 1;
	[Range (0, 5.0f)]
	public float shakeTime = 0.29f;
	[Range (0, 1.0f)]
	public float shakeWindSpeed = 0.61f;
	[Range (0, 1.0f)]
	public float shakeBending = 0.15f;

	void Start () {
		
	}

	void Update () {
		Shader.SetGlobalFloat ("_ShakeDisplacement", displacement);
		Shader.SetGlobalFloat ("_ShakeTime", shakeTime);
		Shader.SetGlobalFloat ("Shake Windspeed", shakeWindSpeed);
		Shader.SetGlobalFloat ("_ShakeBending", shakeBending);

	}
}
