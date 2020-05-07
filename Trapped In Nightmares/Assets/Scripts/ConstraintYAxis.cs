using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class ConstraintYAxis : MonoBehaviour
{
    private float yValue = 0f;
    private float xValue = 0f;
    private float zValue = 0f;
    void Start()
    {
        yValue = transform.position.y;
        xValue = transform.position.x;
        zValue = transform.position.z;
    }

    void Update()
    {
        if (transform.position.y != yValue)
            transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
        if (transform.position.x != xValue)
            transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
        if (transform.position.z != zValue)
            transform.position = new Vector3(transform.position.x, transform.position.y, zValue);
    }
}
