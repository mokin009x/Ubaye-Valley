using UnityEngine;

public class TrackBuilderScript : MonoBehaviour
{
    public enum TrackPieceType
    {
        Nothing,
        Straight,
        Corner,
        Start
    }

    public bool ActiveScript;
    public TrackDetections BackCollider;

    public TrackPieceType CurrentPieceType;

    public TrackDetections FrontCollider;

    public TrackBuilderScript NextBuilder;

    // Use this for initialization
    private void Awake()
    {
        ActiveScript = false;
        if (gameObject.CompareTag("Finish")) ActiveScript = true;
    }

    private void Start()
    {
        BackCollider = gameObject.transform.GetChild(0).GetComponent<TrackDetections>();
        FrontCollider = gameObject.transform.GetChild(1).GetComponent<TrackDetections>();
    }

    // Update is called once per frame
    private void Update()
    {
        TrackDetectionEvent();
    }

    public void TrackDetectionEvent()
    {
        if (ActiveScript)
        {
            if (FrontCollider.DetectedPiece == 2)
            {
                CurrentPieceType = TrackPieceType.Start;
            }
            if (FrontCollider.DetectedPiece == 1)
            {
                CurrentPieceType = TrackPieceType.Corner;
            }

            if (FrontCollider.DetectedPiece == 0)
            {
                CurrentPieceType = TrackPieceType.Straight;
            }
            Debug.Log(CurrentPieceType);
        }
    }

    public void GetNextBuilder()
    {
        NextBuilder = FrontCollider.DetectedObj.GetComponent<TrackBuilderScript>();
  
    }

    public void ActivateScript()
    {
        ActiveScript = true;
    }

    public void TurnOnDetection()
    {
        FrontCollider.DetectingNow = true;
    }
}