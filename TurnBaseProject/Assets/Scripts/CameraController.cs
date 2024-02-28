using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{
    private const float MOVE_SPEED = 10f;
    private const float ROTATION_SPEED = 100f;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 10f;
    private const float ZOOM_AMOUNT = 1f;
    private const float ZOOM_SPEED = 5f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Awake()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.CameraMoveVector;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * MOVE_SPEED * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, InputManager.Instance.CameraRotationAmount, 0);

        transform.eulerAngles += rotationVector * ROTATION_SPEED * Time.deltaTime;
    }

    private void HandleZoom()
    {
        CinemachineTransposer cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        float mouseScrollDeltaY = InputManager.Instance.MouseScrollDeltaY;

        if (mouseScrollDeltaY > 0)
        {
            targetFollowOffset.y -= ZOOM_AMOUNT;
        }
        if (mouseScrollDeltaY < 0)
        {
            targetFollowOffset.y += ZOOM_AMOUNT;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * ZOOM_SPEED);    
    }
}
