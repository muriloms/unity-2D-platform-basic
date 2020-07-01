using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Settings")]
    [SerializeField] private Image flyImage;

    private float _currentTimeForFly;
    private float _flyForTime;
    
    void Update()
    {
        InternalTimeForFlyUpdate();
    }

    public void UpdateTimeForLfye(float currentTimeForFly, float maxTimeFlye)
    {
        _currentTimeForFly = currentTimeForFly;
        _flyForTime = maxTimeFlye;
    }

    private void InternalTimeForFlyUpdate()
    {
        flyImage.fillAmount = Mathf.Lerp(flyImage.fillAmount, _currentTimeForFly / _flyForTime, Time.deltaTime * 10f);
    }
}
