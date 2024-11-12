using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : PlayerStatContainer
{
    [SerializeField] private float _starvingThreshold;
    public bool isStarving { get; private set; }


    private void Awake()
    {
        stat.OnStatChanged += OnStatChanged_Update;
    }

    void OnStatChanged_Update (object sender, float value)
    {
        isStarving = value < _starvingThreshold;
    }

    public bool TryEat(Food food)
    {
        // only eat edible foods while the player is not full
        // allow the player to eat non-edible food items while starving
        bool full = stat.value > maximum;
        bool canEat = (food.isEdible || isStarving) && !full;

        if (canEat)
        {
            stat.SetValue(stat.value + food.value);
        }

        return canEat;
    }
}
