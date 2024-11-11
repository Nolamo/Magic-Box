using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableGrabbableProp : MonoBehaviour, IInteractable, IGrabbable
{
    public virtual void Interact(GameObject interactor)
    {

    }

    public virtual void UpdateFocus(GameObject interactor)
    {

    }

    public virtual void Grab(GameObject grabber)
    {

    }

    public virtual void Drop(GameObject grabber)
    {

    }
}
