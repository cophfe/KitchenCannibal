using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundSources
{
    Bone,
    HealthInspector,
    CupBoard,
    CuttingBoard,
    Torso,
    Limb,
    Fridge,
    Order,
    GameState,
    Meat,
    Vegetable
}

public class AudioMananger : MonoBehaviour
{
    public AudioSource mainAudioSource = null;
    [HideInInspector] public List<AudioClip>[] clipList = new List<AudioClip>[11];
    [SerializeField] private List<AudioClip> boneClips;
    [SerializeField] private List<AudioClip> healthInspectorClips;
    [SerializeField] private List<AudioClip> cupboardClips;
    [SerializeField] private List<AudioClip> cuttingboardClips;
    [SerializeField] private List<AudioClip> torsoClips;
    [SerializeField] private List<AudioClip> limbClips;
    [SerializeField] private List<AudioClip> fridgeClips;
    [SerializeField] private List<AudioClip> orderClips;
    [SerializeField] private List<AudioClip> gameStateClips;
    [SerializeField] private List<AudioClip> meatClips;
    [SerializeField] private List<AudioClip> vegetableClips;

    /// <summary>
    /// fills the array of lists with thir respective lists.
    /// </summary>
    private void Awake()
    {
        mainAudioSource = GetComponent<AudioSource>();
        if (mainAudioSource == null)
            Debug.LogError("Missing component 'AudioSource'");
        
        clipList[0] = boneClips;
        clipList[1] = healthInspectorClips;
        clipList[2] = cupboardClips;
        clipList[3] = cuttingboardClips;
        clipList[4] = torsoClips;
        clipList[5] = limbClips;
        clipList[6] = fridgeClips;
        clipList[7] = orderClips;
        clipList[8] = gameStateClips;
        clipList[9] = meatClips;
        clipList[10] = vegetableClips;
    }


    /// <summary>
    /// Called whenever a sound is needed to be made in the game.
    /// </summary>
    /// <param name="source">The thing that woill be making the sound</param>
    /// <param name="index">The index of the specific sound</param>
    /// <returns></returns>
    public AudioClip GetClip(SoundSources source, int index)
    {
        return clipList[(int)source][index];
    }


    public void PlayOneShot(SoundSources source, int index)
    {
        Debug.Log("Playing sound type: '"+ source.ToString() + "' with index: " + index);
        mainAudioSource.PlayOneShot(GetClip(source, index));
    }
}
