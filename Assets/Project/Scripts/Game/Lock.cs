using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : InteractableProp
{
    [SerializeField] private Prop _door;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        IDoor door = _door as IDoor;

        if (door.isLocked)
        {
            door.Unlock();
        }
        else
        {
            door.Lock();
        }
    }
}
