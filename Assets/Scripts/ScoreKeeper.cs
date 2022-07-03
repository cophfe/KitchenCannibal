using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreChange
{
    OrderComplete,
    OrderFailed,
    HealthInspectorPass,
    HealthInspectorFail,
}

public class ScoreKeeper : MonoBehaviour
{
    public float playerScore = 0.0f;

    public void ResetScore()
    {
        playerScore = 0.0f;
    }

    public void ChangeScore(ScoreChange changeType)
    {
        float value = 0.0f;

        switch (changeType)
        {
            case ScoreChange.OrderComplete:
                value = 1.0f;
                break;

            case ScoreChange.OrderFailed:
                value = -1.0f;
                break;

            case ScoreChange.HealthInspectorPass:
                value = 1.5f;
                break;

            case ScoreChange.HealthInspectorFail:
                value = -1.5f;
                break;
        }

        playerScore += value;
    }
}
