using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") || other.gameObject.layer == 10)
        {
            EventsManager.current.DoorWayTriggerEnter();  //Dispatch the event
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player") || other.gameObject.layer == 10)
        {
            EventsManager.current.DoorWayTriggerExit();  //Dispatch the event
        }
    }
}
