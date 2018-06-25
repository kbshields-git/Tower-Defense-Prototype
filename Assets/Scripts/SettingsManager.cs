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
    public GameObject applyButton;
    public Resolution[] resolutions;

    #endregion

    public GameSettings gameSettings;
    private string gameSettingsFile = "gamesettings.json";
    private int scrnWidth;
    private int scrnHeight;
    public bool settingsChanged;
    

    void OnEnable()
    {
        gameSettings = new GameSettings();
        // Keep track of when changes happen, so we can show the apply button
        settingsChanged = false;
        applyButton.SetActive(false);

        // Setup delegate methods for when settings change
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
        if (!settingsChanged) { OnSettingsChanged(); }
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
        if (!settingsChanged) { OnSettingsChanged(); }
    }

    public void OnResolutionChange()
    {
        scrnWidth = resolutions[resolutionDropdown.value].width;
        scrnHeight = resolutions[resolutionDropdown.value].height;
        Screen.SetResolution(scrnWidth, scrnHeight, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
        if (!settingsChanged) { OnSettingsChanged(); }
    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;
        if (!settingsChanged) { OnSettingsChanged(); }
    }

    public void OnAntiAliasingChange()
    {
        gameSettings.antiAliasing = antiAliasingDropdown.value;
        if (antiAliasingDropdown.value > 0)
        {
            QualitySettings.antiAliasing = (int)Mathf.Pow(2, antiAliasingDropdown.value);
        }
        else
        {
            QualitySettings.antiAliasing = 0;
        }
        if (!settingsChanged) { OnSettingsChanged(); }
    }

    public void OnVsyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vsync = vSyncDropdown.value;
        if (!settingsChanged) { OnSettingsChanged(); }
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
        if (!settingsChanged) { OnSettingsChanged(); }
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
        if (!settingsChanged) { OnSettingsChanged(); }
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
        if (!settingsChanged) { OnSettingsChanged(); }
    }

    public void OnSettingsChanged()
    {
        settingsChanged = true;
        applyButton.SetActive(true);
    }
    #endregion

    #region FileHandling
    public void SaveSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, gameSettingsFile);
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(filePath, jsonData);
        settingsChanged = false;
        applyButton.SetActive(false);
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
            settingsChanged = false;
            applyButton.SetActive(false);
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
