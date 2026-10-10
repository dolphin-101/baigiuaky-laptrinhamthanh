using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour
{
    [Header("Tên Scene muốn chuyển tới")]
    public string sceneToLoad = "HomeScene";

    [Header("UI Reference (Tùy chọn)")]
    public TextMeshProUGUI loadingText;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingText != null)
            {
                loadingText.text = "Loading... " + (progress * 100f).ToString("F0") + "%";
            }

            yield return null; 
        }
    }
}
