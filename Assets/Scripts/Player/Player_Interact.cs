using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interact : MonoBehaviour
{
    [SerializeField] private Player_Input_Controller playerInputController;
    private List<Interactable> interactables;

    private void Awake()
    {
        interactables = new List<Interactable>();
        playerInputController.playerInputAction = new Player_Input();
        playerInputController.playerInputAction.Character.Interact.started += ReadInteractButton;
    }

    private void Update() 
    {
        if (interactables.Count > 1)
        {
            interactables.Sort(new InteractCloserFirst(transform));
            interactables[0].SetInteractUI(true);
            for (int i = 1; i < interactables.Count; i++)
            {
                interactables[i].SetInteractUI(false);
            }
        }
    }   

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player") return;
        if (other.TryGetComponent<Interactable>(out Interactable component))
        {
            if (!interactables.Contains(component))
            {
                interactables.Add(component);
                component.SetInteractUI(true);
                Debug.Log("Interactables: " + interactables.Count);
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.tag == "Player") return;
        if (other.TryGetComponent<Interactable>(out Interactable component))
        {
            if (interactables.Contains(component))
            {
                interactables.Remove(component);
                component.SetInteractUI(false);
                Debug.Log("Interactables: " + interactables.Count);
            }
        }
    }

    private void ReadInteractButton(InputAction.CallbackContext context)
    {
        Debug.Log("Read interact button!");
        if (interactables.Count > 0)
        {
            interactables[0].PlayerInteract();
        }
    }
}

public class InteractCloserFirst : Comparer<Interactable>
{
    private Transform playerTransform;
    public InteractCloserFirst(Transform player)
    {
        playerTransform = player;
    }
    public override int Compare(Interactable x, Interactable y)
    {
        float distanceX = (playerTransform.position - x.transform.position).magnitude;
        float distanceY = (playerTransform.position - x.transform.position).magnitude;

        if (distanceX > distanceY) return 1;
        if (distanceX < distanceY) return -1;
        return 0;
    }
}
