using UnityEngine;
using UnityEngine.UI;
public class MissionDisplay : MonoBehaviour
{
    public Text descriere;
    public Text start;
    public Text end;
    public void SetText(Mission mission)
    {
        descriere.text = mission.Description;
        start.text = mission.startLocation;
        end.text = mission.endLocation;
    }
}
