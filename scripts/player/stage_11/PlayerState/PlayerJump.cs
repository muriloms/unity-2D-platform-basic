using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerStates
{

    [Header("Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private int maxJump = 2;

    private int _jumpAnimatorParameter = Animator.StringToHash("isJump");
    private int _doubleJumpAnimatorParameter = Animator.StringToHash("isDoubleJump");
    private int _fallJumpAnimatorParameter = Animator.StringToHash("isFall");


    public int JumpsLeft {get;set;}

    protected override void InitState()
    {
        base.InitState();
        JumpsLeft = maxJump;
    }

    public override void ExecuteState()
    {
        if(_playerController.Conditions.IsCollidingBelow && _playerController.Force.y == 0f)
        {
            JumpsLeft = maxJump;
            _playerController.Conditions.IsJumping = false;
        }
    }

    protected override void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if(!CanJump())
        {
            return;
        }
        if(JumpsLeft == 0)
        {
            return;
        }

        JumpsLeft--;
        
        float jumpForce = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(_playerController.Gravity));

        // Aplicar forca no eixo y do player
        _playerController.SetVerticalForce(jumpForce);

        _playerController.Conditions.IsJumping = true;
    }

    private bool CanJump()
    {
        if(!_playerController.Conditions.IsCollidingBelow && JumpsLeft <= 0 )
        {
            return false;
        }

        return true;
    }

    public override void SetAnimation()
    {
        //Jump
        _animator.SetBool(_jumpAnimatorParameter, _playerController.Conditions.IsJumping  
                                                    && !_playerController.Conditions.IsCollidingBelow 
                                                    && JumpsLeft > 0
                                                    && !_playerController.Conditions.IsFalling
                                                    && !_playerController.Conditions.IsFly);

        // Double Jump
        _animator.SetBool(_doubleJumpAnimatorParameter, _playerController.Conditions.IsJumping
                                                    && !_playerController.Conditions.IsCollidingBelow
                                                    && JumpsLeft == 0
                                                    && !_playerController.Conditions.IsFalling
                                                    && !_playerController.Conditions.IsFly);

        // Fall
        _animator.SetBool(_fallJumpAnimatorParameter, _playerController.Conditions.IsFalling
                                                    && _playerController.Conditions.IsJumping
                                                    && !_playerController.Conditions.IsCollidingBelow
                                                    && !_playerController.Conditions.IsFly);
    }
}
