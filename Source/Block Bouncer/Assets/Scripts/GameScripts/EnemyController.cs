using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private BallController ballController;
    private Transform ball;

    // Self rigidbody
    private Rigidbody rb;

    [SerializeField]
    private float speed = 2f;

    private Vector3 defaultPosition;

    // Values for speed up coroutine (scroll down for explanations 0._.)
    private float tempSpeed = 0f;
    private bool coroutineStarted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
        ballController = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();

        defaultPosition = transform.position;

        // Handling needed events
        BallController.OnBallCollision += BallController_OnBallCollision;
        LevelManager.OnLevelUp += LevelManager_OnLevelUp;
    }

    private void FixedUpdate()
    {
        // Moving enemy if ball is thrown
        if (ballController.isReleased && Time.timeScale == 1)
        {
            transform.LookAt(ball);
            rb.AddForce(transform.forward * speed);

            if (!coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(BallCatchTimer(20.0f));
            }
        }
    }

    // Coroutine to prevent ball from cycling around the field infinitely while enemy's speed is low
    private IEnumerator BallCatchTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        tempSpeed = speed;
        speed *= 5;
        StopAllCoroutines();
    }

    private void CheckBallCatch()
    {
        StopAllCoroutines();
        coroutineStarted = false;

        if (tempSpeed != 0f)
        {
            speed = tempSpeed;
            tempSpeed = 0;
        }
    }


    // EVENT HANDLERS
    private void BallController_OnBallCollision(BallController.ColliderType type)
    {
        if (type == BallController.ColliderType.Enemy || type == BallController.ColliderType.Block)
        {
            Debug.Log("Enemy");
            transform.position = defaultPosition;
            transform.rotation = Quaternion.identity;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            CheckBallCatch();
        }
    }

    private void LevelManager_OnLevelUp()
    {
        CheckBallCatch();
        speed *= 2;
    }
}
