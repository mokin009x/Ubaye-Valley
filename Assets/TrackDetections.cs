using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TrackDetections : MonoBehaviour {


	public int DetectedPiece;
	public bool DetectingNow = true;
	public GameObject DetectedObj;
	// Use this for initialization
	private void Awake()
	{
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (DetectedObj == null)
		{
			DetectingNow = true;

		}

	}

	private void OnTriggerStay(Collider coll)
	{

		if (DetectingNow == true)
		{
			if (coll.gameObject.CompareTag("CornerPiece"))
			{
				DetectedObj = coll.gameObject;
				DetectedPiece = 1;
				Debug.Log(DetectedPiece);
				DetectingNow = false;

			}

			if (coll.gameObject.CompareTag("StraightPiece"))
			{
				DetectedObj = coll.gameObject;
				DetectedPiece = 0;
				Debug.Log(DetectedPiece);
				DetectingNow = false;
			}	
		}
		else
		{
			
		}



	}
}
