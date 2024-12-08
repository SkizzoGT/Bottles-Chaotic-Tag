using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhaseScript : MonoBehaviour
{
    [Header("THIS SCRIPT WAS MADE BY FIZZY NOT YOU, PLEASE GIVE CREDIT")]
    public Transform sphere;
    public Transform controller;

    void Update()
    {
        sphere.rotation = controller.rotation;
    }
}