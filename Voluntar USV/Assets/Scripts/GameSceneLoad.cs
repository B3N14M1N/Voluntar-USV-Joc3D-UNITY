using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneLoad : MonoBehaviour
{
    [Header ("Scene elements")]
    public Transform mainCamera;
    private Vector3 cameraPos;
    private Vector3 cameraRot;
    public NodeManager nodeManager;
    public GamePlayManager gamePlayManager;

    [Header ("Missions")]
    public List<Mission> scriptableMissions = new List<Mission>();
    public List<Mission> randomlySelectedMissions = new List<Mission>();
    public GameObject missionPannel;
    public GameObject completedMessage;
    public GameObject endedGameMessage;

    public MissionDisplay missionPannel1;
    public MissionDisplay missionPannel2;
    public MissionDisplay missionPannel3;

    [Header ("Menu elements")]
    public GameObject menuPannel;
    public GameObject menuOpenButton;
    public GameObject restartButton;
    public Button playButton;

    // called zero
    void Awake()
    {
        Debug.Log("Awake");
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        cameraPos = mainCamera.position;
        cameraRot = mainCamera.eulerAngles;

        randomlySelectedMissions = new List<Mission>();
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        while (randomlySelectedMissions.Count < 3)
        {
            int i = UnityEngine.Random.Range(0, scriptableMissions.Count);
            var mission = scriptableMissions[i];
            if(!randomlySelectedMissions.Contains(mission))
            {
                randomlySelectedMissions.Add(mission);
            }
        }
        gamePlayManager.LoadScript(randomlySelectedMissions);
        SetMissionPannelInfo();
        playButton.interactable = false;
    }

    public void Update()
    {
        playButton.interactable = gamePlayManager.isOkToStart;
        /*
        playButton.interactable = true;
        if (gamePlayManager.finalNodes.Count < 2) {
            playButton.interactable = false;
        }
        */
        if (gamePlayManager.gameEnded)
        {
            if (gamePlayManager.gameCompleted)
            {
                missionPannel.SetActive(false);
                endedGameMessage.SetActive(false);
                completedMessage.SetActive(true);
                restartButton.SetActive(false);
            }
            else
            {
                missionPannel.SetActive(false);
                completedMessage.SetActive(false);
                endedGameMessage.SetActive(true);
                restartButton.SetActive(true);
            }
        }
    }
    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void StartGame()
    {
        gamePlayManager.GameStart();
        playButton.interactable = false;
    }
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void Menu()
    {
        if (menuOpenButton.activeSelf)
        {
            menuOpenButton.SetActive(false);
            menuPannel.SetActive(true);
        }
        else
        {
            menuOpenButton.SetActive(true);
            menuPannel.SetActive(false);
        }
    }
    public void Restart()
    {
        playButton.interactable = false;
        restartButton.SetActive(true);
        missionPannel.SetActive(true);
        endedGameMessage.SetActive(false);
        completedMessage.SetActive(false);
        mainCamera.position = cameraPos;
        mainCamera.eulerAngles = cameraRot;
        gamePlayManager.LoadScript(randomlySelectedMissions);
        SetMissionPannelInfo();
    }
    public void SetMissionPannelInfo()
    {
        missionPannel1.SetText(randomlySelectedMissions[0]);
        missionPannel2.SetText(randomlySelectedMissions[1]);
        missionPannel3.SetText(randomlySelectedMissions[2]);
    }
}