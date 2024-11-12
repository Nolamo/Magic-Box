using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Food : InteractableGrabbableProp
{
    [field: SerializeField] public bool isEdible { get; private set; }
    [field: SerializeField] public float value { get; private set; }
    public UnityEvent OnEat;

    public override void Interact(GameObject interactor)
    {
        if(interactor.TryGetComponent(out Hunger hunger))
        {
            hunger.TryEat(this);
        }
    }
}
