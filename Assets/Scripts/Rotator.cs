using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Transform itemTr;
    public float rotSpeed = 60f; 
    void Start()
    {
        itemTr = transform;
    }

    void Update()
    {
        itemTr.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
    }
}
