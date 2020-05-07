using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameManager.Instance.posToSpawnPlayer = transform.position;
			gameObject.SetActive(false);
			Debug.Log("Saved Pos CheckPoint " + GameManager.Instance.posToSpawnPlayer);
		}
	}
}
