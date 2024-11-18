using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace UI.Settings
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private TMP_Dropdown resolutionDropdown, qualityLevelsDropdown;
        private List<Resolution> resolutions;

        private void Awake()
        {
            resolutions = Screen.resolutions.Reverse().Distinct().ToList();
            RefreshResolutions();
            RefreshQualityLevels();
            Canvas.ForceUpdateCanvases();
        }

        private void RefreshResolutions()
        {
            resolutionDropdown.ClearOptions();

            Dictionary<string, Resolution> resolutionDict = new Dictionary<string, Resolution>();
            int currentResolutionIndex = 0;
            int index = 0;

            foreach (var resolution in Screen.resolutions.Reverse())
            {
                string resolutionKey = $"{resolution.width} x {resolution.height}";
                if (!resolutionDict.TryAdd(resolutionKey, resolution))
                {
                    if (resolution.width == Screen.width && resolution.height == Screen.height)
                    {
                        currentResolutionIndex = index;
                    }
                    index++;
                }
            }

            List<string> options = new List<string>(resolutionDict.Keys);
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }


        private void RefreshQualityLevels()
        {
            qualityLevelsDropdown.ClearOptions();

            List<string> qualityLevels = new List<string>(QualitySettings.names);
            int currentQualityLevel = QualitySettings.GetQualityLevel();

            qualityLevelsDropdown.AddOptions(qualityLevels);
            qualityLevelsDropdown.value = currentQualityLevel;
            qualityLevelsDropdown.RefreshShownValue();
        }



        public void SetMusicVolume (float volume)
        {
            audioMixer.SetFloat("Music", Mathf.Log(volume)*20);
        }

        public void SetSoundsVolume(float volume)
        {
            audioMixer.SetFloat("Sounds", Mathf.Log(volume)*20);
        }

        public void SetScreenResolution(int i)
        {
            var resolution = resolutions.ToArray()[i];
            Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
        }

        public void SetQuality (int i)
        {
            QualitySettings.SetQualityLevel(i);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
    }
}
