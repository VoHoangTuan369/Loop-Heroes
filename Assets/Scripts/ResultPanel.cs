using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] Image winImage, loseImage;
    [SerializeField] Button nextBtn, backBtn;

    void Start()
    {
        nextBtn.onClick.AddListener(OnNextClicked);
        backBtn.onClick.AddListener(OnBackClicked);
    }

    void OnNextClicked()
    {
        LevelLoader.SelectedLevel++;
        SceneManager.LoadScene("GameScene");
    }

    void OnBackClicked()
    {
        SceneManager.LoadScene("LevelScene");
    }

    void OnDisable()
    {
        winImage.rectTransform.DOKill();
        loseImage.rectTransform.DOKill();
    }
    public void ShowResult(bool isWin)
    {
        if (isWin)
        {
            winImage.gameObject.SetActive(true);
            loseImage.gameObject.SetActive(false);
            AnimateText(winImage);
            SoundMN.Instance.PlayWinSound();
        }
        else
        {
            winImage.gameObject.SetActive(false);
            loseImage.gameObject.SetActive(true);
            AnimateText(loseImage);
            SoundMN.Instance.PlayLoseSound();
        }
        nextBtn.gameObject.SetActive(isWin);
    }
    void AnimateText(Image resultImage)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(resultImage.rectTransform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack));
        seq.Append(resultImage.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.InBack));
    }
}