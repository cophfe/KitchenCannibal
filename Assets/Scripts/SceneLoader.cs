using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[SerializeField]
	OutOfBounds fader;
	[SerializeField]
	string scene;

	public void ExitGame()
	{
		fader.OnExitFade();
		fader.OnEndOverride.AddListener(FinishExit);
	}

	public void EnterGame()
	{
		fader.OnExitFade();
		fader.OnEndOverride.AddListener(FinishEnter);

	}

	void FinishExit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}

	void FinishEnter()
	{
		SceneManager.LoadScene(scene);
	}

}
