using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLineRenderer : MonoBehaviour
{
    public LineRenderer renderer;
    public GameObject object1;
    public GameObject object2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(renderer != null)
        {
            renderer.SetPosition(0, object1.transform.position);
            renderer.SetPosition(1, object2.transform.position);
        }
    }
}
