using UnityEngine;

public class MissionClass
{
    public Node startNode;
    public Node endNode;
    public string description;
    bool started = false;
    bool completed = false;
    public MissionClass(Mission mission)
    {
        foreach (GameObject objectNode in GameObject.FindGameObjectsWithTag("PathNode"))
        {
            if(objectNode.name == mission.startName)
            {
                this.startNode = objectNode.GetComponent<Node>();
                if (endNode != null) break;
            }
            if (objectNode.name == mission.endName)
            {
                this.endNode = objectNode.GetComponent<Node>();
                if (startNode != null) break;
            }
        }
        description = mission.Description;
        started = false;
        completed = false;
    }
    public bool Check(Node nodePassed)
    {
        if(!started && nodePassed == startNode) { started = true; }
        if(started && nodePassed == endNode) { completed = true; }
        return completed;
    }
    public bool Check()
    {
        return completed;
    }
}
