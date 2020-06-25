using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f; // parametro para força da gravidade
    // texto
    [SerializeField] private TMP_Text textOverHead;

    [Header("Settings")]
    [SerializeField] private LayerMask collideWith; // layer de colisao
    [SerializeField] private int verticalRayAmount = 4; // numero de raycast criados para colisao abaixo

    // referenciar boxcollider do gameobject
    private BoxCollider2D _boxCollider2D;

    // referenciar script de condicoes
    private PlayerConditions _conditions;

    // referenciar level component
    private LevelManager _levelManager;

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

    // distancia entre raycast e o contato real ("pele")
    private float _skin = 0.05f;

    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();

        _conditions = new PlayerConditions();
        _conditions.Reset();

        _levelManager = GameObject.FindGameObjectWithTag ("manager").GetComponent<LevelManager>();
        
        textOverHead.text = "Gravidade \n" + gravity.ToString();
    }

    void Update()
    {
        if(_levelManager.gravityIsActive){
            ApplyGravity();
            StartMovement();
        }
        

        
        SetRayOrigins();

        // colisao abaixo
        CollisionBelow();

        // Aplicar forcas no player
        transform.Translate(_movePosition, Space.Self);

        // frear aumento do tamano dos raycast
        SetRayOrigins();
        CalculateMovement();

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

    private void CollisionBelow(){

        if(_movePosition.y < -0.00f)
        {
            _conditions.IsFalling = true;
        }
        else
        {
            _conditions.IsFalling = false;
        }

        if (!_conditions.IsFalling)
        {
            _conditions.IsCollidingBelow = false;
            return;
        }
        
        // Calculate ray lenght
        float rayLenght = _boundsHeight / 2f + _skin;
        if(_movePosition.y < 0)
        {
            rayLenght += Mathf.Abs(_movePosition.y);
        }

        // Calculate ray origin
        Vector2 leftOrigin = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rightOrigin = (_boundsBottomRight + _boundsTopRight) / 2f;
        leftOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);
        rightOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);

        // Raycast
        for(int i = 0; i < verticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, (float)i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, -transform.up * rayLenght, Color.red);

            if (hit)
            {
                if(_force.y > 0)
                {
                    _movePosition.y = _force.y * Time.deltaTime;
                    _conditions.IsCollidingBelow = false;
                }
                else
                {
                    _movePosition.y = -hit.distance + _boundsHeight / 2f + _skin;
                }

                

                _conditions.IsCollidingBelow = true;
                _conditions.IsFalling = false;

                if (Mathf.Abs(_movePosition.y) < 0.0001f)
                {
                    _movePosition.y = 0f;
                }
            }
            
        }
    }

    private void CalculateMovement()
    {
        if(Time.deltaTime > 0)
        {
            _force = _movePosition / Time.deltaTime;
        }
    }

    private void StartMovement()
    {
        _movePosition = _force * Time.deltaTime;
        
    }

    public void ApplyGravity()
    {
        _currentGravity = gravity;

        _force.y += _currentGravity * Time.deltaTime;

    }
}
