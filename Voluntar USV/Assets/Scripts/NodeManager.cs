using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private bool SelectingMode = false;

    [Header("Checkpoints settings")]
    public GameObject checkpointObject;
    public Material checkpointMaterial;
    public Material lineMaterial;
    public Color selectedNodeColor = Color.cyan;
    public Color openNodeColor = Color.green;
    public Color startMissionNodeColor = Color.white;
    public Color endMissionNodeColor = Color.yellow;

    [Header("Node Manager")]
    public LayerMask layer;
    public Node startingNode;
    private Vector3 offsetY = new Vector3(0, 0.2f, 0);
    private List<Node> allNodes = new List<Node>();
    public List<Node> selectedNodes = new List<Node>();
    public GameObject C_roof;
    public List<Node> nearC = new List<Node>();
    private List<Node> startMissionNodes = new List<Node>();
    private List<Node> endMissionNodes = new List<Node>();

    [Header("Line Manager")]
    public Color pathColor = Color.green;
    public int pathLineLayer = 10;
    public AnimationCurve curve = new AnimationCurve();
    private List<GameObject> lineRenderers = new List<GameObject>();

    public void LoadScript()
    {
        ClearScript();

    }
    private void ClearScript()
    {
        RemoveAllRendered();
        allNodes?.Clear();
        foreach (GameObject objectNode in GameObject.FindGameObjectsWithTag("PathNode"))
        {
            allNodes.Add(objectNode.GetComponent<Node>());
        }
        if (startingNode == null)
        {
            startingNode = allNodes[0];
        }
        startMissionNodes?.Clear();
        endMissionNodes?.Clear();
        startMissionNodes = new List<Node>();
        endMissionNodes = new List<Node>();
        selectedNodes?.Clear();
        selectedNodes = new List<Node>
        {
            startingNode
        };
        SelectingMode = false;
    }
    void Update()
    {
        if (SelectingMode && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layer))
            {
                if (hit.transform.GetComponent<Node>() != null)
                {
                    Node node = hit.transform.GetComponent<Node>();
                    if (selectedNodes[selectedNodes.Count - 1] == node)
                    {
                        if (selectedNodes.Count > 1)
                        {
                            selectedNodes.RemoveAt(selectedNodes.Count - 1);
                        }
                    }
                    else
                        selectedNodes.Add(node);

                    DrawCheckpoints(selectedNodes[selectedNodes.Count - 1]);
                    RenderCRoof(selectedNodes.Last());
                }
            }
            
        }
    }
    #region PRIVATE
    private void DrawCheckpoints(Node currentNode, Node parent = null)
    {
        // daca nu este nod de skip, Sterge toate liniile randate (caile intre noduri),
        // sterge toate checkpointurile randate,
        // si adauga un checkpoint la nodul curent "parinte".
        if(!currentNode.skip)
        {
            RemoveAllRendered();
            AddAllMissionCheckpoints();
            // adauga checkpoint selectat
            if (startMissionNodes.Contains(currentNode) || endMissionNodes.Contains(currentNode))
            {
                RemoveCheckpoint(currentNode);
            }
            AddCheckpoint(currentNode, selectedNodeColor, true);
        }

        // Randeaza linii intre nodul curent si nodurile legate de acesta
        for(int i = 0; i < currentNode.paths.Count; i++)
        {
            // dace nu este nodul parinte
            if (currentNode.paths[i] != parent)
            {
                // creaza o noua linie de randat
                GameObject line = new GameObject();
                line.layer = pathLineLayer;
                line.transform.parent = this.transform;
                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.numCapVertices = 3;
                lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderer.receiveShadows = false;
                lineRenderer.widthCurve = curve;
                lineRenderer.SetPosition(0, currentNode.Position + offsetY);
                lineRenderer.SetPosition(1, currentNode.paths[i].Position + offsetY);
                lineRenderer.sharedMaterial = new Material(lineMaterial);
                
                // daca nodul legatura este skip, randeaza legaturi pana gaseste noduri care pot fi selectate
                // si sa le randeze checkpoints
                if (currentNode.paths[i].skip)
                {
                    DrawCheckpoints(currentNode.paths[i], currentNode);
                }
                // altfel randeaza un checkpoint verde
                else
                {
                    if (startMissionNodes.Contains(currentNode.paths[i]) || endMissionNodes.Contains(currentNode.paths[i]))
                    {
                        RemoveCheckpoint(currentNode.paths[i]);
                    }
                    AddCheckpoint(currentNode.paths[i], openNodeColor, true);
                }
                lineRenderer.sharedMaterial.color = pathColor;
                lineRenderers.Add(line);
            }
        }
    }
    private void RemoveAllRendered()
    {
        while (lineRenderers.Count > 0)
        {
            GameObject.DestroyImmediate(lineRenderers[0].GetComponent<LineRenderer>().sharedMaterial, true);
            GameObject.DestroyImmediate(lineRenderers[0].GetComponent<LineRenderer>(), true);
            GameObject.DestroyImmediate(lineRenderers[0], true);
            lineRenderers.RemoveAt(0);
        }
        lineRenderers.Clear();
        foreach (Node node in allNodes)
        {
            RemoveCheckpoint(node);
        }
    }
    private void RemoveCheckpoint(Node node)
    {
        // Sterge elementele componente (colider si renderer) si lasa doar nodul
        if (node.transform.GetComponent<SphereCollider>() != null)
        {
            GameObject.DestroyImmediate(node.transform.GetComponent<SphereCollider>(),true);
        }
        if (node.transform.childCount != 0)
        {
            Transform children = node.transform.GetChild(0);
            GameObject.DestroyImmediate(children.GetComponent<MeshRenderer>().sharedMaterial,true);
            GameObject.DestroyImmediate(children.gameObject, true);
        }
    }
    private void AddAllMissionCheckpoints()
    {
        foreach (Node node in startMissionNodes)
        {
            AddCheckpoint(node, startMissionNodeColor, false);
        }
        foreach (Node node in endMissionNodes)
        {
            AddCheckpoint(node, endMissionNodeColor, false);
        }
    }
    private Vector3 AddCheckpoint(Node node, Color color, bool collider)
    {
        // adauga un obiect copil la obiectul nod.
        // nodului i se adauga un collider
        if (collider)
        {
            if (node.transform.GetComponent<SphereCollider>() == null)
            {
                node.gameObject.AddComponent<SphereCollider>();
                node.GetComponent<SphereCollider>().radius = node.Radius;
            }
        }
        // randeaza un checkpoint. in copilul nodului
        if (node.transform.childCount == 0)
        {
            GameObject checkpoint = GameObject.Instantiate(checkpointObject);
            checkpoint.transform.SetParent(node.transform);
            checkpoint.transform.localPosition = new Vector3(0, 0.15f, 0f);
            checkpoint.transform.localScale= new Vector3(2,2,2);
            checkpoint.transform.transform.localRotation = new Quaternion();
            checkpoint.GetComponent<MeshRenderer>().sharedMaterial = new Material(checkpointMaterial);
            checkpoint.GetComponent<MeshRenderer>().sharedMaterial.color = color;
        }
        return node.transform.position;
    }
    private List<Node> getSkipPath(Node currentNode, Node parent, Node target){
        List<Node> final = new List<Node>
        {
            currentNode
        };
        // parcurge nodurile copii din nodul curent
        foreach (Node node in currentNode.paths)
        {
            // daca gaseste nodul dorit returneaza o lista cu nodurile skip parcurse
            if(node == target)
            {
                final.Add(node);
                return final;
            }
            // altfel parcurge nodurile copii ai copiluilui curent
            if ((node != parent || currentNode.paths.Count == 1) && node.skip)
            {
                // daca a fost gasit nodul tinta ataseaza nodurile parcurse si returneaza ruta finala intre noduri
                List<Node> temp = getSkipPath(node, currentNode, target);
                if (temp !=null && temp.Last() == target)
                {
                    final.AddRange(temp);
                    return final;
                }
            }
            
        }
        // daca nu a fost gasit o legatura returneaaza null
        return null;
    }
    #endregion
    #region PUBLIC
    public void RenderCRoof(Node node)
    {
        C_roof.SetActive(!nearC.Contains(node));
    }

    public List<Node> finalPath()
    {
        // determina ruta finala cu tot cu nodurile skip
        List<Node> final = new List<Node>
        {
            selectedNodes[0]
        };
        // parcurge nodurile selectate(doar noduri care nu sunt skip)
        for (int i = 1; i < selectedNodes.Count; i++)
        {
            // verifica daca nodul selectat are legatura cu nodul anterior ( legatura directa )
            if (final.Last().paths.Contains(selectedNodes[i]))
            {
                final.Add(selectedNodes[i]);
            }
            // altfel inseamna ca exista o legatura indirecta de noduri skip
            else
            {
                // determina ruta skip dintre cele 2 noduri selectate
                Node parent = final.Count == 1 ? final[0] : final[final.Count - 2];
                List<Node> temp = getSkipPath(final.Last(), parent, selectedNodes[i]);
                if (temp != null && temp.Last() == selectedNodes[i])
                    final.AddRange(temp);
            }
        }
        return final;
    }

    public void SetSelectionMode(bool value)
    {
        if (!value)
        {
            RemoveAllRendered();
            SelectingMode = false;
        }
        else
        {
            if (!SelectingMode)
            {
                DrawCheckpoints(selectedNodes[selectedNodes.Count - 1]);
                RenderCRoof(selectedNodes.Last());
            }
            SelectingMode = true;
            AddAllMissionCheckpoints();
        }
    }
    public void AddStartMissionNode(Node node)
    {
        startMissionNodes.Add(node);
    }
    public void AddEndMissionNode(Node node)
    {
        endMissionNodes.Add(node);
    }
    public bool CheckContainsRequiredNodes()
    {
        foreach (Node node in startMissionNodes)
        {
            if (!selectedNodes.Contains(node))
            {
                return false;
            }
        }
        foreach (Node node in endMissionNodes)
        {
            if (!selectedNodes.Contains(node))
            {
                return false;
            }
        }
        return true;
    }
    #endregion
}
