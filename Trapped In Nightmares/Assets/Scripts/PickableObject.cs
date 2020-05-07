using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public ParticleSystem glowParticle;
    public ParticleSystem interactionParticle;
    public Material mat;
    
    private bool isPlayerNear = false;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

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
                StartCoroutine(Dissolve());
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
            yield return new WaitForSeconds(0.008f);
        } 
        gameObject.SetActive(false);
    }
}
