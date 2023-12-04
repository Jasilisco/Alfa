using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraSetUp : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private bool initialized = false;
    public void initializeCamera()
    {
        cam = gameObject.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        cam.AddCinemachineComponent<CinemachineFramingTransposer>();
        initialized = true;
    }

    public void updateCameraSize(float cameraHeight)
    {
        if (initialized == true)
        {
            cam.m_Lens.OrthographicSize = cameraHeight;
        }
        else
        {
            throw new Exception("Camara no inicializada");
        }
    }
}
