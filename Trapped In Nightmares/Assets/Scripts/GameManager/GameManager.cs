using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public RobotMovement robot;
    public Transform CameraFollowPoint;

    public Image fadeImage;

    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera dialogueCamera;

    public bool isRobotMovingFreely = false;
    public bool isPlayerNearDraggable = false;
    public bool isDialogueEnabled = false;
    
    private float shakeElapsedTime = 0f;
    private CinemachineBasicMultiChannelPerlin cameraNoise;

    public Vector3 posToSpawnPlayer;
    public Vector3 posToSpawnRobot;

    public bool canPlayerMove = true;

    private void Awake() 
   {
       if (Instance == null)
       {
           Instance = this;
           //DontDestroyOnLoad (gameObject);
       }
       else
           Destroy(this);

   }

    private void Start()
    {
        StartCoroutine(FadeImage(true));
        cameraNoise = normalCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            isRobotMovingFreely = !isRobotMovingFreely;
        
        normalCamera.m_Follow = isRobotMovingFreely ? robot.transform : CameraFollowPoint;

        CameraShake();
    }

    public void ActivateNormalCamera()
    {
        normalCamera.enabled = true;
        dialogueCamera.enabled = false;

    }

    public void ActivateDialogueCamera()
    {
        dialogueCamera.enabled = true;
        normalCamera.enabled = false;

    }

    private void CameraShake()
    {
        if(shakeElapsedTime > 0f)
        {
            cameraNoise.m_AmplitudeGain = 2f;
            cameraNoise.m_FrequencyGain = 3f;

            shakeElapsedTime -= Time.deltaTime;
        }
        else
        {
            shakeElapsedTime = 0f;
            cameraNoise.m_AmplitudeGain = 0f;
            cameraNoise.m_FrequencyGain = 0f;
        }

        //print(shakeElapsedTime);
    }

    public void InvokeCameraShake(float time)
    {
        shakeElapsedTime = time;
    }

    public void GameOver()
    {

        StartCoroutine(Reset());


    }

    IEnumerator Reset()
    {
        CharacterController playerController = player.GetComponent<CharacterController>();
        canPlayerMove = false;
        StartCoroutine(FadeImage(false));
        yield return new WaitForSeconds(1.5f);

        playerController.enabled = false;
        Transform PlayerTf = player.transform;
        PlayerTf.position = posToSpawnPlayer;
        PlayerTf.rotation = Quaternion.identity;
        playerController.enabled = true;
        robot.transform.position = PlayerTf.position - (-PlayerTf.forward);
        StartCoroutine(FadeImage(true));
        canPlayerMove = true;

    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1.3f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1.3f; i += Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }

}
