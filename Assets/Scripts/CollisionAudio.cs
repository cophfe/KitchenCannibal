using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudio : MonoBehaviour
{
    public SoundSources soundSources;
    public AudioSource source;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude > GameManager.Instance.minimumCollisionVelocity)
        {

        switch (soundSources)
        {
            case SoundSources.Bone:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(1, 4)));
                break;

            case SoundSources.Bread:
                break;

            case SoundSources.Cloche:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(1, 4)));
                break;

            case SoundSources.CupBoard:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(0, 4)));

                break;
            case SoundSources.CuttingBoard:
                break;

            case SoundSources.Fridge:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(1, 6)));
                break;

            case SoundSources.GameState:
                break;

            case SoundSources.Grill:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(2, 4)));
                break;

            case SoundSources.Grinder:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(0, 3)));
                break;

            case SoundSources.HealthInspector:
                break;

            case SoundSources.Knife:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(0, 2)));
                break;

            case SoundSources.Limb:
                    source.PlayOneShot(GameManager.Instance.audioManager.GetClip(soundSources, Random.Range(3, 6)));
                break;

            case SoundSources.Meat:
                break;
            case SoundSources.Music:
                break;
            case SoundSources.Order:
                break;
            case SoundSources.Plate:
                break;
            case SoundSources.Hand:
                break;
            case SoundSources.Salad:
                break;
            case SoundSources.Torso:
                break;
            case SoundSources.Vegetable:
                break;
        }
        }
    }
}
