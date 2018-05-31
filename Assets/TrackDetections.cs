using UnityEngine;

public class TrackDetections : MonoBehaviour
{
    public GameObject DetectedObj;


    public int DetectedPiece;

    public bool DetectingNow = true;

    // Use this for initialization
    private void Awake()
    {
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (DetectedObj == null) DetectingNow = true;
    }

    private void OnTriggerStay(Collider coll)
    {
        if (DetectingNow)
        {
            if (coll.gameObject.CompareTag("Finish"))
            {
                DetectedObj = coll.gameObject;
                DetectedPiece = 2;
                Debug.Log(DetectedPiece);
                DetectingNow = false;
            }
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
    }
}