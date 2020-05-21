using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WhosTalking
{
    Kid,
    Robot
}

public class DialogueTrigger : MonoBehaviour
{
    public WhosTalking talker;
    public Dialogue dialogue;

    [SerializeField] private bool destroyAfter = true;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && destroyAfter)
        {
            gameObject.SetActive(false);
        }
            
    }

    public void TriggerDialogue()
    {
        GameManager.Instance.DefineTargetDialogueCamera(talker);
        DialogueManager.current.StartDialogue(dialogue);
    }
}
