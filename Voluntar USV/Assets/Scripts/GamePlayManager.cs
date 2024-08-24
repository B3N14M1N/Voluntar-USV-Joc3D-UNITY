using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [Header("Path Manager")]
    private NodeManager nodeManager;
    private int selectedCount = 0;
    public List<Node> finalNodes = new List<Node>();
    private GameObject finalLine;
    private Color finalPathColor = Color.red;

    [Header("Avatar Settings")]
    public Transform avatar;
    public Vector3 originalAvatarPosition;
    public Vector3 originalAvatarRotation;
    private Animator animator;
    private bool idle = true;
    private bool walking = false;
    private bool jogging = false;

    [Header("Missions Settings")]
    public List<MissionClass> missions = new List<MissionClass>();
    private int index = 0;
    private bool gameStarted = false;
    public bool gameEnded = false;
    public bool gameCompleted = false;
    public float walkingSpeed = 12f;
    public bool isOkToStart = false;
    public void LoadScript(List<Mission> scriptableMissions)
    {
        selectedCount = 0;
        index = 0;
        gameStarted = false;
        gameEnded = false;
        gameCompleted = false;
        isOkToStart = false;
        nodeManager = GetComponent<NodeManager>();
        nodeManager.LoadScript();

        animator = avatar.GetComponentInChildren<Animator>();
        avatar.transform.position = originalAvatarPosition;
        avatar.eulerAngles = originalAvatarRotation;
        SetIdle();

        missions?.Clear();
        missions = new List<MissionClass>();
        foreach (Mission mission in scriptableMissions)
        {
            MissionClass newMission = new MissionClass(mission);
            missions.Add(newMission);
            nodeManager.AddStartMissionNode(newMission.startNode);
            nodeManager.AddEndMissionNode(newMission.endNode);
        }

        if (finalLine == null)
        {
            finalLine = new GameObject();
            finalLine.layer = nodeManager.pathLineLayer;
            finalLine.transform.parent = this.transform;
            LineRenderer lineRenderer = finalLine.AddComponent<LineRenderer>();
            lineRenderer.numCapVertices = 3;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.widthCurve = nodeManager.curve;
            lineRenderer.sharedMaterial = new Material(nodeManager.lineMaterial);
            lineRenderer.sharedMaterial.color = finalPathColor;
        }
        finalLine.SetActive(false);
        nodeManager.SetSelectionMode(true);
    }
    // Update is called once per frame
    void Update()
    {
        if(selectedCount != nodeManager.selectedNodes.Count)
        {
            selectedCount = nodeManager.selectedNodes.Count;
            finalNodes = nodeManager.finalPath();
            isOkToStart = nodeManager.CheckContainsRequiredNodes();
        }
        bool showPath = Input.GetKey(KeyCode.Space);
        if (showPath)
        {
            finalNodes = nodeManager.finalPath();
            LineRenderer lineRenderer = finalLine.GetComponent<LineRenderer>();
            lineRenderer.positionCount = finalNodes.Count;
            for(int i = 0; i < finalNodes.Count; i++)
            {
                lineRenderer.SetPosition(i, finalNodes[i].Position);
            }
        }
        finalLine.SetActive(showPath);

        if (gameStarted) {
            GameRunning();
        }
    }
    public void GameStart()
    {
        gameStarted = true;
        gameCompleted = false;
        gameEnded = false;
        isOkToStart = false;
        nodeManager.SetSelectionMode(false);
        finalNodes = nodeManager.finalPath();
        Debug.Log("Path length: " + finalNodes.Count);
        if (finalNodes.Count < 2)
        {
            Debug.LogError("EROARE");
        }
        else
        {
            index = 1;
            avatar.transform.position = finalNodes[0].Position;
            avatar.transform.LookAt(finalNodes[index].transform);
            avatar.transform.eulerAngles = new Vector3(0, avatar.transform.rotation.y, 0);
            SetJogging();
            foreach (var mission in missions)
            {
                mission.Check(finalNodes[index]);
            }
        }
    }
    public void GameEnded()
    {
        isOkToStart = false;
        gameCompleted = true;
        foreach (var mission in missions)
        {
            if (!mission.Check())
            {
                gameCompleted = false;
                break;
            }
        }
        gameEnded = true;
        SetIdle();
    }
    public void GameRunning()
    {
        if (avatar.transform.position == finalNodes[index].Position)
        {
            var missionsCompleted = true;
            foreach (var mission in missions)
            {
                if (!mission.Check(finalNodes[index]))
                {
                    missionsCompleted = false;
                }
            }
            if (missionsCompleted || index == finalNodes.Count - 1)
            {
                GameEnded();
            }
            else
            {
                index++;
                nodeManager.RenderCRoof(finalNodes[index]);
            }
        }
        else
        {
            var step = walkingSpeed * Time.deltaTime;
            avatar.transform.position = Vector3.MoveTowards(avatar.transform.position, finalNodes[index].Position, step);
            avatar.transform.LookAt(finalNodes[index].transform);
            avatar.eulerAngles = new Vector3(0, avatar.transform.eulerAngles.y, 0);
        }
    }
    private void SetIdle()
    {
        idle = true;
        jogging = false;
        walking = false;
        animator.SetBool("Idle", idle);
        animator.SetBool("Walking", walking);
        animator.SetBool("Jogging", jogging);
    }
    private void SetWalking()
    {
        idle = false;
        jogging = false;
        walking = true;
        animator.SetBool("Idle", idle);
        animator.SetBool("Walking", walking);
        animator.SetBool("Jogging", jogging);
    }
    private void SetJogging()
    {
        idle = false;
        walking = false;
        jogging = true;
        animator.SetBool("Idle", idle);
        animator.SetBool("Walking", walking);
        animator.SetBool("Jogging", jogging);
    }
}
