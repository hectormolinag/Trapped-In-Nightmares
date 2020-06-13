using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObjets : MonoBehaviour
{
	[SerializeField] private Material outlineMaterial;
	[SerializeField] private float outlineScaleFactor;
	[SerializeField] private Color outlineColor;
	
	private Renderer outlineRenderer;
	
	private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
	private static readonly int ScaleFactor = Shader.PropertyToID("_ScaleFactor");

	void Start()
	{
		outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);
	}

	Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color){

		GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation ,transform);

		Renderer rend = outlineObject.GetComponent<Renderer>();
		rend.material = outlineMat;
		rend.material.SetColor(OutlineColor, color);
		rend.material.SetFloat(ScaleFactor, scaleFactor);
		rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		outlineObject.GetComponent<OutlineObjets>().enabled = false;
		outlineObject.GetComponent<Collider>().enabled = false;
		rend.enabled = false;

		return rend;
	}

	private void OnMouseEnter()
	{
		outlineRenderer.enabled = true;
	}

	private void OnMouseExit()
	{
		outlineRenderer.enabled = false;
	}
}