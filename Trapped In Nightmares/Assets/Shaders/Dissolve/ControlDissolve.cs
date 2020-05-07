using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDissolve : MonoBehaviour
{
    Material mat;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        mat.SetFloat("_DissolveAmount", Mathf.Lerp(-3, 3, Mathf.PingPong(Time.time * 0.25f, 1)));
    }
}
