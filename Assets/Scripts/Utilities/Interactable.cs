using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IComparable
{
    public UnityEvent OnInteract;
    public GameObject interactUI;

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        Interactable otherInteractable = obj as Interactable;
        if (otherInteractable == null) throw new ArgumentException("Comparing Interactables but Object is not Interactable!");

        return this.transform.position.y.CompareTo(otherInteractable.transform.position.y);
    }

    public void PlayerInteract()
    {
        OnInteract.Invoke();
    }

    public void SetInteractUI(bool active)
    {
        if (active && interactUI.activeInHierarchy) return;
        interactUI.SetActive(active);
    }
}
