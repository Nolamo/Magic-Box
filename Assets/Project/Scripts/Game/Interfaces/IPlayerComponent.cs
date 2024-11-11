using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent
{
    void SubscribeInputs();

    void UnsubscribeInputs();
}
