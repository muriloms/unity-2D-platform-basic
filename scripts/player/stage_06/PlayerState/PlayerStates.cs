using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    protected PlayerController _playerController;
    protected Animator _animator;

    protected float _horizontalInput;
    protected float _verticalInput;

    protected virtual void Start()
    {
        InitState();
    }

    protected virtual void InitState()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();
    }

    public virtual void ExecuteState()
    {

    }

    public virtual void LocalInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        GetInput();
    }

    protected virtual void GetInput()
    {

    }

    public virtual void SetAnimation()
    {

    }
}
