using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoviment : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;

    private float _horizontalMovement;
    private float _movement;

    private int _idleAnimatorParameter = Animator.StringToHash("isIdle");
    private int _runAnimatorParameter = Animator.StringToHash("isWalk");

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

    public override void SetAnimation()
    {
        _animator.SetBool(_idleAnimatorParameter, _horizontalMovement == 0 && _playerController.Conditions.IsCollidingBelow);
        _animator.SetBool(_runAnimatorParameter, Mathf.Abs(_horizontalMovement) > 0.1f && _playerController.Conditions.IsCollidingBelow);
    }

    
}
