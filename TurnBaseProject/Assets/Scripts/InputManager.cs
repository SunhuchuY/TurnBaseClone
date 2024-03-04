#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 CameraMoveVector
    {
        get
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
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
#endif
        }
    }

    public float CameraRotationAmount
    {
        get
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraRotate.ReadValue<float>();   
#else
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
#endif
        }
    }

    public float MouseScrollDeltaY
    {
        get
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
            return Input.mouseScrollDelta.y;
#endif

        }
    }

    public Vector3 MousePosition
    {
        get
        {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }
    }

    public bool IsMouseLeftButtonDownThisFrame
    {
        get
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.Click.WasPressedThisFrame();
#else
            return Input.GetMouseButtonDown(0);
#endif
        }
    }
}
