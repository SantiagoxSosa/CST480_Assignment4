using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCanvas : MonoBehaviour
{
    public Camera arCamera;
    public float threshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraVector = arCamera.transform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, cameraVector.normalized);
        if (dotProduct < -threshold)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
