using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [Header("UI")] 
    public GameObject dialogueBox;
    public Image fadeImage;
    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera dialogueCamera;
    [Header("Lighting and Fog")] 
    public Light globalSceneLight;
    
    public bool isRobotMovingFreely = false;
    public bool isPlayerNearDraggable = false;
    public bool isDialogueEnabled = false;
    
    private float shakeElapsedTime = 0f;
    private CinemachineBasicMultiChannelPerlin cameraNoise;

    public Vector3 posToSpawnPlayer;
    public Vector3 posToSpawnRobot;

    public bool canPlayerMove = true;

    private Coroutine ResetCoroutine;
    private Coroutine FadeCoroutine;

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

        if (globalSceneLight != null)
            globalSceneLight.intensity = 0.1f;

        RenderSettings.fogDensity = 0.01f;

    }

    void Update()
    {
        if (robot.gameObject.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(1))
                isRobotMovingFreely = !isRobotMovingFreely;
        
            normalCamera.m_Follow = isRobotMovingFreely ? robot.transform : CameraFollowPoint;
        }

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

    public void DefineTargetDialogueCamera(WhosTalking talker)
    {
        if (talker == WhosTalking.Kid)
        {
            dialogueCamera.m_Follow = player.transform;
        }
        else if (talker == WhosTalking.Robot)
            dialogueCamera.m_Follow = robot.transform;
          
        
    }

    public void GameOver()
    {
        ResetCoroutine = StartCoroutine(ResetGame());
    }

    public void Fade()
    {
        FadeCoroutine = StartCoroutine(FadeComplete());
        robot.transform.position = player.transform.position - (-player.transform.forward);
    }

    private IEnumerator ResetGame()
    {
        CharacterController playerController = player.GetComponent<CharacterController>();
        canPlayerMove = false;
        StartCoroutine(FadeImage(false));
        
        float t = Time.time;                                            // Use this instead of
        while(Time.time < t + 1.5f ){ yield return null; }              // yield return new WaitForSeconds(1.5f);
        
        playerController.enabled = false;
        Transform PlayerTf = player.transform;
        PlayerTf.position = posToSpawnPlayer;
        PlayerTf.rotation = Quaternion.identity;
        playerController.enabled = true;
        
        if(robot.gameObject.activeInHierarchy)
            robot.transform.position = PlayerTf.position - (-PlayerTf.forward);
        
        StartCoroutine(FadeImage(true));
        canPlayerMove = true;
        
        StopCoroutine(ResetCoroutine);

    }

    private IEnumerator FadeImage(bool fadeAway)
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

    private IEnumerator FadeComplete()
    {
        StartCoroutine(FadeImage(false));
        
        float t = Time.time;                                 
        while(Time.time < t + 1.5f) { yield return null; } 

        StartCoroutine(FadeImage(true));
        
        StopCoroutine(FadeCoroutine);
    }

    public void ChangeIntensityGlobalIlumination(float newIntensity)
    {
        if(globalSceneLight != null)
            globalSceneLight.intensity = newIntensity;
    }

    public void ChangeFogIntensity(float newIntensity)
    {
        RenderSettings.fogDensity = newIntensity;
    }


}
