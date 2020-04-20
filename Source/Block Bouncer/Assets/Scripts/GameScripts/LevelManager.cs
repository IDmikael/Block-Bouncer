using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Text txCurrentLevel = null;

    [SerializeField]
    private GameObject blocksContainer = null;

    private int currentLevel = 0;
    private int kickedBlocks = 0;

    // LevelUP event
    public static event Action OnLevelUp = delegate { };

    private void Start()
    {
        txCurrentLevel.text = currentLevel.ToString();

        // Ball collision event handler
        BallController.OnBallCollision += BallController_OnBallCollision;
    }

    private void BallController_OnBallCollision(BallController.ColliderType type)
    {
        if (type == BallController.ColliderType.Block)
        {
            kickedBlocks++;

            if (kickedBlocks >= blocksContainer.transform.childCount)
                LevelUp();
        }
    }

    private void LevelUp()
    {
        OnLevelUp();

        kickedBlocks = 0;
        currentLevel++;

        foreach(Transform block in blocksContainer.transform)
        {
            block.gameObject.SetActive(true);
        }

        Debug.Log("Level Up! Current level: " + currentLevel);

        txCurrentLevel.text = currentLevel.ToString();
    }
}
