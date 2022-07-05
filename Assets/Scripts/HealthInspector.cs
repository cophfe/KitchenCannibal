using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthInspector : MonoBehaviour
{
	public float startOffset;
	public Animator healthInspectorTextAnimator;
	public Animator healthInspectorVisual;
	public TextMeshPro healthInspectorText;
    public int timeBetweenInspections = 180;
    public int scanTime = 5;
    public int warningDuration = 10;
	public CheckFridge fridgeChecker;
	public DoorClose[] doors = null;
	public float counter = 0.0f;
	bool warning = false;
    private bool hasStarted = false;
	public float timeTakenFromBones = 20;

    IEnumerator HealthInspectorCalled()
    {
        hasStarted = true;
		warning = false;

		healthInspectorTextAnimator.SetTrigger("Scanning");
		healthInspectorVisual.SetTrigger("Scan");
		healthInspectorText.text = "SCANNING...";
		GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, 7);
		Debug.Log("Health Inspector: Arrive");
		yield return new WaitForSeconds((float)scanTime);
		healthInspectorTextAnimator.SetTrigger("Reset");


		OnArrive();
        counter = timeBetweenInspections;
        hasStarted = false;
    }

    private void StartWarning()
    {
        Debug.Log("Health Inspector: Warning");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, 0);
		healthInspectorTextAnimator.SetTrigger("Warning");
		warning = true;
	}

    private void OnArrive()
    {
		
		// Check if all human meat is in the fridge
		// for each human meat 
		if (fridgeChecker.AreAllInBox() && DoorsClosed())
			OnAvoidCaught();
        else
			OnCaught();
    }

	bool DoorsClosed()
	{
		bool open = false;
		foreach( var door in doors)
		{
			open |= !door.Closed;
		}
		return !open;
	}

    private void OnCaught()
    {
        Debug.Log("Health Inspector: Player Caught");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, Random.Range(1, 4));
        GameManager.Instance.scoreKeeper.ChangeScore(ScoreChange.HealthInspectorFail);
    }

    private void OnAvoidCaught()
    {
        Debug.Log("Health Inspector: Player Safe");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, Random.Range(4, 7));
        GameManager.Instance.scoreKeeper.ChangeScore(ScoreChange.HealthInspectorPass);
    }

    public void StartInspection()
    {
        StartCoroutine(HealthInspectorCalled());
    }

	public void OnBonesMeal()
	{
		GameManager.Instance.audioManager.PlayOneShot(SoundSources.BonesMeal, 0);
		healthInspectorTextAnimator.SetTrigger("Damage");
	}
	private void Awake()
    {
		counter = startOffset;
    }

    private void Update()
    {
		
        if (!hasStarted)
        {
			counter -= Time.deltaTime;
			if (counter <= 0)
				StartInspection();
			else if (!warning && counter < warningDuration)
				StartWarning();

			healthInspectorText.text = System.String.Format("The Health Inspector is coming in: {0:0.0} seconds", counter);
        }
    }
}
