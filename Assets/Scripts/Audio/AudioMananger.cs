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

    private void Awake()
    {
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
}
