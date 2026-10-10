using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;

    [Header("Giao diện Nút Bấm Settings")]
    public Image musicButtonImage;
    public Image soundButtonImage;

    [Header("Hình ảnh trạng thái Bật / Tắt")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Kho Âm thanh FMOD")]
    public EventReference buttonClickEvent;
    public EventReference gameOverEvent;
    public EventReference gameWinEvent;
    public EventReference failClickEvent;
    public EventReference matchEvent;
    public EventReference objectClickEvent;

    private bool isMusicOn = true;
    private bool isSoundOn = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicBus = RuntimeManager.GetBus("bus:/music");
        sfxBus = RuntimeManager.GetBus("bus:/sfx");

        isMusicOn = PlayerPrefs.GetInt("MusicState", 1) == 1;
        isSoundOn = PlayerPrefs.GetInt("SFXState", 1) == 1;
    }

    void Start()
    {
        ApplyAudioStates();
        UpdateMusicUI();
        UpdateSoundUI();
    }

    private void ApplyAudioStates()
    {
        musicBus.setMute(!isMusicOn);
        sfxBus.setMute(!isSoundOn);
    }


    public void PlayClickSound()
    {
        if (!buttonClickEvent.IsNull)
            RuntimeManager.PlayOneShot(buttonClickEvent);
    }

    public void PlayGameOverSound()
    {
        if (!gameOverEvent.IsNull)
            RuntimeManager.PlayOneShot(gameOverEvent);
    }

    public void PlayGameWinSound()
    {
        if (!gameWinEvent.IsNull)
            RuntimeManager.PlayOneShot(gameWinEvent);
    }

    public void PlayFailClickSound()
    {
        if (!failClickEvent.IsNull)
            RuntimeManager.PlayOneShot(failClickEvent);
    }

    public void PlayMatchSound()
    {
        if (!matchEvent.IsNull)
            RuntimeManager.PlayOneShot(matchEvent);
    }

    public void PlayMatchSoundWithCombo(float comboLevel)
    {
        if (!matchEvent.IsNull)
        {
            FMOD.Studio.EventInstance matchInstance = RuntimeManager.CreateInstance(matchEvent);
            matchInstance.setParameterByName("Combolevel1", comboLevel);
            matchInstance.start();
            matchInstance.release();
        }
    }

    public void PlayObjectClickSound()
    {
        if (!objectClickEvent.IsNull)
            RuntimeManager.PlayOneShot(objectClickEvent);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        PlayerPrefs.SetInt("MusicState", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        musicBus.setMute(!isMusicOn);

        UpdateMusicUI();
        PlayClickSound();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        PlayerPrefs.SetInt("SFXState", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        sfxBus.setMute(!isSoundOn);

        UpdateSoundUI();

        if (isSoundOn)
        {
            PlayClickSound();
        }
    }


    private void UpdateMusicUI()
    {
        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
        }
    }

    private void UpdateSoundUI()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }
}