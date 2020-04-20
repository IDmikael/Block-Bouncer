using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Game objects
    private Rigidbody rbBall;
    private BallController ballController;

    private Rigidbody rbPlayer;
    private SpringJoint springJoint;

    // Ball throw values
    [SerializeField]
    private float releaseTime = .15f;
    [SerializeField]
    private float maxDragDistance = 2f;

    // Values for mouse position calculating (for dragging player object by mouse or touch)
    private Vector3 mOffset;
    private float mZCoord;

    // Check if player object pressed
    private bool isPressed = false;

    [SerializeField]
    private ArrowDrawer arrow = null;

    private Vector3 defaultPosition;

    private bool isGamePaused = false;

    // Current control type
    public string controlType = PrefsNames.modeNames[0];

    private void Awake()
    {
        rbPlayer = GetComponent<Rigidbody>();
        springJoint = GetComponent<SpringJoint>();

        rbBall = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
        ballController = rbBall.gameObject.GetComponent<BallController>();

        defaultPosition = transform.position;

        // Subscribing on needed events
        BallController.OnBallCollision += BallController_OnBallCollision;
        MainMenuUI.OnGamePause += MainMenuUI_OnGamePause;
    }

    private void Update()
    {
        if (isPressed && controlType == PrefsNames.modeNames[0])
        {
            Vector3 mousePos = GetMouseAsWorldPoint() + mOffset;

            // Check if player at it max distance from the ball
            if (Vector3.Distance(mousePos, rbBall.position) > maxDragDistance)
            {
                rbPlayer.position = rbBall.position + (mousePos - rbBall.position).normalized * maxDragDistance;
            }
            else
            {
                rbPlayer.position = mousePos;
            }

            // Drawing arrow
            arrow.ChangeArrowLegth(Vector3.Distance(rbBall.position, rbPlayer.position));
            arrow.ChangeLineDestPosition(rbPlayer.position);

            // Changing player's rotation
            Vector3 targetPos = new Vector3(rbBall.position.x, transform.position.y, rbBall.position.z);
            transform.LookAt(targetPos);
        }
    }

    // INPUT METHODS
    private void OnMouseDown()
    {
        if (ballController.isReleased || isGamePaused)
            return;

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        // Activating arrow and line renderer
        arrow.gameObject.SetActive(true);
        arrow.SetupLineRenderer(rbBall.position, rbPlayer.position);

        // Connecting ball to spring joint
        springJoint.connectedBody = rbBall;

        isPressed = true;

        rbPlayer.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rbPlayer.isKinematic = true;
        rbBall.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnMouseUp()
    {
        if (ballController.isReleased || isGamePaused)
            return;

        isPressed = false;
        rbPlayer.isKinematic = true;
        rbPlayer.constraints = RigidbodyConstraints.FreezeAll;
        rbBall.constraints = RigidbodyConstraints.FreezePositionY;

        StartCoroutine(Release());
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // Coroutine to disconnect ball from player after mouseUp in some time
    private IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseTime);

        arrow.gameObject.SetActive(false);
        ballController.isReleased = true;
        springJoint.connectedBody = null;
    }

    // For control mode switch. 0 - finger, 1 - accelerometer
    public void ChangeControlType(string controlTypeValue)
    {
        controlType = controlTypeValue;

        if (controlType == PrefsNames.modeNames[0])
        {
            rbPlayer.gameObject.SetActive(true);
            ballController.isReleased = false;
            ballController.isAccMode = false;
            rbBall.constraints = RigidbodyConstraints.FreezeAll;
        }
        else if (controlType == PrefsNames.modeNames[1])
        {
            rbPlayer.gameObject.SetActive(false);
            ballController.isReleased = true;
            ballController.isAccMode = true;
            rbBall.constraints = RigidbodyConstraints.None;
        }
    }


    // EVENTS
    private void BallController_OnBallCollision(BallController.ColliderType type)
    {
        if (type == BallController.ColliderType.Enemy || type == BallController.ColliderType.Block)
        {
            ballController.RespawnBall();

            // Resetting player at it's default position with zero rotation
            transform.position = defaultPosition;
            transform.rotation = Quaternion.identity;
        }
    }

    private void MainMenuUI_OnGamePause(bool isPaused)
    {
        isGamePaused = isPaused;
    }
}
