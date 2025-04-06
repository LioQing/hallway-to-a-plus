using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSubmenu : MonoBehaviour
{
    public GameObject environmentButton;
    public GameObject anomalyButton;

    public GameObject environmentList;
    public GameObject anomalyList;

    private int _layer;
    private bool? _isEnvironmentListActive;
    
    private TextMeshProUGUI _environmentText;
    private TextMeshProUGUI _anomalyText;
    private GameObject _environmentListContent;
    private GameObject _anomalyListContent;
    private GameObject _environmentListItem;
    private GameObject _anomalyListItem;
    
    private void Start()
    {
        environmentButton.SetActive(false);
        anomalyButton.SetActive(false);
        environmentList.SetActive(false);
        anomalyList.SetActive(false);
        
        _layer = 0;
        _isEnvironmentListActive = null;
        
        _environmentText = environmentButton.GetComponentInChildren<TextMeshProUGUI>();
        _anomalyText = anomalyButton.GetComponentInChildren<TextMeshProUGUI>();
        
        _environmentText.text = AnomalySettings.Environment.ToString();
        _anomalyText.text = AnomalySettings.Anomaly != null ? AnomalySettings.Anomaly!.ToString() : "Random";

        _environmentListContent = environmentList.GetComponent<ScrollRect>().content.gameObject;
        _anomalyListContent = anomalyList.GetComponent<ScrollRect>().content.gameObject;
        
        _environmentListItem = _environmentListContent.transform.GetChild(0).gameObject;
        _environmentListItem.SetActive(false);
        
        for (var i = 0; i < Enum.GetValues(typeof(AnomalyManager.Environment)).Length; i++)
        {
            CreateEnvironmentListItem(i);
        }

        var height = Enum.GetValues(typeof(AnomalyManager.Environment)).Length * 32.0f - 2.0f;
        _environmentListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        for (var i = 0; i < _environmentListContent.transform.childCount; i++)
        {
            var item = _environmentListContent.transform.GetChild(i);
            item.localPosition += new Vector3(0, -height / 2.0f + 15.0f, 0);
        }
        
        _anomalyListItem = _anomalyListContent.transform.GetChild(0).gameObject;
        _anomalyListItem.SetActive(false);
        
        for (var i = 0; i < Enum.GetValues(typeof(AnomalyManager.AnomalyType)).Length; i++)
        {
            CreateAnomalyListItem(i);
        }
        
        height = Enum.GetValues(typeof(AnomalyManager.AnomalyType)).Length * 32.0f - 2.0f;
        _anomalyListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        for (var i = 0; i < _anomalyListContent.transform.childCount; i++)
        {
            var item = _anomalyListContent.transform.GetChild(i);
            item.localPosition += new Vector3(0, -height / 2.0f + 15.0f, 0);
        }
    }

    public void OnCustomMenu()
    {
        _layer = _layer == 0 ? 1 : 0;

        if (_layer == 1)
        {
            environmentButton.SetActive(true);
            anomalyButton.SetActive(true);
        }
        else
        {
            environmentButton.SetActive(false);
            anomalyButton.SetActive(false);
            environmentList.SetActive(false);
            anomalyList.SetActive(false);
        }
    }

    public void OnEnvironmentList()
    {
        _layer = _layer == 1 ? 2 : 1;

        if (_layer == 2 || _isEnvironmentListActive == false)
        {
            _isEnvironmentListActive = true;
            environmentList.SetActive(true);
            anomalyList.SetActive(false);
        }
        else
        {
            _isEnvironmentListActive = null;
            environmentList.SetActive(false);
        }
    }
    
    public void OnAnomalyList()
    {
        _layer = _layer == 1 ? 2 : 1;
        
        if (_layer == 2 || _isEnvironmentListActive == true)
        {
            _isEnvironmentListActive = false;
            environmentList.SetActive(false);
            anomalyList.SetActive(true);
        }
        else
        {
            _isEnvironmentListActive = null;
            anomalyList.SetActive(false);
        }
    }

    private void CreateEnvironmentListItem(int index)
    {
        var environment = (AnomalyManager.Environment) index;
        var offset = index * 32.0;
        var item = Instantiate(_environmentListItem, _environmentListContent.transform);
        item.SetActive(true);
        item.transform.localPosition += new Vector3(0, (float) offset, 0);
        item.GetComponentInChildren<TextMeshProUGUI>().text = environment.ToString();
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            AnomalySettings.Environment = environment;
            _environmentText.text = environment.ToString();
            _isEnvironmentListActive = null;
            _layer = 1;
            environmentList.SetActive(false);
        });
    }
    
    private void CreateAnomalyListItem(int index)
    {
        var anomaly = (AnomalyManager.AnomalyType) index;
        var offset = index * 32.0;
        var item = Instantiate(_anomalyListItem, _anomalyListContent.transform);
        item.SetActive(true);
        item.transform.localPosition += new Vector3(0, (float) offset, 0);
        item.GetComponentInChildren<TextMeshProUGUI>().text = anomaly.ToString();
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            AnomalySettings.Anomaly = anomaly;
            _anomalyText.text = anomaly.ToString();
            _isEnvironmentListActive = null;
            _layer = 1;
            anomalyList.SetActive(false);
        });
    }
}
