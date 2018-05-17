using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TrackDetections : MonoBehaviour {


	public int DetectedPiece;
	public bool DetectingNow;
	public GameObject DetectedObj;
	// Use this for initialization
	private void Awake()
	{
		DetectingNow = true;
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerStay(Collider coll)
	{

		if (DetectingNow == true)
		{
			if (coll.gameObject.CompareTag("CornerPiece"))
			{
				DetectedObj = coll.gameObject;
				DetectedPiece = 1;
				DetectingNow = false;
				Debug.Log(DetectedPiece);

			}

			if (coll.gameObject.CompareTag("StraightPiece"))
			{
				DetectedObj = coll.gameObject;
				DetectedPiece = 0;
				DetectingNow = false;
				Debug.Log(DetectedPiece);

			}	
		}
		else
		{
			
		}
			
		
		
	}
}
