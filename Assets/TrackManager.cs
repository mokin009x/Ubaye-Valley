using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class TrackManager : MonoBehaviour
{
    public GameObject startpiece;
    public TrackBuilderScript TrackBuilderRefference;
    public GameObject CurrentCollider;
    public Button test;
    public bool foundStartPiece;

    public GameObject StraightPiecePrefab;
	// Use this for initialization
    private void Awake()
    {
        foundStartPiece = false;
    }

    void Start () {
       
	}
	
	// Update is called once per frame
	void Update ()
	{

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        FindStartPiece();
	        BuildTrack();
        }

        if (Input.GetKeyDown(KeyCode.B)&& foundStartPiece==true)
	    {
	    }

	    
	}

    public void FindStartPiece()
    {
        var startInScene = GameObject.Find("StartPiece");
        if (startInScene)
        {
            startpiece = startInScene;
            TrackBuilderRefference = startpiece.GetComponent<TrackBuilderScript>();
            foundStartPiece = true;
            
        }
    }

    public void BuildTrack()
    {
        CurrentCollider = startpiece.transform.Find("FrontCollider").gameObject;
        
        if (TrackBuilderRefference.CurrentPiece == TrackBuilderScript.TrackPieceType.Straight)
        {
            Instantiate(StraightPiecePrefab, startpiece.transform.position, Quaternion.identity);
        }
        


    }

  
}
