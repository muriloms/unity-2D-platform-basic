using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool gravityIsActive {get;set;}

    void Start()
    {
        gravityIsActive = false;
    }

    void Update()
    {
        
    }

    public void ActivateGravity(){
        gravityIsActive = true;
    }
    public void DeactivateGravity(){
        gravityIsActive = false;
    }
}
