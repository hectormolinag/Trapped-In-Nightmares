using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] private float timeToFall = 1f;
    private Coroutine coroutine;
    private Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        EventsManager.current.onPlatformTriggerEnter += Fall;   //Subscribe event
    }

    private void Fall(int id)
    {
        coroutine = StartCoroutine(StartTimeToFall(id));
    }

    private IEnumerator StartTimeToFall(int id)
    {
        if(id == this.id)
        {
            mat.SetColor("_BaseColor", Color.red);

            yield return new WaitForSeconds(timeToFall);
            LeanTween.moveLocalY(gameObject, -10, 1f).setEaseOutQuad();
            yield return new WaitForSeconds(2f);

            ResetPlatform();
        }
    }

    private void ResetPlatform()
    {
         mat.SetColor("_BaseColor", new Color(0.5f, 0.5f, 0.5f, 1f));

        LeanTween.moveLocalY(gameObject, -1.3f, 1f).setEaseOutQuad();

        if(coroutine != null)
        StopCoroutine(coroutine);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            EventsManager.current.PlatformTriggerEnter(id);
        }
    }

}
