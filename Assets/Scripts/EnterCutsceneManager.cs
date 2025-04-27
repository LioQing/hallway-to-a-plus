using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EnterCutsceneManager : MonoBehaviour
{
    public GameObject choiceUi;
    public GameObject loadingUi;
    public GameObject changingGradeUi;
    public float loadingTime = 10.0f;
    public VideoPlayer videoPlayer;
    public VideoPlayer videoPlayerBack;
    public VideoClip toOffice;
    public VideoClip toLeft;
    public VideoClip toRight;
    public VideoClip enter;
    public Vector3 leftPosition;
    public Quaternion leftRotation;
    public Vector3 rightPosition;
    public Quaternion rightRotation;
    [CanBeNull] public PlayerController playerController;
    public bool skipCutscene;
    
    private bool? _isLeft;
    
    private void Start()
    {
        StartCoroutine(PlayEnterCutscene());
    }
    
    public void OnLeft()
    {
        _isLeft = true;
    }
    
    public void OnRight()
    {
        _isLeft = false;
    }

    private IEnumerator PlayEnterCutscene()
    {
        choiceUi.SetActive(false);
        loadingUi.SetActive(true);
        changingGradeUi.SetActive(false);

        if (playerController != null)
        {
            playerController.shouldUpdate = false;
        }

        yield return new WaitForSeconds(loadingTime);

        videoPlayer.enabled = true;
        PrepareVideo(videoPlayer, enter);
        
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
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        choiceUi.SetActive(true);
        
        while (!_isLeft.HasValue)
        {
            yield return null;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        choiceUi.SetActive(false);
        StartCoroutine(DelayedDisablePlayer(videoPlayer));
        StartCoroutine(PlayStartCutscene(_isLeft.Value));
    }

    private IEnumerator PlayStartCutscene(bool isLeft)
    {
        videoPlayerBack.enabled = true;
        PrepareVideo(videoPlayerBack, isLeft ? toLeft : toRight);
        
        if (playerController != null)
        {
            playerController.transform.position = isLeft ? leftPosition : rightPosition;
            playerController.transform.rotation = isLeft ? leftRotation : rightRotation;
        }
        
        yield return PlayVideo(videoPlayerBack);
        
        videoPlayerBack.enabled = false;
        
        if (playerController != null)
        {
            playerController.shouldUpdate = true;
        }
    }

    public void OnLeave()
    {
        StartCoroutine(PlayLeaveCutscenes());
    }

    private IEnumerator PlayLeaveCutscenes()
    {
        if (!skipCutscene)
        {
            videoPlayer.enabled = true;
            PrepareVideo(videoPlayer, toOffice);
        
            if (playerController != null)
            {
                playerController.shouldUpdate = false;
            }

            yield return PlayVideo(videoPlayer);
        }
        
        changingGradeUi.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        
        SceneManager.LoadScene("MainScene");
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
