using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoviment : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;

    private float _horizontalMovement;
    private float _movement;

    private int _idleAnimatorParameter = Animator.StringToHash("Idle");
    private int _runAnimatorParameter = Animator.StringToHash("Run");

    public override void ExecuteState()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if(Mathf.Abs(_horizontalMovement) > 0.01f)
        {
            _movement = _horizontalMovement;
        }
        else
        {
            _movement = 0f;
        }

        float moveSpeed = _movement * speed;
        _playerController.SetHorizontalForce(moveSpeed);
    }

    protected override void GetInput()
    {
        _horizontalMovement = _horizontalInput;
    }

    
}
