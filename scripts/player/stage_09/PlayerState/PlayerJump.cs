using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerStates
{

    [Header("Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private int maxJump = 2;


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
    }

    private bool CanJump()
    {
        if(!_playerController.Conditions.IsCollidingBelow && JumpsLeft <= 0 )
        {
            return false;
        }

        return true;
    }
}
