using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class InteractableGrabbableProp : Prop, IInteractable, IGrabbable
{

    [field: SerializeField] public string InteractHint { get; set; } = "Interact";
    GameObject IInteractable.gameObject { get => gameObject; }
    public bool isFocused { get; set; }
    [field: SerializeField] public UnityEvent<GameObject> OnFocused { get; set; }
    [field: SerializeField] public UnityEvent<GameObject> OnUnfocused { get; set; }
    [field: SerializeField] public UnityEvent<GameObject> OnInteract { get; set; }

    public virtual void Interact(GameObject interactor)
    {
        OnInteract?.Invoke(interactor);
    }

    public virtual void UpdateFocus(GameObject interactor)
    {
        if(interactor.TryGetComponent(out Interactor interactorComponent) && interactorComponent.focusedObject == gameObject)
        {
            OnFocused?.Invoke(interactor);
        }
        else
        {
            OnUnfocused?.Invoke(interactor);
        }
    }

    public virtual void Grab(GameObject grabber)
    {

    }

    public virtual void Drop(GameObject grabber)
    {

    }
}
