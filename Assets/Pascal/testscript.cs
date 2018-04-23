using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour {
	float speed;
	// Use this for initialization
	void Start () {
		speed = 70f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y + Input.acceleration.x * Time.deltaTime * speed, transform.localEulerAngles.z);
	}

	public void MoveForward(){
		Debug.Log ("commit sudoku");
	}
}
