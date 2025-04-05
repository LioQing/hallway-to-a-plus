using System.Collections;
using UnityEngine;

public class MannequinController : MonoBehaviour
{
    public GameObject mannequin;

    [Header("First Appearance")]
    public Vector3 firstInitialPosition;
    public Vector3 firstFinalPosition;
    public Quaternion firstRotation;
    public float firstWaitTime;
    public float firstSpeed;

    [Header("Second Appearance")]
    public Vector3 secondPosition;
    public Quaternion secondRotation;
    
    private int _appearanceCount;
    private Camera _mainCamera;

    private void Start()
    {
        mannequin.transform.position = firstInitialPosition;
        mannequin.transform.rotation = firstRotation;
        
        _mainCamera = Camera.main;
    }

    public void OnFirstAppearanceEnter()
    {
        if (_appearanceCount != 0)
        {
            return;
        }

        StartCoroutine(FirstAppearanceMove());
        _appearanceCount++;
    }
    
    public void OnFirstAppearanceExit()
    {
        if (_appearanceCount != 1)
        {
            return;
        }
        
        mannequin.SetActive(false);
        _appearanceCount++;
    }

    public void OnSecondAppearanceEnter()
    {
        if (_appearanceCount != 2)
        {
            return;
        }
        
        mannequin.transform.position = secondPosition;
        mannequin.transform.rotation = secondRotation;
        
        mannequin.SetActive(true);
        _appearanceCount++;
    }
    
    public void OnSecondAppearanceExit()
    {
        if (_appearanceCount != 3)
        {
            return;
        }
        
        mannequin.SetActive(false);
        _appearanceCount++;
    }

    private IEnumerator FirstAppearanceMove()
    {
        var facingDirection = _mainCamera.transform.forward;
        yield return new WaitUntil(() => Vector3.Dot(
            facingDirection,
            (firstInitialPosition - facingDirection).normalized
        ) > 0.2f);
        
        yield return new WaitForSeconds(firstWaitTime);

        if (_appearanceCount != 1)
        {
            yield break;
        }
        
        var journeyLength = Vector3.Distance(firstInitialPosition, firstFinalPosition);
        var distanceCovered = 0f;
    
        while (distanceCovered < journeyLength)
        {
            var step = firstSpeed * Time.deltaTime;
            mannequin.transform.position = Vector3.MoveTowards(mannequin.transform.position, firstFinalPosition, step);
        
            distanceCovered = Vector3.Distance(firstInitialPosition, mannequin.transform.position);
            yield return null;
        }
    
        mannequin.transform.position = firstFinalPosition;
        mannequin.SetActive(false);
    }
}
