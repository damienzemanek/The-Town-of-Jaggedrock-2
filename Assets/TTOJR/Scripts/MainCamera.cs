using UnityEngine;
using DependencyInjection;
using System.Collections.Generic;
using System;

public class MainCamera : MonoBehaviour, IDependencyProvider
{
    [Provide]
    public MainCamera Provide()
    {
        return this;
    }
    public Camera cam;
    public float castDist;
}
