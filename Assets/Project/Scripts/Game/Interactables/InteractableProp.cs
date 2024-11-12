using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableProp : Prop, IInteractable
{
    [field: SerializeField] public string InteractHint { get; set; } = "Interact";

    public virtual void Interact(GameObject interactor)
    {
        Debug.LogWarning($"undefiend interact behavior on {gameObject.name}");
    }

    public virtual void UpdateFocus(GameObject interactor)
    {

    }
}
