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
	public GameObject CornerPiecePrefab;

	public int Random;
	// Use this for initialization
    private void Awake()
    {
        foundStartPiece = false;
	    Random = 0;
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
        var startInScene = GameObject.Find("StartPiece/Random");
	    Debug.Log("test");
	    startpiece = startInScene;
	    TrackBuilderRefference = startInScene.GetComponent<TrackBuilderScript>();
	    foundStartPiece = true;
    }

    public void BuildTrack()
    {
	    Random = Random + 1;
	    
	    /*
	    if (TrackBuilderRefference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Straight)
	    {
		    Instantiate(CornerPiecePrefab, new Vector3(0,0,10+ Random), Quaternion.identity);
		    TrackBuilderRefference.FrontCollider.DetectingNow = true;

		    Debug.Log("you got here");
		    TrackBuilderRefference = TrackBuilderRefference.NextBuilder;
	    }
	    */

	    
		    
	    if (TrackBuilderRefference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Corner)
		    {
			    Instantiate(StraightPiecePrefab, new Vector3(0,0,10+ Random), Quaternion.identity);
			    TrackBuilderRefference.FrontCollider.DetectingNow = true;

			    Debug.Log("you got here");
			    TrackBuilderRefference = TrackBuilderRefference.NextBuilder;
			    
 
		    }  
	    
		   
	    
		   

		   
	    
      
    }

  
}
