using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilderScript : MonoBehaviour
{
    public enum TrackPieceType
    {
        Nothing,
        Straight,
        Corner
    }

    public TrackPieceType CurrentPiece;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("StraightPiece"))
        {
            CurrentPiece = TrackPieceType.Straight;
        }
        
    }
}
