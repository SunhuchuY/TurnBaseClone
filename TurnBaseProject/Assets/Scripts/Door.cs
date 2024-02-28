using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    private Animator animator;
    private GridPosition gridPosition;
    private float timer;
    private bool isActive;
    private Action onInteractComplete;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }   

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0f) 
        {
            isActive = false;
            onInteractComplete();
        }
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, false);
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, true);
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}
