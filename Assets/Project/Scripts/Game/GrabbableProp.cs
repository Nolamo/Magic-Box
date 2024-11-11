using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableProp : Prop, IGrabbable
{
    public virtual void Grab(GameObject grabber)
    {

    }

    public virtual void Drop(GameObject grabber)
    {

    }
}
