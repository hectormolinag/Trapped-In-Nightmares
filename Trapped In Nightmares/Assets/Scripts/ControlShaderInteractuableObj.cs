using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShaderInteractuableObj : MonoBehaviour
{
    [SerializeField] private float intensity = 1f;
    
    private Material mat;
    private static readonly int ColorFactor = Shader.PropertyToID("_ColorFactor");

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void OnMouseEnter()
    {
        mat.SetFloat(ColorFactor, intensity);
    }

    private void OnMouseExit()
    {
        mat.SetFloat(ColorFactor, 0f);
    }
}
