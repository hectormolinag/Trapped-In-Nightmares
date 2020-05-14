using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    [Header("Particle System")]
    public ParticleSystem interactionButtonParticle;

    private bool canCheckInput = false;
    void Start()
    {
        interactionButtonParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(GameManager.Instance.player.transform.position - transform.position, Vector3.up);
        
        if (canCheckInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            canCheckInput = true;
            
            interactionButtonParticle.gameObject.SetActive(true);
            interactionButtonParticle.Play();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCheckInput = false;
            
            interactionButtonParticle.gameObject.SetActive(false);
        }
            
    }

    public void TriggerDialogue()
    {
        DialogueManager.current.StartDialogue(dialogue);
    }

    private void OnGrabArmRobot()
    {
        
    }

    private void onGrabBodyRobot()
    {
        
    }
}
