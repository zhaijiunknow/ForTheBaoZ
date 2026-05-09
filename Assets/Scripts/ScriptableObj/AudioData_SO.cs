using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable/AudioData")]

public class AudioData_SO : ScriptableObject
{
    [Header("Music")]
    public AudioClip[] BGMClip;

    [Header("Sound Effect")]
    public AudioClip[] SFXClip;

}