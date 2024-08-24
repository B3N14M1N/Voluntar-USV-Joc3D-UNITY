using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string startName;
    public string endName;
    public string startLocation;
    public string endLocation;
    [Multiline]
    public string Description;
}