using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthInspector : MonoBehaviour
{
    public int timeBetweenInspections = 180;
    public int warningDuration = 10;
	public CheckFridge fridgeChecker;
    public float counter = 0.0f;
    private bool hasStarted = false;
	public float timeTakenFromBones = 20;

    IEnumerator HealthInscpectorCalled()
    {
        hasStarted = true;
        StartWarning();
        {
            yield return new WaitForSeconds((float)warningDuration);
        }

        OnArrive();
        counter = timeBetweenInspections;
        hasStarted = false;
    }

    private void StartWarning()
    {
        Debug.Log("Health Inspector: Warning");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, 0);
    }

    private void OnArrive()
    {
        Debug.Log("Health Inspector: Arrive");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.HealthInspector, 7);
        // Check if all human meat is in the fridge
        // for each human meat 
        // if (meat x !infridge)
        // OnCaught();
        // else
        // OnAvoidCaught()
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
        StartCoroutine(HealthInscpectorCalled());
    }

	public void OnBonesMeal()
	{
		GameManager.Instance.audioManager.PlayOneShot(SoundSources.BonesMeal, 0);
	}
    private void Awake()
    {
        counter = timeBetweenInspections;
    }

    private void Update()
    {
        if (!hasStarted)
        {
        counter -= Time.deltaTime;
            if (counter <= 0)
                StartInspection();

        }
    }
}
