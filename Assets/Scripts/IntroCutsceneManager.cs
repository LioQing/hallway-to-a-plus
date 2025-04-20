using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroCutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject titleUi;

    private void Start()
    {
        titleUi.SetActive(false);
        videoPlayer.loopPointReached += OnVideoComplete;
    }

    private void OnVideoComplete(VideoPlayer player)
    {
        titleUi.SetActive(true);
        player.gameObject.SetActive(false);

        SceneManager.LoadScene("EnterScene");
    }
}
