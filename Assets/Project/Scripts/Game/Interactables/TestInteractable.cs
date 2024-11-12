using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : InteractableProp
{
    public override void Interact(GameObject interactor)
    {
        Debug.Log($"{interactor.name} interacted with this!");
    }
}
