using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public GameObject gameObject {  get; }
    [field: SerializeField] public string InteractHint { get; set; }

    public bool isFocused { get; set; }

    [field: SerializeField] public UnityEvent<GameObject> OnFocused { get; set; }
    [field: SerializeField] public UnityEvent<GameObject> OnUnfocused { get; set; }
    [field: SerializeField] public UnityEvent<GameObject> OnInteract { get; set; }

    public void Interact(GameObject interactor)
    {
        OnInteract?.Invoke(interactor);
    }

    public void UpdateFocus(GameObject interactor)
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
}
