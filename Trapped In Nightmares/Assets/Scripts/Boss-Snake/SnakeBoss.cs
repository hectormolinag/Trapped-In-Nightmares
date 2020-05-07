using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBoss : MonoBehaviour
{
    private enum States
    {
        Idle,
        Phase1,
        Phase2,
        ReturnBox
    };

    public SkinnedMeshRenderer rend;
    public Animator snakeAnim;

    void Start()
    {
        
    }


    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            StartCoroutine(Damaged());
        }
    }

    IEnumerator Damaged()
    {
        rend.material.SetColor("_BaseColor", Color.red);
        snakeAnim.SetTrigger("Damage");
        yield return new WaitForSeconds(2f);
        rend.material.SetColor("_BaseColor", Color.white);
    }

}
