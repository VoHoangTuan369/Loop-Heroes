using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // để load lại scene khi replay

public class SettingPanel : MonoBehaviour
{
    [SerializeField] Toggle soundTg;
    [SerializeField] GameObject soundOn, soundOff;
    [SerializeField] Button exitBtn, replayBtn, backBtn;

    private void Start()
    {
        // gắn sự kiện cho toggle và button
        soundTg.onValueChanged.AddListener(OnSoundToggleChanged);
        exitBtn.onClick.AddListener(OnExitClicked);
        replayBtn.onClick.AddListener(OnReplayClicked);
        backBtn.onClick.AddListener(OnBackClicked);

        // khởi tạo trạng thái ban đầu
        UpdateSoundUI(soundTg.isOn);
    }
    void OnSoundToggleChanged(bool isOn)
    {
        UpdateSoundUI(isOn);
        SoundMN.Instance.ToggleSound(isOn);
    }

    void UpdateSoundUI(bool isOn)
    {
        if (soundOn != null) soundOn.SetActive(isOn);
        if (soundOff != null) soundOff.SetActive(!isOn);
    }

    void OnExitClicked()
    {
        gameObject.SetActive(false);
    }

    void OnReplayClicked()
    {
        GameManager.Instance.RestartGame();
        gameObject.SetActive(false);
    }

    void OnBackClicked()
    {
        SceneManager.LoadScene("LevelScene");
    }
    public void ShowPanel() 
    {
        gameObject.SetActive(true);
    }
}
