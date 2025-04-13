using System;
using System.Collections;
using System.Collections.Generic;
using GaussianSplatting.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutsceneManager : MonoBehaviour
{
    public GameObject loadingUi;
    public TextMeshProUGUI textUi;
    public float loadingTime = 10.0f;

    public Vector3 position1Start;
    public Vector3 position1End;
    public Quaternion rotation1Start;
    public Quaternion rotation1End;
    public Vector3 position2Start;
    public Vector3 position2End;
    public Quaternion rotation2Start;
    public Quaternion rotation2End;
    public Vector3 position3Start;
    public Vector3 position3End;
    public Quaternion rotation3Start;
    public Quaternion rotation3End;
    public Vector3 position4Start;
    public Vector3 position4End;
    public Quaternion rotation4Start;
    public Quaternion rotation4End;
    
    public float transitionTime = 8.0f;
    
    public GameObject gaussianSplatsScene;
    public GameObject torch;
    public GameObject twChim;

    private IEnumerable<GaussianSplatRenderer> _splatRenderers;
    private Camera _camera;
    private GaussianCutout _torchCutout;

    private void Start()
    {
        _splatRenderers = gaussianSplatsScene.GetComponentsInChildren<GaussianSplatRenderer>(true);
        _camera = GetComponent<Camera>();
        _torchCutout = torch.GetComponentInChildren<GaussianCutout>();
        
        loadingUi.SetActive(true);
        textUi.gameObject.SetActive(false);
        torch.SetActive(false);
        twChim.SetActive(false);
        StartCoroutine(PlayEndCutscene());
    }

    private IEnumerator PlayEndCutscene()
    {
        yield return new WaitForSeconds(loadingTime);
        loadingUi.SetActive(false);
        textUi.gameObject.SetActive(true);
        
        transform.position = position1Start;
        transform.rotation = rotation1Start;
        
        yield return StartCoroutine(Transition(0, position1Start, position1End, rotation1Start, rotation1End));

        textUi.text = "COMP3329 Group 25\n\nLio Qing - Game Design, Programming, 3D Modeling, Soundtrack\nSteven Law - Game Design, 3D Modeling";
        
        yield return StartCoroutine(Transition(1, position2Start, position2End, rotation2Start, rotation2End));

        textUi.text = "Hallway to A+\n\nMade with Unity, Blender, Gaussian Splatting";
        
        yield return StartCoroutine(Transition(2, position3Start, position3End, rotation3Start, rotation3End));

        textUi.text = "Thank you for playing!";
        
        yield return StartCoroutine(Transition(3, position4Start, position4End, rotation4Start, rotation4End));

        SceneManager.LoadScene("MainMenuScene");
    }
    
    private IEnumerator Transition(int index, Vector3 startPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation)
    {
        var elapsedTime = 0f;

        if (index == 0)
        {
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.black;
        }
        else if (index == 1)
        {
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.black;
            
            torch.SetActive(true);
                
            foreach (var splatRenderer in _splatRenderers)
            {
                splatRenderer.m_Cutouts = new[] { _torchCutout };
            }
        }
        else if (index == 3)
        {
            twChim.SetActive(true);
        }

        while (elapsedTime < transitionTime)
        {
            var progress = elapsedTime / transitionTime;

            if (index == 0)
            {
                foreach (var splatRenderer in _splatRenderers)
                {
                    splatRenderer.m_SplatScale = Mathf.Lerp(0.0f, 1.0f, Math.Min(1f, progress * 1.1f));
                }
            }
            else if (index == 3)
            {
                if (progress >= 0.5f)
                {
                    var adjustedProgress = (progress - 0.5f) / 0.5f;
                    var start = Quaternion.Euler(0, 0, 0);
                    var end = Quaternion.Euler(0, 180, 0);
                    twChim.transform.rotation = Quaternion.Slerp(start, end, adjustedProgress);
                }
            }
            
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionTime);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        transform.rotation = endRotation;

        if (index == 0)
        {
            foreach (var splatRenderer in _splatRenderers)
            {
                splatRenderer.m_SplatScale = 1f;
            }
            _camera.clearFlags = CameraClearFlags.Skybox;
        }
        else if (index == 1)
        {
            _camera.clearFlags = CameraClearFlags.Skybox;
            
            torch.SetActive(false);
            
            foreach (var splatRenderer in _splatRenderers)
            {
                splatRenderer.m_Cutouts = Array.Empty<GaussianCutout>();
            }
        }
    }
}
