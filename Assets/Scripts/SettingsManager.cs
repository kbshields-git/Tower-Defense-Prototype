using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour {
    #region Knobs
    public Toggle fullscreenToggle;
    public TMP_Dropdown fullscreenModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown textureQualityDropdown;
    public TMP_Dropdown antiAliasingDropdown;
    public TMP_Dropdown vSyncDropdown;
    public AudioMixer masterMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Resolution[] resolutions;
    #endregion

    public GameSettings gameSettings;
    private string gameSettingsFile = "gamesettings.json";
    private int scrnWidth;
    private int scrnHeight;

    

    void OnEnable()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(fullscreenToggle); });
        fullscreenModeDropdown.onValueChanged.AddListener(delegate { OnFullScreenMode(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antiAliasingDropdown.onValueChanged.AddListener(delegate { OnAntiAliasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnVsyncChange(); });
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChange);

        
        Resolution curRes = Screen.currentResolution;
        resolutions = Screen.resolutions;
        int tempResIndex = 0;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            if(resolution.ToString() == curRes.ToString())
            {
                gameSettings.resolutionIndex = resolutionDropdown.value = tempResIndex;                
            }
            tempResIndex++;
        }

        LoadSettings();
    }

    #region SettingDelegateMethods
    public void OnFullscreenToggle(Toggle change)
    {
        if (!change.isOn)
        {
            gameSettings.fullScreen = fullscreenToggle.isOn = false;
            //Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(scrnWidth, scrnHeight, false);

        }
        else if(change.isOn)
        {
            gameSettings.fullScreen = fullscreenToggle.isOn = true;
            Screen.fullScreen = !Screen.fullScreen;
            if (gameSettings.fullScreenMode == 0)
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            Screen.SetResolution(scrnWidth, scrnHeight, true);
        }        
    }
    
    public void OnFullScreenMode()
    {
        if (fullscreenModeDropdown.value == 0)
        {
            gameSettings.fullScreenMode = fullscreenModeDropdown.value = 0;
        }
        else if(fullscreenModeDropdown.value == 1)
        {
            gameSettings.fullScreenMode = fullscreenModeDropdown.value = 1;
        }
        Screen.SetResolution(scrnWidth, scrnHeight, Screen.fullScreen);
    }

    public void OnResolutionChange()
    {
        scrnWidth = resolutions[resolutionDropdown.value].width;
        scrnHeight = resolutions[resolutionDropdown.value].height;
        Screen.SetResolution(scrnWidth, scrnHeight, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;        
    }

    public void OnAntiAliasingChange()
    {
        QualitySettings.antiAliasing = gameSettings.antiAliasing =(int)Mathf.Pow(2,antiAliasingDropdown.value);
    }

    public void OnVsyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vsync = vSyncDropdown.value;
    }

    public void OnMasterVolumeChange(float volume)
    {
        gameSettings.masterVolume = volume;
        if (volume == 0)
        {
            masterMixer.SetFloat("masterVol", -80);
        }
        else
        {
            masterMixer.SetFloat("masterVol", Mathf.Log10(volume) * 20);
        }
    }

    public void OnMusicVolumeChange(float volume)
    {
        gameSettings.musicVolume = volume;
        if (volume == 0)
        {
            masterMixer.SetFloat("musicVol", -80);
        }
        else
        {
            masterMixer.SetFloat("musicVol", Mathf.Log10(volume) * 20);
        }
    }

    public void OnSfxVolumeChange(float volume)
    {
        gameSettings.sfxVolume = volume;
        if (volume == 0)
        {
            masterMixer.SetFloat("sfxVol", -80);
        }
        else
        {
            masterMixer.SetFloat("sfxVol", Mathf.Log10(volume) * 20);
        }
    }
    #endregion

    #region FileHandling
    public void SaveSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, gameSettingsFile);
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(filePath, jsonData);
    }

    public void LoadSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, gameSettingsFile);
        if (File.Exists(filePath))
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(filePath));
            
            fullscreenToggle.isOn = gameSettings.fullScreen;
            Screen.fullScreen = gameSettings.fullScreen;
            fullscreenModeDropdown.value = gameSettings.fullScreenMode;
            resolutionDropdown.value = gameSettings.resolutionIndex;
            textureQualityDropdown.value = gameSettings.textureQuality;
            antiAliasingDropdown.value = gameSettings.antiAliasing;
            vSyncDropdown.value = gameSettings.vsync;
            masterVolumeSlider.value = Mathf.Clamp01(gameSettings.masterVolume);
            musicVolumeSlider.value = Mathf.Clamp01(gameSettings.musicVolume);
            sfxVolumeSlider.value = Mathf.Clamp01(gameSettings.sfxVolume);
        }
        else
        {
            //File does not yet exist. Lets take the current active settings and set gamesettings that way.
            gameSettings.fullScreen = fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenModeDropdown.value = gameSettings.fullScreenMode = 0;
            //We should have just matched this in OnEnable()
            //If not, we'll end up with the lowest res
            gameSettings.resolutionIndex = resolutionDropdown.value;
            gameSettings.textureQuality = textureQualityDropdown.value = QualitySettings.masterTextureLimit;
            gameSettings.antiAliasing = antiAliasingDropdown.value = QualitySettings.antiAliasing;
            gameSettings.vsync = vSyncDropdown.value = QualitySettings.vSyncCount;
            float volume = 1.0f;
            masterVolumeSlider.value = gameSettings.masterVolume = volume;
            musicVolumeSlider.value = gameSettings.musicVolume = volume;
            sfxVolumeSlider.value = gameSettings.sfxVolume = volume;

            //Now lets just go ahead and save these settings for next time
            SaveSettings();
        }

    }
    #endregion
}
