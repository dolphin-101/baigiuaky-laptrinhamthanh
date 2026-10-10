using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class homesetting : MonoBehaviour
{
    [Header("--- Quản lý Đóng/Mở Panel ---")]
    public GameObject panelSetting; 
    public GameObject overlayDarkBackground;
    public Button btnOpenSetting;   
    public Button btnCloseSetting; 

    [Header("--- Quản lý Nút Âm Thanh ---")]
    public Button btnToggleMusic;
    public Button btnToggleSFX;

    [Header("Hình ảnh Trạng thái")]
    public Sprite imgMusicOn;
    public Sprite imgMusicOff;
    public Sprite imgSFXOn;
    public Sprite imgSFXOff;

    public string buttonClickSound = "event:/button";
    private bool isMusicOn = true;
    private bool isSFXOn = true;


    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;
    private FMOD.Studio.EventInstance pauseSnapshot;

    void Start()
    {
        if (panelSetting) panelSetting.SetActive(false);
        if (overlayDarkBackground) overlayDarkBackground.SetActive(false);

        if (btnOpenSetting) btnOpenSetting.onClick.AddListener(OpenSettingPanel);
        if (btnCloseSetting) btnCloseSetting.onClick.AddListener(CloseSettingPanel);

        musicBus = RuntimeManager.GetBus("bus:/music");
        sfxBus = RuntimeManager.GetBus("bus:/sfx");

        pauseSnapshot = RuntimeManager.CreateInstance("snapshot:/PauseSnapshot");

        isMusicOn = PlayerPrefs.GetInt("MusicState", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXState", 1) == 1;
        UpdateMusicState();
        UpdateSFXState();

        if (btnToggleMusic) btnToggleMusic.onClick.AddListener(ToggleMusic);
        if (btnToggleSFX) btnToggleSFX.onClick.AddListener(ToggleSFX);
    }

    

    void OpenSettingPanel()
    {
        if (overlayDarkBackground) overlayDarkBackground.SetActive(true);
        if (panelSetting) panelSetting.SetActive(true);

        pauseSnapshot.start();
    }

    void CloseSettingPanel()
    {
        if (overlayDarkBackground) overlayDarkBackground.SetActive(false);
        if (panelSetting) panelSetting.SetActive(false);

        pauseSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicState", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMusicState();
    }

    void UpdateMusicState()
    {
        if (btnToggleMusic)
            btnToggleMusic.GetComponent<Image>().sprite = isMusicOn ? imgMusicOn : imgMusicOff;
        musicBus.setMute(!isMusicOn);
    }

    void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt("SFXState", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSFXState();
    }

    void UpdateSFXState()
    {
        if (btnToggleSFX)
            btnToggleSFX.GetComponent<Image>().sprite = isSFXOn ? imgSFXOn : imgSFXOff;
        sfxBus.setMute(!isSFXOn);
    }

    void OnDestroy()
    {
        pauseSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        pauseSnapshot.release();
    }
}