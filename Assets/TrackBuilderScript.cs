using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class TrackBuilderScript : MonoBehaviour
{
	public bool ActiveScript;
	public enum TrackPieceType
	{
		Nothing,
		Straight,
		Corner
	}

	public TrackDetections FrontCollider;
	public TrackDetections BackCollider;

	public TrackPieceType CurrentPieceType;

	public TrackBuilderScript NextBuilder;

	// Use this for initialization
	private void Awake()
	{
		ActiveScript = false;
		
	}

	void Start()
	{
		if (this.gameObject.CompareTag("Finish"))
		{
			ActiveScript = true;
		}
		BackCollider = gameObject.transform.GetChild(0).GetComponent<TrackDetections>();
		FrontCollider = gameObject.transform.GetChild(1).GetComponent<TrackDetections>();
	}

	// Update is called once per frame
	void Update()
	{
		TrackDetectionEvent();
	}

	private void TrackDetectionEvent()
	{
		if (gameObject.CompareTag("Finish"))
		{
			ActiveScript = false;
		}
		if (ActiveScript == true)
		{
			if (FrontCollider.DetectedPiece == 1)
			{
				CurrentPieceType = TrackPieceType.Corner;
				//NextBuilder = FrontCollider.DetectedObj.GetComponent<TrackBuilderScript>();
			}
			if (FrontCollider.DetectedPiece == 0)
			{
				CurrentPieceType = TrackPieceType.Straight;
				//NextBuilder = FrontCollider.DetectedObj.GetComponent<TrackBuilderScript>();

			
			}
		}
		
	}

}

