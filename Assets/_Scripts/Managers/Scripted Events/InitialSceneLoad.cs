using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class InitialSceneLoad : MonoBehaviour
{
    public string SceneName = "";
    public VideoPlayer IntroVideoPlayer;

    void Start()
    {
        IntroVideoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp) {
        SceneManager.LoadScene(SceneName);
    }
}
