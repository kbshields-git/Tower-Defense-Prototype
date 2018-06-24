public class GameSettings {
    #region AudioSettings
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    #endregion

    #region VideoSettings
    public bool fullScreen;
    public int fullScreenMode; //0 exclusive, 1 windowed
    public int textureQuality;
    public int antiAliasing;
    public int vsync;
    public int resolutionIndex;

    #endregion
}
