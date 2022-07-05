using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreChange
{
    OrderComplete,
    OrderFailed,
	BonesOrder,
    HealthInspectorPass,
    HealthInspectorFail,
}

public class ScoreKeeper : MonoBehaviour
{
    public float playerScore = 0.0f;
    private int ordersComplete = 0;
    private int ordersFailed = 0;
    private int healthInspectorPass = 0;
    private int healthInspectorFail = 0;
	private int taintedMeals = 0;
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
                ordersComplete++;
                value = 1.0f;
                break;
			case ScoreChange.BonesOrder:
                ordersComplete++;
				taintedMeals++;
				value = 1.0f;
				break;
			case ScoreChange.OrderFailed:
                ordersFailed++;
                value = -1.0f;
                break;

            case ScoreChange.HealthInspectorPass:
                healthInspectorPass++;
                value = 1.5f;
                break;

            case ScoreChange.HealthInspectorFail:
                healthInspectorFail++;
                value = -1.5f;
                break;
        }

        playerScore += value;
    }
}
