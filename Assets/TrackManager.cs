using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class TrackManager : MonoBehaviour
{
    public GameObject startpiece;
    public TrackBuilderScript TrackBuilderReference;
    public GameObject CurrentCollider;
    public Button test;
    public bool foundStartPiece;

    public GameObject StraightPiecePrefab;
	public GameObject CornerPiecePrefab;

	public Vector3 trackSpawnLocation;
	public Vector3 nextTrackSpawnModifier;
	
	
	
	public int Random;
	// Use this for initialization
    private void Awake()
    {
	    trackSpawnLocation = Vector3.zero;
	    nextTrackSpawnModifier = Vector3.zero;

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
		    BuildTrack();

        }

        if (Input.GetKeyDown(KeyCode.A))
	    {
		    FindStartPiece();

	    }

	    
	}

    public void FindStartPiece()
    {
        var startInScene = GameObject.Find("StartPiece/Random");
	    Debug.Log("test");
	    startpiece = startInScene;
	    TrackBuilderReference = startInScene.GetComponent<TrackBuilderScript>();
	    foundStartPiece = true;
    }

    public void BuildTrack()
    {
	    Random = Random + 1;
	    
	    if (TrackBuilderReference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Straight)
	    {
		    Debug.Log(TrackBuilderReference.CurrentPieceType);
		    nextTrackSpawnModifier = nextTrackSpawnModifier + new Vector3(0, 0, 9);
		    Instantiate(StraightPiecePrefab, trackSpawnLocation+nextTrackSpawnModifier, Quaternion.identity);

		    Debug.Log("you got here");

	    }
	    else if (TrackBuilderReference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Corner)
	    {
		    Debug.Log(TrackBuilderReference.CurrentPieceType);

		    nextTrackSpawnModifier = nextTrackSpawnModifier + new Vector3(0, 0, 9);

		    Instantiate(CornerPiecePrefab, trackSpawnLocation+nextTrackSpawnModifier, Quaternion.Euler(0,180,0));

		    Debug.Log("you got here");
		    


	    }
	    else
	    {
		    
	    }

	    Builder();

    }
	void Builder()
	{
		TrackBuilderReference.ActiveScript = false;
		TrackBuilderReference = TrackBuilderReference.NextBuilder;
		TrackBuilderReference.ActiveScript = true;
		TrackBuilderReference.FrontCollider.DetectingNow = true;

	}

  
}
