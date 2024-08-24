using System.Collections.Generic;
using UnityEngine;

public class Node: MonoBehaviour
{
    public float Radius = 1f;
    public bool skip = false;

    public Vector3 Position { get { return transform.position; } }
    public List<Node> paths = new List<Node>();
    
    public List<Node> SortPaths()
    {
        for (int i = 0; i < paths.Count-1; i++)
        {
            for (int j = i; j < paths.Count; j++)
            {
                if ((paths[i].Position - Position).sqrMagnitude >= (paths[j].Position - Position).sqrMagnitude)
                {
                    (paths[j], paths[i]) = (paths[i], paths[j]);
                }
            }
        }
        return paths;
    }

    public Node AddPath()
    {
        GameObject newNode = new GameObject();
        newNode.transform.position = Position;
        newNode.transform.gameObject.name = name + "."+ paths.Count;
        newNode.tag = "PathNode";

        newNode.layer = 6;
        newNode.transform.gameObject.AddComponent<SphereCollider>();
        newNode.transform.gameObject.GetComponent<SphereCollider>().radius = 1f;

        newNode.AddComponent<Node>();
        newNode.GetComponent<Node>().paths.Add(this);
        newNode.GetComponent<Node>().Radius = 1f;

        paths.Add(newNode.GetComponent<Node>());
        SortPaths();

        return newNode.GetComponent<Node>();
    }

    public Node RemoveNode()
    {
        Node closestNode = null;
        //if (paths.Count > 0)
            foreach(Node node in paths)
            {
                if (node != null)
                {
                    closestNode = node;
                    break;
                }
            }

        for(int i=0;i<paths.Count;i++)
        {
            paths[i]?.paths?.Remove(this);
        }

        paths.Clear();
        GameObject.DestroyImmediate(this.GetComponent<SphereCollider>(),true);
        GameObject.DestroyImmediate(this.gameObject);

        return closestNode;
    }
    public void MergePaths(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            if (!paths.Contains(node) && node != this)
            {
                paths.Add(node);
                if(!node.paths.Contains(this))
                    node.paths.Add(this);
            }
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

#if UNITY_EDITOR_WIN
        if(!skip)
            Gizmos.DrawSphere(Position, Radius/2f);
#endif

        foreach (Node node in paths)
        {
            Gizmos.DrawLine(Position, node.Position);
        }
    }
}
