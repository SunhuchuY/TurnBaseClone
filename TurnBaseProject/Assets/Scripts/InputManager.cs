#define USE_NEW_INPUT_SYSTEM
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 CameraMoveVector
    {
        get
        {
            Vector2 inputMoveDir = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.y = +1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.y = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x = +1f;
            }

            return inputMoveDir;
        }
    }

    public float CameraRotationAmount
    {
        get
        {
            float rotationAmount = 0;

            if (Input.GetKey(KeyCode.Q))
            {
                rotationAmount = +1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotationAmount = -1f;
            }

            return rotationAmount;
        }
    }

    public float MouseScrollDeltaY
    {
        get
        {
            return Input.mouseScrollDelta.y;
        }
    }

    public Vector3 MousePosition
    {
        get
        {
            return Input.mousePosition;
        }
    }

    public bool IsMouseLeftButtonDown
    {
        get
        {
            return Input.GetMouseButtonDown(0);
        }
    }
}
