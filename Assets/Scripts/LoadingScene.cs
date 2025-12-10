using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;

    AsyncOperation operation;

    void Start()
    {
        StartCoroutine(LoadGameSceneAsync());
    }

    IEnumerator LoadGameSceneAsync()
    {
        operation = SceneManager.LoadSceneAsync("LevelScene");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, operation.progress, Time.deltaTime * 5f);
            yield return null;
        }

        loadingSlider.value = 1f;
        yield return new WaitForSeconds(2f); // tạo cảm giác chờ
        operation.allowSceneActivation = true;
    }
}
