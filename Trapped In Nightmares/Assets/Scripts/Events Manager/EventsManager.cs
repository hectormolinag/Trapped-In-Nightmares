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

   public event Action<int> onPlatformTriggerEnter;

   public event Action<int> onParentingCage;
   public event Action<int> onUnParentingCage;

   public event Action<int> onStartMiniGame;
   public event Action<int> onEndMiniGame;
    
   public void DoorWayTriggerEnter()
   {
       if(onDoorWayTriggerEnter != null)
       {
           onDoorWayTriggerEnter();
       }
   }

   public void DoorWayTriggerExit()
   {
       if(onDoorWayTriggerExit != null)
       {
           onDoorWayTriggerExit();
       }
   }

   public void PlatformTriggerEnter(int id)
   {
       if(onPlatformTriggerEnter != null)
       {
           onPlatformTriggerEnter(id);
       }
   }

   public void ParentingCage(int id)
   {
       if(onParentingCage != null)
       {
           onParentingCage(id);
       }
   }

   public void UnParentingCage(int id)
   {
       if(onUnParentingCage != null)
       {
           onUnParentingCage(id);
       }
   }

   public void OnStartMiniGame(int id)
   {
       if(onStartMiniGame != null)
       {
           onStartMiniGame(id);
       }
   }

   public void OnEndMiniGame(int id)
   {
       if(onEndMiniGame != null)
       {
           onEndMiniGame(id);
       }
   }


}
