using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [SerializeField] Transform target = null;
    [SerializeField] float speed = 0f;
    [SerializeField] GameObject head = null;

    [SerializeField] ParticleSystem chargingFlash = null;
    [SerializeField] ParticleSystem releaseFlash = null;
    [SerializeField] ParticleSystem chargeComplete = null;

    [SerializeField] Light lightRobot = null;
    [SerializeField] Material robotMAT = null;

    [SerializeField] GameObject laserBeam = null;

    //public float headRotation;

    private bool canFollowTarget = false;
    private Animator anim;
    private float EmissionColorValue = 0.5f;

    private Vector3 inputDirection;
    float currentTime = 0f;

    bool startCharging = false;

    bool isChargeComplete = false;

    float startHeight;

    private Coroutine shootLaserBeamCoroutine;
    
    private static readonly int CanWalk = Animator.StringToHash("canWalk");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Camera _camera;

    /*
    private void Awake()
    {
        if (GameManager.Instance.robot == null)
            GameManager.Instance.robot = GetComponent<RobotMovement>();
    }
*/
    void Start()
    {
        _camera = Camera.main;
        laserBeam.SetActive(false);
        startHeight = transform.position.y;
        anim = GetComponent<Animator>();

        robotMAT.EnableKeyword("_EMISSION");

        lightRobot.intensity = 20f;
        currentTime = 0f;
        EmissionColorValue = 1f;
        robotMAT.SetColor(EmissionColor, new Color(EmissionColorValue, EmissionColorValue, EmissionColorValue));

        transform.position = GameManager.Instance.posToSpawnRobot;
    }

    void Update()
    {
        if (!GameManager.Instance.isDialogueEnabled)
        {
            if (GameManager.Instance.isRobotMovingFreely)
            {
                ManualMovement();
            }
            else
                MovementAndAnimations();

            ShootFlash();
        }

    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isDialogueEnabled)
        {
            HeadRotation();
        }
    }

    private void ShootFlash()
    {
        if (!GameManager.Instance.isPlayerNearDraggable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startCharging = true;
                releaseFlash.Play();
            }

            if (Input.GetMouseButton(0))
            {
                if (startCharging)
                {
                    chargingFlash.Play();
                    currentTime = 0f;
                    startCharging = false;
                }

                currentTime += Time.deltaTime;
                EmissionColorValue = Mathf.Lerp(1f, 3f, currentTime / 2f);

                if (EmissionColorValue == 3f && !isChargeComplete)
                {
                    isChargeComplete = true;
                    chargingFlash.Stop();
                    chargeComplete.Play();
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                if (EmissionColorValue == 3)
                {
                    GameManager.Instance.InvokeCameraShake(2f);

                    chargeComplete.Stop();
                    isChargeComplete = false;
                    chargingFlash.Stop();

                    //releaseFlash.Play();
                    shootLaserBeamCoroutine = StartCoroutine(ShootLaserBeam());

                    currentTime = 0f;
                    lightRobot.intensity = 0f;
                    EmissionColorValue = 0f;
                }
                else
                {
                    chargingFlash.Stop();
                    currentTime = 0f;
                    lightRobot.intensity = 20f;
                    EmissionColorValue = 1f;

                }

                
            }

            if ((EmissionColorValue < 1f) && !chargingFlash.isPlaying)
            {
                currentTime += Time.deltaTime;
                EmissionColorValue = Mathf.Lerp(0f, 1f, currentTime / 3f);
                lightRobot.intensity = EmissionColorValue * 20f;
            }


            robotMAT.SetColor(EmissionColor, new Color(EmissionColorValue, EmissionColorValue, EmissionColorValue));
        }
    }

    private void MovementAndAnimations()
    {
        var transform1 = target.transform;
        var position = transform1.position;
        Vector3 targetLookAt = new Vector3(position.x, transform.position.y, position.z);
        transform.LookAt(targetLookAt);

        if (canFollowTarget)
        {
            anim.SetBool(CanWalk, true);
            var position1 = transform.position;
            position1 = Vector3.MoveTowards(position1, target.position, speed * Time.deltaTime);
            position1 = new Vector3(position1.x, position1.y, position1.z);
            transform.position = position1;
        }
        else
        {
            anim.SetBool(CanWalk, false);
        }

        if (Vector3.Distance(transform.position, target.position) <= 3f)
        {
            speed = 0.7f;
        }
        if (Vector3.Distance(transform.position, target.position) >= 3f)
        {
            speed = 2f;
        }
        if (Vector3.Distance(transform.position, target.position) >= 5f)
        {
            speed = 4f;
        }
    }
    
    private void HeadRotation()
    {
        if (_camera != null)
        {
            Ray cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            float rayLenght;

            if(groundPlane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLenght);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.red);

                head.transform.LookAt(new Vector3(pointToLook.x, head.transform.position.y, pointToLook.z));

                // Fix Rotatated Bone Issue
                Vector3 rotationEuler = head.transform.rotation.eulerAngles;
                head.transform.rotation = Quaternion.Euler(new Vector3(rotationEuler.x, rotationEuler.y + 270, rotationEuler.z));
            }
        }
    }

    private void ManualMovement()
    {
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");

        transform.position += inputDirection.normalized * (3 * Time.deltaTime);

        if (inputDirection != Vector3.zero)
        {
            transform.forward = inputDirection.normalized;
            anim.SetBool(CanWalk, true);
        }
        else
            anim.SetBool(CanWalk, false);
    }

    IEnumerator ShootLaserBeam()
    {
        laserBeam.SetActive(true);
        yield return new WaitForSeconds(2f);
        laserBeam.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            canFollowTarget = false; 
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            canFollowTarget = true;
        }
    }
}
