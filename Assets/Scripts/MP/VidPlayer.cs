using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VidPlayer : MonoBehaviour
{
    //Thanks to Max O'Didily's Tutorial: https://youtu.be/9UE3hLSHMTE?si=BPwxLOXiFM4wj_uU

    [SerializeField] string videoFileName;

    private void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }
}
