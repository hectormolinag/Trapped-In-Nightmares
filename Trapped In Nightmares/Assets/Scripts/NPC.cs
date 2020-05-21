using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Dialogue Default")]
    public Dialogue dialogue;
    [Header("Dialogue when Arms are Picked")]
    public Dialogue dialogueArms;
    [Header("Dialogue when Body is Picked")]
    public Dialogue dialogueBody;
    [Header("Dialogue when all parts are Picked")]
    public Dialogue dialogueFinal;
    
    [Header("Particle System")]
    public ParticleSystem interactionButtonParticle;

    [Header("Robot")] 
    public GameObject robot;

    [Header("Trigger Barriers")] 
    [SerializeField] private GameObject triggerNotice = null;

    [SerializeField] private GameObject triggerEndTutorial = null;

    private bool canCheckInput = false;

    [SerializeField] private bool armsPicked = false;
    [SerializeField] private bool bodyPicked = false;
    
    void Start()
    {
        robot = GameManager.Instance.robot.gameObject;
        
        interactionButtonParticle.gameObject.SetActive(false);

        EventsManager.current.onGrabArmRobot += GrabArmRobot;
        EventsManager.current.onGrabBodyRobot += GrabBodyRobot;
    }

    private void OnDisable()
    {
        EventsManager.current.onGrabArmRobot -= GrabArmRobot;
        EventsManager.current.onGrabBodyRobot -= GrabBodyRobot;
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
        triggerNotice.SetActive(false);
        
        GameManager.Instance.DefineTargetDialogueCamera(WhosTalking.Robot);
        
        if(!armsPicked && !bodyPicked)
            DialogueManager.current.StartDialogue(dialogue);
        else if (armsPicked && !bodyPicked)
            DialogueManager.current.StartDialogue(dialogueArms);
        else if (!armsPicked && bodyPicked)
            DialogueManager.current.StartDialogue(dialogueBody);
        else
        {
            StartCoroutine(AppearRealRobot());
        }
    }

    IEnumerator AppearRealRobot()
    {
        GameManager.Instance.Fade();
        yield return new WaitForSeconds(1.5f);
        DialogueManager.current.StartDialogue(dialogueFinal);
        robot.transform.position = gameObject.transform.position;
        robot.transform.rotation = gameObject.transform.rotation;
        triggerEndTutorial.SetActive(false);
        robot.SetActive(true);
        gameObject.SetActive(false);
    }

    private void GrabArmRobot()
    {
        armsPicked = true;
    }

    private void GrabBodyRobot()
    {
        bodyPicked = true;
    }
    
}
