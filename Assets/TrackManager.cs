using System.Timers;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class TrackManager : MonoBehaviour
{


    public GameObject CornerPiecePrefab;
    public GameObject CurrentCollider;
    public bool foundStartPiece;
    public Vector3 nextTrackSpawnModifier;

    public bool buildingTrack = false;
    public int Pieces;
    public GameObject startpiece;

    public GameObject StraightPiecePrefab;
    public Button test;
    public TrackBuilderScript TrackBuilderReference;

    public Vector3 trackSpawnLocation;
    public Vector3 trackRotation;

    public float buildDelay= 1;
    private float buildTimer;


    public GameObject currentPiece;
    // Use this for initialization
    private void Awake()
    {

        foundStartPiece = false;
        Pieces = 0;
    }

    private void Start()
    {
        trackSpawnLocation = GameObject.FindGameObjectWithTag("ActualStart").transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
        Debug.Log(TotalPieces());
        FindStartPiece();

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            buildingTrack = true;
        }

        if (buildingTrack == true)
        {
            BuildTrack();

        }

        buildTimer = Mathf.Clamp(buildTimer - Time.deltaTime,0,10);
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
        

        if (buildTimer <=0)
        {
            if (TrackBuilderReference.CurrentPieceType != TrackBuilderScript.TrackPieceType.Start)
            {
                if (TrackBuilderReference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Straight)
                {
                    Debug.Log(TrackBuilderReference.CurrentPieceType);
                    trackSpawnLocation = trackSpawnLocation+ nextTrackSpawnModifier;

                    currentPiece = Instantiate(StraightPiecePrefab, trackSpawnLocation, Quaternion.identity);

                    Debug.Log("you got here");

                }
                else if (TrackBuilderReference.CurrentPieceType == TrackBuilderScript.TrackPieceType.Corner)
                {
                    Debug.Log(TrackBuilderReference.CurrentPieceType);

                    trackSpawnLocation = trackSpawnLocation+ nextTrackSpawnModifier;

                    currentPiece = Instantiate(CornerPiecePrefab, trackSpawnLocation, Quaternion.Euler(0, 180, 0));
                    Debug.Log("you got here");

                }
                Builder();
                PieceRotAndPos();
            }
            else
            {
                buildingTrack = false;
            }

            buildTimer = buildDelay;
        }

        }

    public void PieceRotAndPos()
    {
        Vector3 frontDirection;
        Vector3 BackDirection;

        frontDirection = currentPiece.transform.position + currentPiece.transform.GetChild(1).transform.position;
        Debug.Log(frontDirection);
    }

    private void Builder()
    {
        TrackBuilderReference.ActiveScript = false;
        TrackBuilderReference.GetNextBuilder();
        TrackBuilderReference = TrackBuilderReference.NextBuilder;
        TrackBuilderReference.ActivateScript();
        TrackBuilderReference.TurnOnDetection();
    }

    public int TotalPieces()
    {
        var allPieces =  GameObject.FindGameObjectsWithTag("CornerPiece").Length + GameObject.FindGameObjectsWithTag("StraightPiece").Length + GameObject.FindGameObjectsWithTag("Finish").Length ;
        return allPieces;
    }
    
}