using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class TrackBuilder : MonoBehaviour
{
    public GameObject startpiece;
    public Button test;
    public bool foundStartPiece;
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
	    GameObject startInScene = GameObject.Find("Track/StartPiece");

        if (foundStartPiece == false && startInScene)
        {
            startpiece = startInScene;
            foundStartPiece = true;

            Debug.Log(foundStartPiece);
	    }
        else if (!startInScene)
	    {
	        Debug.Log(foundStartPiece);
	    }

	    
	}


}
