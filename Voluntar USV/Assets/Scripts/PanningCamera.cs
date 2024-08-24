using UnityEngine;

public class PanningCamera : MonoBehaviour
{
    Vector3 lastPosition;

    [Header("Settings")]
    public float zoomSensitivity = 50f;
    public float panningSensitivity = 35f;
    public float speedIncreasing = 2.5f;
    private float speed = 1f;
    public Vector2 clampAngle = new Vector2(45, 65);
    public Vector2 clampX = new Vector2(-70, 130);
    public Vector2 clampY = new Vector2(30,65);
    public Vector2 clampZ = new Vector2(-200, 100);

    [Header("Keys")]
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Forward = KeyCode.W;
    public KeyCode Backward = KeyCode.S;
    public KeyCode Up = KeyCode.LeftShift;
    public KeyCode Down = KeyCode.LeftControl;
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
    void Update()
    {
        Vector3 direction = OnPanningMove() * Time.deltaTime;

        if(direction != Vector3.zero)
        {
            speed = Mathf.MoveTowards(speed, speedIncreasing, Time.deltaTime / 2);
            direction.x *= speed;
            direction.z *= speed;
            transform.Translate(direction, Space.World);
        }
        else
        {
            speed = 1f;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, clampX.x, clampX.y),
            Mathf.Clamp(transform.position.y, clampY.x, clampY.y),
            Mathf.Clamp(transform.position.z, clampZ.x, clampZ.y));
        transform.eulerAngles = new Vector3(clampAngle.x + (clampAngle.y - clampAngle.x) * (transform.position.y - clampY.x) / (clampY.y - clampY.x), 0, 0);
    }
    private Vector3 OnPanningMove()
    {
        Vector3 target = Vector3.zero;
        if (Input.GetKey(Forward))
        {
            target += new Vector3(0, 0, panningSensitivity);
        }
        if (Input.GetKey(Backward))
        {
            target += new Vector3(0, 0, -panningSensitivity);
        }
        if (Input.GetKey(Left))
        {
            target += new Vector3(-panningSensitivity, 0, 0);
        }
        if (Input.GetKey(Right))
        {
            target += new Vector3(panningSensitivity, 0, 0);
        }
        if (Input.GetKey(Up))
        {
            target += new Vector3(0, zoomSensitivity, 0);
        }
        if (Input.GetKey(Down))
        {
            target += new Vector3(0, -zoomSensitivity, 0);
        }
        return target;
    }
}
