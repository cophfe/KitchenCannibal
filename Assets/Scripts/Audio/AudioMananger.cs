using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundSources
{
    Bone,
    Bread,
    Cloche,
    CupBoard,
    CuttingBoard,
    Fridge,
    GameState,
    Grill,
    Grinder,
    HealthInspector,
    Knife,
    Limb, 
    Meat,
    Music,
    Order,
    Plate,
    Hand,
    Salad,
    Torso,
    Vegetable
}

public class AudioMananger : MonoBehaviour
{
    public AudioSource mainAudioSource = null;
    private List<AudioClip>[] clipList = new List<AudioClip>[20];
    [SerializeField] private List<AudioClip> boneClips;
    [SerializeField] private List<AudioClip> breadClips;
    [SerializeField] private List<AudioClip> clocheClips;
    [SerializeField] private List<AudioClip> cupboardClips;
    [SerializeField] private List<AudioClip> cuttingboardClips;
    [SerializeField] private List<AudioClip> fridgeClips;
    [SerializeField] private List<AudioClip> gamestateClips;
    [SerializeField] private List<AudioClip> grillClips;
    [SerializeField] private List<AudioClip> grinderClips;
    [SerializeField] private List<AudioClip> healthInspectorClips;
    [SerializeField] private List<AudioClip> knifeClips;
    [SerializeField] private List<AudioClip> limbClips;
    [SerializeField] private List<AudioClip> meatClips;
    [SerializeField] private List<AudioClip> musicClips;
    [SerializeField] private List<AudioClip> orderClips;
    [SerializeField] private List<AudioClip> plateClips;
    [SerializeField] private List<AudioClip> handClips;
    [SerializeField] private List<AudioClip> saladClips;
    [SerializeField] private List<AudioClip> torsoClips;
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
        clipList[1] = breadClips;
        clipList[2] = clocheClips;
        clipList[3] = cupboardClips;
        clipList[4] = cuttingboardClips;
        clipList[5] = fridgeClips;
        clipList[6] = gamestateClips;
        clipList[7] = grillClips;
        clipList[8] = grinderClips;
        clipList[9] = healthInspectorClips;
        clipList[10] = knifeClips;
        clipList[11] = limbClips;
        clipList[12] = meatClips;
        clipList[13] = musicClips;
        clipList[14] = orderClips;
        clipList[15] = plateClips;
        clipList[16] = handClips;
        clipList[17] = saladClips;
        clipList[18] = torsoClips;
        clipList[19] = vegetableClips;
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
