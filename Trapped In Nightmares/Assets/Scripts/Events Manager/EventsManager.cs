using System;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager current;

   private void Awake() 
   {
       current = this;
   }

   public event Action onDoorWayTriggerEnter;
   public event Action onDoorWayTriggerExit;

   public event Action onGrabArmRobot;
   public event Action onGrabBodyRobot;

   public void DoorWayTriggerEnter()
   {
       onDoorWayTriggerEnter?.Invoke();
   }

   public void DoorWayTriggerExit()
   {
       onDoorWayTriggerExit?.Invoke();
   }

   public void GrabArmRobot()
   {
       onGrabArmRobot?.Invoke();
   }

   public void GrabBodyRobot()
   {
       onGrabBodyRobot?.Invoke();
   }




}
