﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonCore.Config;
using CommonCore.UI;
using CommonCore.Input;

namespace CommonCore.UI
{

    /// <summary>
    /// Controller for config panel
    /// </summary>
    public class ConfigPanelController : PanelController
    {
        //labels for antialiasing quality names; these match CommonCore defaults
        private static readonly string[] AntialiasingSettingNames = new string[] { "Off", "FXAA", "SMAA", "SMAA+" };

        [Header("Containers"), SerializeField]
        private RectTransform PanelContainer = null;

        [Header("Basic Config Options")]
        public Slider LookSpeedSlider;
        public Toggle LookYToggle;

        public Slider GraphicsQualitySlider;
        public Text GraphicsQualityLabel;
        public Slider AntialiasingQualitySlider;
        public Text AntialiasingQualityLabel;
        public Slider ViewDistanceSlider;
        public Text ViewDistanceLabel;

        public Slider SoundVolumeSlider;
        public Slider MusicVolumeSlider;
        public Dropdown ChannelDropdown;

        public Dropdown InputDropdown;

        public override void SignalInitialPaint()
        {
            base.SignalInitialPaint();

            //initialize subpanels
            var subpanels = ConfigModule.Instance.SortedConfigPanels;
            foreach(var subpanel in subpanels)
            {
                var subpanelGO = Instantiate(subpanel, PanelContainer);
                subpanelGO.SetActive(true);
            }
        }

        public override void SignalPaint()
        {
            base.SignalPaint();

            PaintValues();
        }

        private void PaintValues()
        {
            LookSpeedSlider.value = ConfigState.Instance.LookSpeed;
            LookYToggle.isOn = ConfigState.Instance.LookInvert;

            GraphicsQualitySlider.maxValue = QualitySettings.names.Length - 1;
            GraphicsQualitySlider.value = QualitySettings.GetQualityLevel();
            GraphicsQualitySlider.interactable = true;

            AntialiasingQualitySlider.value = ConfigState.Instance.AntialiasingQuality;
            ViewDistanceSlider.value = ConfigState.Instance.ViewDistance;

            SoundVolumeSlider.value = ConfigState.Instance.SoundVolume;
            MusicVolumeSlider.value = ConfigState.Instance.MusicVolume;

            var cList = new List<string>(Enum.GetNames(typeof(AudioSpeakerMode)));
            ChannelDropdown.ClearOptions();
            ChannelDropdown.AddOptions(cList);
            ChannelDropdown.value = cList.IndexOf(AudioSettings.GetConfiguration().speakerMode.ToString());

            var iList = MappedInput.AvailableMappers.ToList();
            InputDropdown.ClearOptions();
            InputDropdown.AddOptions(iList);
            InputDropdown.value = iList.IndexOf(ConfigState.Instance.InputMapper);

            //handle subpanels
            foreach (var subpanel in PanelContainer.GetComponentsInChildren<ConfigSubpanelController>())
                subpanel.PaintValues();
        }

        public void OnQualitySliderChanged()
        {
            GraphicsQualityLabel.text = QualitySettings.names[(int)GraphicsQualitySlider.value];
        }

        public void OnAntialiasingSliderChanged()
        {
            AntialiasingQualityLabel.text = AntialiasingSettingNames[(int)AntialiasingQualitySlider.value];
        }

        public void OnViewDistanceSliderChanged()
        {
            ViewDistanceLabel.text = ((int)ViewDistanceSlider.value).ToString();
        }

        public void OnClickConfirm()
        {
            UpdateValues();
            ConfigState.Save();
            ConfigModule.Apply();
            //MappedInput.SetMapper(ConfigState.Instance.InputMapper); //we need to do this manually unfortunately...
            Modal.PushMessageModal("Applied settings changes!", "Changes Applied", null, OnConfirmed, true);
        }

        public void OnClickRevert()
        {
            Modal.PushConfirmModal("This will revert all unsaved settings changes", "Revert Changes", null, null, null, (status, tag, result) =>
            {
                if(status == ModalStatusCode.Complete && result)
                {
                    PaintValues();
                }
                else
                {

                }
            }, true);
        }

        public void OnClickConfigureInput()
        {
            MappedInput.ConfigureMapper();
        }

        private void OnConfirmed(ModalStatusCode status, string tag)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if(sceneName == "MainMenuScene")
                SceneManager.LoadScene(sceneName);
        }

        private void UpdateValues()
        {
            ConfigState.Instance.LookSpeed = LookSpeedSlider.value;
            ConfigState.Instance.LookInvert = LookYToggle.isOn;

            QualitySettings.SetQualityLevel( (int)GraphicsQualitySlider.value);
            ConfigState.Instance.AntialiasingQuality = (int)AntialiasingQualitySlider.value;
            ConfigState.Instance.ViewDistance = ViewDistanceSlider.value;

            ConfigState.Instance.SoundVolume = SoundVolumeSlider.value;
            ConfigState.Instance.MusicVolume = MusicVolumeSlider.value;
            ConfigState.Instance.SpeakerMode = (AudioSpeakerMode)Enum.Parse(typeof(AudioSpeakerMode), ChannelDropdown.options[ChannelDropdown.value].text);

            //handle subpanels
            foreach (var subpanel in PanelContainer.GetComponentsInChildren<ConfigSubpanelController>())
                subpanel.UpdateValues();
        }
    }
}