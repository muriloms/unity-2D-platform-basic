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
    [SerializeField] private int horizontalRayAmount = 4; // numero de raycast para colisao horizontal

    // Orientacao do player
    public bool FacingRight { get; set; }

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

    private float _internalFaceDirection = 1f;
    private float _faceDirection;

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
        
        ApplyGravity();
        StartMovement();
        
        
        SetRayOrigins();
        GetFaceDirection();

        if (FacingRight)
        {
            CollisionHorizontal(1);
        }
        else
        {
            CollisionHorizontal(-1);
        }

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

    private void CollisionHorizontal(int direction)
    {
        Vector2 rayHorizontalBottom = (_boundsBottomLeft + _boundsBottomRight) / 2f;
        Vector2 rayHorizontalTop = (_boundsTopLeft + _boundsTopRight) / 2f;

        rayHorizontalBottom += (Vector2)transform.up * _skin;
        rayHorizontalTop -= (Vector2)transform.up * _skin;

        float rayLenght = Mathf.Abs(_force.x * Time.deltaTime) + _boundsWidth / 2f + _skin * 2f;

        for (int i = 0; i < horizontalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(rayHorizontalBottom, rayHorizontalTop, (float)i / (horizontalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction * transform.right, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, transform.right * rayLenght * direction, Color.blue);

            if (hit)
            {
                if (direction >= 0)
                {
                    _movePosition.x = hit.distance - _boundsWidth / 2f - _skin * 2f;
                    _conditions.IsCollidingRight = true;
                }
                else
                {
                    _movePosition.x = -hit.distance + _boundsWidth / 2f + _skin * 2f;
                    _conditions.IsCollidingLeft = true;
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

    public void SetHorizontalForce(float xForce)
    {
        _force.x = xForce;
    }

    public void ApplyGravity()
    {
        _currentGravity = gravity;

        _force.y += _currentGravity * Time.deltaTime;

    }

    #region Direction
    private void GetFaceDirection()
    {
        _faceDirection = _internalFaceDirection;

        FacingRight = _faceDirection == 1;

        if(_force.x > 0.001f)
        {
            _faceDirection = 1f;
            FacingRight = true;
        }
        else if(_force.x < -0.001f)
        {
            _faceDirection = -1f;
            FacingRight = false;
        }

        _internalFaceDirection = _faceDirection;

        RotateModel();
    }

    private void RotateModel()
    {
        if (FacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    #endregion
}
