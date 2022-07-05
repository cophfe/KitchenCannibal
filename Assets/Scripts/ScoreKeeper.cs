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
	public bool WinState { get; private set; } = false;
    public int OrdersComplete {get; private set; }= 0;
    public int OrdersFailed {get; private set; }= 0;
    public int HealthInspectorPass {get; private set; }= 0;
    public int HealthInspectorFail {get; private set; }= 0;
	public int TaintedMeals { get; private set; } = 0;
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
                OrdersComplete++;
                value = 1.0f;
                break;
			case ScoreChange.BonesOrder:
                OrdersComplete++;
				TaintedMeals++;
				value = 1.0f;
				break;
			case ScoreChange.OrderFailed:
                OrdersFailed++;
                value = -1.0f;
                break;

            case ScoreChange.HealthInspectorPass:
                HealthInspectorPass++;
                value = 1.5f;
                break;

            case ScoreChange.HealthInspectorFail:
                HealthInspectorFail++;
                value = -1.5f;
                break;
        }

        playerScore += value;
    }
}
