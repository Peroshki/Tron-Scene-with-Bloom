using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A simple script which attaches the X-Ray shader to a secondary camera
// Based on the tutorial by Catlike Coding (https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/)


[ExecuteInEditMode]
public class XRayReplacement : MonoBehaviour
{
    public Shader XRayShader;

    void OnEnable()
    {
        GetComponent<Camera>().SetReplacementShader(XRayShader, "XRay");
    }
}