using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinText : MonoBehaviour
{
	[field: SerializeField]
	public TextMeshPro OrdersComplete { get; private set; }
	[field: SerializeField]
	public TextMeshPro Score { get; private set; }
	[field: SerializeField]
	public TextMeshPro Inspector { get; private set; }
	[field: SerializeField]
	public TextMeshPro Tainted { get; private set; }

	public void SetText(ScoreKeeper keeper)
	{
		gameObject.SetActive(true);
		char score;
		if (keeper.playerScore > 7)
			score = 'S';
		else if (keeper.playerScore > 6.5f)
			score = 'A';
		else if (keeper.playerScore > 5.5f)
			score = 'B';
		else if (keeper.playerScore > 5.0f)
			score = 'C';
		else
			score = 'F';

		OrdersComplete.text = $"ORDERS  COMPLETE: {keeper.OrdersComplete}/{keeper.OrdersComplete + keeper.OrdersFailed}";
		Score.text = $"SCORE: {score}";
		Inspector.text = $"INSPECTORS  FOOLED: {keeper.HealthInspectorPass}/{keeper.HealthInspectorPass + keeper.HealthInspectorFail}";
		Tainted.text = $"ORDERS  TAINTED: {keeper.TaintedMeals}";
	}

	private void Awake()
	{
		gameObject.SetActive(false);
	}
}
