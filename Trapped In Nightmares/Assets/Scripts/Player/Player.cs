using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public Vector3 Drag;

    private CharacterController _controller;
    private Vector3 _velocity;

    private float y;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    private Animator anim;

    private Vector3 move;

    private GameObject draggableObjectInRange;

    [SerializeField] private bool isHoldingBoxLeft = false;
    [SerializeField] private bool isHoldingBoxRight = false;
    [SerializeField] private bool isHoldingBoxUp = false;
    [SerializeField] private bool isHoldingBoxDown = false;

    [SerializeField] private Transform CameraFollowPoint = null;

    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsPushing = Animator.StringToHash("isPushing");
    private static readonly int MoveX = Animator.StringToHash("moveX");
    private static readonly int MoveY = Animator.StringToHash("moveY");
    private bool _isCameraFollowPointNotNull;

    void Start()
    {
        _isCameraFollowPointNotNull = CameraFollowPoint != null;
        draggableObjectInRange = null;
        
        _controller = GetComponent<CharacterController>();
        _groundChecker = transform.GetChild(0);
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (!GameManager.Instance.isDialogueEnabled)
        {
            if (!GameManager.Instance.isRobotMovingFreely && GameManager.Instance.canPlayerMove)
                Movement();
            else
                move = Vector3.zero;

            if (draggableObjectInRange != null)
                PickBox();

            UpdateAnimations();
        }
        else
        {
            move = Vector3.zero;
            anim.SetBool(IsMoving, false);
            
            _velocity.y += Gravity * Time.deltaTime;
            _velocity.y /= 1 + Drag.y * Time.deltaTime;
            y = _velocity.y;
            _velocity = _velocity.normalized;
            _velocity.y = y;
            _controller.Move(_velocity * Time.deltaTime);
        }
    
        // UPDATE CAMERA FOLLOW POINT POSITION
        if (_isCameraFollowPointNotNull)
        {
            if (_isGrounded)
            {
                Vector3 posCameraPoint = CameraFollowPoint.position;
                var position = transform.position;
                CameraFollowPoint.position = position;
            }
            else
            {
                var position = transform.position;
                CameraFollowPoint.position = new Vector3(position.x, CameraFollowPoint.position.y, position.z);
            }
        }

    }

    private void Movement()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(isHoldingBoxUp || isHoldingBoxDown)
        {
            move.x = 0f;
        }
        if(isHoldingBoxRight || isHoldingBoxLeft)
        {
            move.z = 0f;
        }
        
        _controller.Move(Vector3.ClampMagnitude(move, 1) * (Time.deltaTime * Speed));

        //Jump Logic
        if (isHoldingBoxUp || isHoldingBoxDown || isHoldingBoxRight || isHoldingBoxLeft) return;
        
        if (move != Vector3.zero)
            transform.forward = move;
        
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
            anim.SetTrigger(Jump);
        }

        _velocity.y += Gravity * Time.deltaTime;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

        y = _velocity.y;

        _velocity = _velocity.normalized;

        _velocity.y = y;

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void UpdateAnimations()
    {
        if(move != Vector3.zero)
        {
            anim.SetBool(IsMoving, true);
        }
        else
        {
            anim.SetBool(IsMoving, false);
        }

        // Animations Push - Pull
        if(isHoldingBoxUp || isHoldingBoxDown || isHoldingBoxRight || isHoldingBoxLeft)
        {
            anim.SetBool(IsPushing, true);
        }
        else
        {
            anim.SetBool(IsPushing, false);
        }

        if(isHoldingBoxRight)
        {
            anim.SetFloat(MoveX, -move.x);
            return;
        }
        if(isHoldingBoxLeft)
        {
            anim.SetFloat(MoveX, move.x);
            return;
        }
        if(isHoldingBoxUp)
        {
            anim.SetFloat(MoveY, -move.z);
            return;
        }
        if(isHoldingBoxDown)
        {
            anim.SetFloat(MoveY, move.z);
            return;
        }

        anim.SetFloat(MoveX, 0);
        anim.SetFloat(MoveY, 0);
    }

    private void PickBox()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (draggableObjectInRange.transform.forward == transform.forward)    //Jugador está atrás de la caja
            {
                isHoldingBoxDown = true;
                Speed = 1f;
                draggableObjectInRange.transform.parent = transform;
            }
            if (draggableObjectInRange.transform.forward == -transform.forward)    // Jugador está adelante de la caja
            {
                isHoldingBoxUp = true;
                Speed = 1f;
                draggableObjectInRange.transform.parent = transform;
            }
            if (draggableObjectInRange.transform.forward == transform.right)   // Jugador está a la derecha de la caja
            {
                isHoldingBoxRight = true;
                Speed = 1f;
                draggableObjectInRange.transform.parent = transform;
            }
            if (draggableObjectInRange.transform.forward == -transform.right)   // Jugador está a la izquierda de la caja
            {
                isHoldingBoxLeft = true;
                Speed = 1f;
                draggableObjectInRange.transform.parent = transform;
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isHoldingBoxUp = false;
            isHoldingBoxDown = false;
            isHoldingBoxLeft = false;
            isHoldingBoxRight = false;
            Speed = 3f;
            draggableObjectInRange.transform.parent = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Draggable"))
        {
            draggableObjectInRange = other.gameObject;

            GameManager.Instance.isPlayerNearDraggable = true;
        }

        if (other.CompareTag("Acid"))
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Draggable"))
        {
            draggableObjectInRange = null;

            GameManager.Instance.isPlayerNearDraggable = false;
        }
    }


}
