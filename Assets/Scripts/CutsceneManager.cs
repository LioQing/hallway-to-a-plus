using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public GameObject loadingUi;
    public float loadingTime = 10.0f;
    public VideoPlayer videoPlayer;
    public VideoPlayer videoPlayerBack;
    public VideoClip fromOffice;
    public VideoClip fromLeft;
    public VideoClip fromRight;
    public VideoClip leave;
    public VideoClip trapped;
    [CanBeNull] public PlayerController playerController;
    public bool skipCutscene;
    
    private void Start()
    {
        StartCoroutine(OnStart());
    }

    private IEnumerator OnStart()
    {
        loadingUi.SetActive(true);

        videoPlayerBack.enabled = false;
        videoPlayer.enabled = true;
        PrepareVideo(videoPlayer, fromOffice);

        if (playerController != null)
        {
            playerController.shouldUpdate = false;
        }

        yield return new WaitForSeconds(loadingTime);
        
        loadingUi.SetActive(false);
        
        if (skipCutscene)
        {
            videoPlayer.enabled = false;
            if (playerController != null)
            {
                playerController.shouldUpdate = true;
            }
            yield break;
        }
        
        yield return PlayVideo(videoPlayer);
        
        videoPlayer.enabled = false;
        
        if (playerController != null)
        {
            playerController.shouldUpdate = true;
        }
    }

    public void OnLeave(bool isLeft, bool isCorrect)
    {
        StartCoroutine(PlayLeaveCutscenes(isLeft, isCorrect));
    }

    private IEnumerator PlayLeaveCutscenes(bool isLeft, bool isCorrect)
    {
        if (!skipCutscene)
        {
            videoPlayer.enabled = true;
            PrepareVideo(videoPlayer, isLeft ? fromLeft : fromRight);
        
            videoPlayerBack.enabled = true;
            PrepareVideo(videoPlayerBack, isCorrect ? leave : trapped);

            if (playerController != null)
            {
                playerController.shouldUpdate = false;
            }

            yield return PlayVideo(videoPlayer);

            StartCoroutine(DelayedDisablePlayer(videoPlayer)); // Disable player video after playing
        
            yield return PlayVideo(videoPlayerBack);
        }
        
        if (isCorrect)
        {
            AnomalySettings.NextEnvironment(); // Environment has to be set before anomaly
            AnomalySettings.NextAnomaly();
            
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            AnomalySettings.PastAnomalies.Clear();
            AnomalySettings.Anomaly = null;
            AnomalySettings.Environment = AnomalyManager.Environment.Normal;
            
            AnomalySettings.NextAnomaly();
        
            SceneManager.LoadScene("MainScene");
        }
    }
    
    private void PrepareVideo(VideoPlayer player, VideoClip clip)
    {
        player.clip = clip;
        player.time = 0.0f;
        player.Prepare();
    }
    
    private IEnumerator PlayVideo(VideoPlayer player)
    {
        player.Play();
        
        yield return new WaitForSeconds((float)player.clip.length);

        while (player.isPlaying)
        {
            yield return null;
        }
    }

    private IEnumerator DelayedDisablePlayer(VideoPlayer player)
    {
        yield return new WaitForSeconds(0.1f);
        
        player.enabled = false;
    }
}
