using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFly : PlayerStates
{

    [Header("Settings")]
    [SerializeField] private float flyForce = 3f;
    [SerializeField] private float timeForFly = 5f;
    [SerializeField] private float timeForFlyAgain = 0.5f;

    private float _remainingFlightTime;
    private float _durationFly;
    private bool _stillCanFly = true;

    private int _jetpackAnimatorParameter = Animator.StringToHash("isFly");

    protected override void InitState()
    {
        base.InitState();

        _durationFly = timeForFly;
        _remainingFlightTime = timeForFly;

        UIManager.Instance.UpdateTimeForLfye(_remainingFlightTime, timeForFly);
    }
    protected override void GetInput()
    {
        if(Input.GetKey(KeyCode.X))
        {
            Fly();

        }

        if(Input.GetKeyUp(KeyCode.X))
        {
            EndFlye();
        }
    }

    // Acionar voo
    private void Fly()
    {
        if(!_stillCanFly) return;

        if(_remainingFlightTime <= 0)
        {
            EndFlye();
            _stillCanFly = false;
            return;
        }

        _playerController.SetVerticalForce(flyForce);
        _playerController.Conditions.IsFly = true;
        StartCoroutine(TimeToFly());

    }

    // Finalizar voo
    private void EndFlye()
    {
        _playerController.Conditions.IsFly = false;
        StartCoroutine(FlightTime());
    }

    // Tempo maximo de voo
    private IEnumerator TimeToFly()
    {
        float timeForCanFly = _remainingFlightTime;
        if(timeForCanFly > 0 && _playerController.Conditions.IsFly && _remainingFlightTime <= timeForCanFly)
        {
            timeForCanFly -= Time.deltaTime;
            _remainingFlightTime = timeForCanFly;

            UIManager.Instance.UpdateTimeForLfye(_remainingFlightTime, timeForFly);

            yield return null;
        }

    }

    // Tempo de voo
    private IEnumerator FlightTime()
    {
        yield return new WaitForSeconds(timeForFlyAgain);

        float againFly = _remainingFlightTime;

        while(againFly < timeForFly && !_playerController.Conditions.IsFly)
        {
            againFly += Time.deltaTime;
            _remainingFlightTime = againFly;

            UIManager.Instance.UpdateTimeForLfye(_remainingFlightTime, timeForFly);

            if(!_stillCanFly && againFly > 0.02f)
            {
                _stillCanFly = true;
            }

            yield return null;
        }
    }

    public override void SetAnimation()
    {
        _animator.SetBool(_jetpackAnimatorParameter, _playerController.Conditions.IsFly);
    }
}
