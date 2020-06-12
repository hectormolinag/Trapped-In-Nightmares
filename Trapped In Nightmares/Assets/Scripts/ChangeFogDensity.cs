using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFogDensity : MonoBehaviour
{
    [SerializeField] private float newFogDensity = 0f;
    [SerializeField] float timeToMove = 2f;
    
    private bool canStartChanging = false;
    float currentTime = 0f;
    private float currentFogDensity = 0f;

    private void Update()
    {
        if (canStartChanging)
        {
            if (currentTime <= timeToMove)
            {
                currentTime += Time.deltaTime;
                RenderSettings.fogDensity = Mathf.Lerp(currentFogDensity, newFogDensity, currentTime / timeToMove);
            }
            else
            {
                //transform.position.x = 2.3f;
                currentTime = 0f;
                canStartChanging = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentFogDensity = RenderSettings.fogDensity;
            canStartChanging = true;
        }
    }
}
