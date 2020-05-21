using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickableObjectsList
{
    Arms,
    Body
}

public class PickableObject : MonoBehaviour
{
    public PickableObjectsList objectType;
    public ParticleSystem glowParticle;
    public ParticleSystem interactionParticle;
    public Material mat;
    
    private bool isPlayerNear = false;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private Coroutine dissolveCoroutine;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat(DissolveAmount, -3f);      
        interactionParticle.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
                dissolveCoroutine = StartCoroutine(Dissolve());

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            interactionParticle.gameObject.SetActive(true);
            interactionParticle.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionParticle.gameObject.SetActive(false);
            interactionParticle.Stop();
        }
    }

    IEnumerator Dissolve()
    {
        glowParticle.Stop();
        interactionParticle.gameObject.SetActive(false);
        for (float i = 0; i < 1; i += 0.005f)
        {
            float dissolve = Mathf.Lerp(0, 2f, i);
            mat.SetFloat(DissolveAmount, dissolve);
            
            float t = Time.time;                                            // Use this instead of
            while(Time.time < t + 0.008f){ yield return null; }             // yield return new WaitForSeconds(0.008f);
        } 
        StopCoroutine(dissolveCoroutine);
        
        if(objectType == PickableObjectsList.Arms)
            EventsManager.current.GrabArmRobot();  //Dispatch the event
        else if(objectType == PickableObjectsList.Body)
            EventsManager.current.GrabBodyRobot();  //Dispatch the event
        
        gameObject.SetActive(false);
    }
}
