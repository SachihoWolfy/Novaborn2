using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Courtesy of https://forum.unity.com/threads/how-to-convert-string-to-audioclip.1417317/
// My SoundController Class is driving me insane.
public class AudioClipGetter : MonoBehaviour
{
    //add your clips to this list in inspector
    [SerializeField] List<AudioClip> clips;

    //this is the the object that will hold a mapping of strings to AudioClips
    Dictionary<string, AudioClip> dict;

    public static AudioClipGetter instance;

    void Awake()
    {
        instance = this;
        //initialize an empty dictionary, so we can add clip names and their associated AudioClips to it
        dict = new Dictionary<string, AudioClip>();

        //add all clips to the dictionary, such that they string that will be used to be associated
        //with them will be the name of the clip file (i.e. 'cloud.mp3')
        for (int i = 0; i < clips.Count; i++)
        {
            AudioClip clip = clips[i];
            dict[clip.name] = clip;
        }
    }

    //this is your association function. you provide it the name of the clip (i.e. 'cloud.mp3')
    //as the parameter to the function. It returns the clip with that name, if you added that clips
    //to the serialized list field 'clips'.
    public AudioClip GetClip(string clipName)
    {
        //check that there is a clip associated with clip
        if (dict.ContainsKey(clipName)) return dict[clipName];
        else return null;
    }
}
