using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // To check if ball connected to player's spring joint
    public bool isReleased = false;

    // Accelerometer values
    [SerializeField]
    private float speedAcc = 1.1f;
    public bool isAccMode = false;

    private Rigidbody rigidBody;
    private Vector3 defaultPosition;

    // Ball collision event
    public static event Action<ColliderType> OnBallCollision = delegate { };

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        defaultPosition = transform.position;
    }

    private void Update()
    {
        // Applying accelerometer control if game is not on pause
        if (isAccMode && Time.timeScale == 1)
        {
            rigidBody.velocity = new Vector3(Input.acceleration.x * speedAcc, 0, -Input.acceleration.z * speedAcc);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gObj = collision.collider.gameObject;

        switch (gObj.tag)
        {
            case "Enemy":
                OnBallCollision(ColliderType.Enemy);
                break;
            case "Block":
                gObj.SetActive(false);
                OnBallCollision(ColliderType.Block);
                break;
        }
    }

    public void RespawnBall()
    {
        isReleased = isAccMode ? true : false;
        transform.position = defaultPosition;
        rigidBody.constraints = isAccMode ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
    }

    public enum ColliderType
    {
        Block,
        Enemy
    }
}
