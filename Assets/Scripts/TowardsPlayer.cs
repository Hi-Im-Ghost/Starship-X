using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowardsPlayer : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }


    void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
