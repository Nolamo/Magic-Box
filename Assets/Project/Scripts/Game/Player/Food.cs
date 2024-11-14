using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Food : InteractableGrabbableProp
{
    [field: SerializeField] public bool isEdible { get; private set; }
    // value to replenish hunger stat with
    [field: SerializeField] public float value { get; private set; }
    public UnityEvent OnEat;

    [SerializeField] private int _bites = 1;

    public override void Interact(GameObject interactor)
    {
        if(interactor.TryGetComponent(out Hunger hunger))
        {
            hunger.TryEat(this);
        }
    }

    public void Eat(Hunger hunger)
    {
        // depletes bites
        _bites--;

        hunger.stat.SetValue(hunger.stat.value + value);

        if(_bites <= 0) { Destroy(gameObject); }
    }
}
