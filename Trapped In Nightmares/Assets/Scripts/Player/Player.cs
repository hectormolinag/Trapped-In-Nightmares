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

    private CharacterController controller;
    private Vector3 velocity;

    private float y;
    private bool isGrounded = true;
    private Transform groundChecker;
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
        
        controller = GetComponent<CharacterController>();
        groundChecker = transform.GetChild(0);
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
            
            velocity.y += Gravity * Time.deltaTime;
            velocity.y /= 1 + Drag.y * Time.deltaTime;
            y = velocity.y;
            velocity = velocity.normalized;
            velocity.y = y;
            controller.Move(velocity * Time.deltaTime);
        }
    
        // UPDATE CAMERA FOLLOW POINT POSITION
        if (_isCameraFollowPointNotNull)
        {
            if (isGrounded)
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
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (isGrounded && velocity.y < 0)
            velocity.y = 0f;

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(isHoldingBoxUp || isHoldingBoxDown)
        {
            move.x = 0f;
        }
        if(isHoldingBoxRight || isHoldingBoxLeft)
        {
            move.z = 0f;
        }
        
        controller.Move(Vector3.ClampMagnitude(move, 1) * (Time.deltaTime * Speed));

        //Jump Logic
        if (isHoldingBoxUp || isHoldingBoxDown || isHoldingBoxRight || isHoldingBoxLeft) return;
        
        if (move != Vector3.zero)
            transform.forward = move;
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(-JumpHeight * Gravity);
            anim.SetTrigger(Jump);
        }

        velocity.y += Gravity * Time.deltaTime;

        velocity.x /= 1 + Drag.x * Time.deltaTime;
        velocity.y /= 1 + Drag.y * Time.deltaTime;
        velocity.z /= 1 + Drag.z * Time.deltaTime;

        y = velocity.y;

        velocity = velocity.normalized;

        velocity.y = y;

        controller.Move(velocity * Time.deltaTime);
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
