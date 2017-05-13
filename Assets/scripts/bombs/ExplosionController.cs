using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

	private Color alphaColor;
	private float timeToFade = 1.0f;

	void Start() {
		alphaColor = GetComponent<MeshRenderer>().material.color;
		alphaColor.a = 0;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<MeshRenderer>().material.color = Color.Lerp(GetComponent<MeshRenderer>().material.color, alphaColor, timeToFade * Time.deltaTime);
		Color c = GetComponent<MeshRenderer> ().material.color;
		if (c.a == 0) {
			Destroy (gameObject);
		}
	}
}
