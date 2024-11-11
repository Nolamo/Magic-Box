using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// interactor pattern
public class Interactor : MonoBehaviour
{
    public GameObject focusedObject { get; set; }
    public GameObject grabbedObject { get; set; }
    public Rigidbody grabbedRB { get; set; }

    public virtual void Interact()
    {
        if (focusedObject == null) return;

        if (focusedObject.TryGetComponent(out Prop prop))
        {
            if(prop is IInteractable interactable)
            {
                Debug.Log($"interacting with {prop.name}");
                interactable.Interact(gameObject);
            }
        }
    }

    public void SetFocusedObject(GameObject focusedObject)
    {
        if(focusedObject == null)
        {
            this.focusedObject = null; 
            return;
        }

        if (focusedObject == this.focusedObject) return;

        Debug.Log($"Focused Object:{focusedObject.name}");
        this.focusedObject = focusedObject;
    }

    public virtual void Grab()
    {

        if (focusedObject == null) return;
        if (grabbedObject != null) return;

        if (focusedObject.TryGetComponent(out Prop prop))
        {
            if (prop is IGrabbable grabbable)
            {
                Debug.Log($"grabbing {prop.name}");
                grabbedObject = prop.gameObject;
                grabbedRB = grabbedObject.GetComponent<Rigidbody>();
                grabbable.Grab(gameObject);
            }
        }
    }

    public virtual void Drop()
    {
        if (grabbedObject == null) return;

        if (grabbedObject.TryGetComponent(out Prop prop))
        {
            if (prop is IGrabbable grabbable)
            {
                grabbedObject = null;
                grabbable.Drop(gameObject);
            }
        }
    }
}
