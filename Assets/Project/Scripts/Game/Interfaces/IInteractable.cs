using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string InteractHint { get; set; }

    public void Interact(GameObject interactor)
    {

    }

    public void UpdateFocus(GameObject interactor)
    {

    }
}
