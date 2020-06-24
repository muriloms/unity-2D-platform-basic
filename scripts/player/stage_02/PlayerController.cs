using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f; // parametro para força da gravidade


    // referenciar boxcollider do gameobject
    private BoxCollider2D _boxCollider2D;

    // limites com base no boxcollider
    private Vector2 _boundsTopLeft;
    private Vector2 _boundsTopRight;
    private Vector2 _boundsBottomLeft;
    private Vector2 _boundsBottomRight;

    // largura e altura dos limites
    private float _boundsWidth;
    private float _boundsHeight;

    // parametros para controlar gravidade
    private float _currentGravity;
    private Vector2 _force;
    private Vector2 _movePosition;

    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        ApplyGravity();
        StartMovement();

        
        SetRayOrigins();

        // desenhar limites na cena
        Debug.DrawRay(_boundsBottomLeft, Vector2.left, Color.green);
        Debug.DrawRay(_boundsBottomRight, Vector2.right, Color.green);
        Debug.DrawRay(_boundsTopLeft, Vector2.left, Color.green);
        Debug.DrawRay(_boundsTopRight, Vector2.right, Color.green);
    }

    // Funcao para definir limites com base no boxcollider
    private void SetRayOrigins()
    {
        Bounds playerBounds = _boxCollider2D.bounds;

        _boundsTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        _boundsTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);
        _boundsBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        _boundsBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);

        _boundsHeight = Vector2.Distance(_boundsBottomLeft, _boundsTopLeft);
        _boundsWidth = Vector2.Distance(_boundsBottomLeft, _boundsBottomRight);
    }

    private void StartMovement()
    {
        _movePosition = _force * Time.deltaTime;
        
    }

    private void ApplyGravity()
    {
        _currentGravity = gravity;

        _force.y += _currentGravity * Time.deltaTime;

    }
}
