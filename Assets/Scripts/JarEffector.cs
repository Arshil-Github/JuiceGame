using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JarEffector : MonoBehaviour
{
    public Jar jar;

    private void Start()
    {
        
    }
    private void OnMouseDown()
    {
        jar.HoldStart();
    }
    private void OnMouseDrag()
    {
        jar.Dragging();
    }
    private void OnMouseUp() {
        jar.HoldEnd();
    }
}
