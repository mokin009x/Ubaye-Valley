using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour {
	float speed;
    public AudioSource engineSound;
	// Use this for initialization
	void Start () {
		speed = 70f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y + Input.acceleration.x * Time.deltaTime * speed, transform.localEulerAngles.z);
		if (Input.GetMouseButton(0)){
			MoveForward ();
		}
        else
        {
            engineSound.Stop();

        }
			
	}

	public void MoveForward(){
		transform.Translate (0f, 0f, speed * Time.deltaTime * 0.05f);
        if (engineSound.isPlaying == false)
        engineSound.Play();
	}
}
