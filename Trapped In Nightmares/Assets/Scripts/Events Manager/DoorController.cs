using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{    
    public float riseAmount = 3f;

    float yPosStart;
    void Start()
    {
        yPosStart = transform.position.y;

        EventsManager.current.onDoorWayTriggerEnter += OnDoorwayOpen;   //Subscribe event

        EventsManager.current.onDoorWayTriggerExit += OnDoorwayClose;   //Subscribe event
    }

    private void OnDoorwayOpen()
    {
        LeanTween.moveLocalY(gameObject, riseAmount, 0.9f).setEaseOutQuad();
    }
       
    private void OnDoorwayClose()
    {
        LeanTween.moveLocalY(gameObject, yPosStart, 0.7f).setEaseOutQuad();
    }
}
