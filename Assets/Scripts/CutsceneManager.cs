using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public GameObject loadingUi;
    public GameObject leaveMessageUi;
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

    private TextMeshProUGUI _leaveMessage;
    
    private void Start()
    {
        _leaveMessage = leaveMessageUi.GetComponentInChildren<TextMeshProUGUI>();
        
        if (AnomalySettings.StoryLevel != null)
        {
            StartCoroutine(OnStart());
        }
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
        
            videoPlayerBack.enabled = false;
            PrepareVideo(videoPlayerBack, isCorrect ? leave : trapped);

            if (playerController != null)
            {
                playerController.shouldUpdate = false;
            }

            yield return PlayVideo(videoPlayer);

            StartCoroutine(DelayedDisablePlayer(videoPlayer)); // Disable player video after playing

            videoPlayerBack.enabled = true;
            yield return PlayVideo(videoPlayerBack);
        }

        _leaveMessage.text = isCorrect
            ? "You successfully escaped the hallway that night"
            : "You chose the wrong hallway, and the cycle continues...";
        
        leaveMessageUi.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        if (AnomalySettings.StoryLevel == null)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        
        if (isCorrect)
        {
            AnomalySettings.NextEnvironment(); // Environment has to be set before anomaly
            AnomalySettings.NextAnomaly();
            AnomalySettings.StoryLevel += 1;
            AnomalySettings.PrevTrapped = false;

            SceneManager.LoadScene(AnomalySettings.StoryLevel == 5 ? "EndScene" : "EnterScene");
        }
        else
        {
            AnomalySettings.PastAnomalies.Clear();
            AnomalySettings.Anomaly = null;
            AnomalySettings.Environment = AnomalyManager.Environment.Normal;
            AnomalySettings.StoryLevel = 0;
            AnomalySettings.PrevTrapped = true;
            
            AnomalySettings.NextAnomaly();
        
            SceneManager.LoadScene("MainScene");
        }
    }
    
    private void PrepareVideo(VideoPlayer player, VideoClip clip)
    {
        player.clip = clip;
        player.time = 0.0f;
        player.Stop();
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
