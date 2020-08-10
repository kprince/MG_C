using MiddleGround.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Setting : MG_UIBase
    {
        public Button btn_Close;
        public Button btn_Sound;
        public Button btn_Music;
        public Button btn_Wheel;
        public Button btn_Slots;
        public Button btn_Scratch;
        public Button btn_Dice;
        public Button btn_Close2;
        public Image img_SoundSwitch;
        public Image img_MusicSwitch;
        Sprite sp_SwitchOn;
        Sprite sp_SwitchOff;
        protected override void Awake()
        {
            base.Awake();
            btn_Close.onClick.AddListener(OnCloseButtonClick);
            btn_Close2.onClick.AddListener(OnCloseButtonClick);
            btn_Sound.onClick.AddListener(OnSoundButtonClick);
            btn_Music.onClick.AddListener(OnMusicButtonClick);
            btn_Wheel.onClick.AddListener(OnWheelButtonClick);
            btn_Slots.onClick.AddListener(OnSlotsButtonClick);
            btn_Scratch.onClick.AddListener(OnScratchButtonClick);
            btn_Dice.onClick.AddListener(OnDiceButtonClick);
            sp_SwitchOn = img_SoundSwitch.sprite;
            sp_SwitchOff = img_MusicSwitch.sprite;
        }
        void OnWheelButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
            MG_UIManager.Instance.MenuPanel.OnWheelButtonClick();
        }
        void OnSlotsButtonClick()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
            MG_UIManager.Instance.MenuPanel.OnSlotsButtonClick();
        }
        void OnScratchButtonClick()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
            MG_UIManager.Instance.MenuPanel.OnScratchButtonClick();

        }
        void OnDiceButtonClick()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
            MG_UIManager.Instance.MenuPanel.OnDiceButtonClick();

        }
        void OnCloseButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
        }
        void OnSoundButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            bool oldstate = MG_Manager.Instance.Get_Save_SoundOn();
            MG_Manager.Instance.Set_Save_SoundOn(!oldstate);
            MG_AudioManager.Instance.SetSoundState(oldstate);
            img_SoundSwitch.sprite = oldstate ? sp_SwitchOff : sp_SwitchOn;
        }
        void OnMusicButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            bool oldstate = MG_Manager.Instance.Get_Save_MusicOn();
            MG_Manager.Instance.Set_Save_MusicOn(!oldstate);
            MG_AudioManager.Instance.SetMusicState(oldstate);
            img_MusicSwitch.sprite = oldstate ? sp_SwitchOff : sp_SwitchOn;
        }
        Vector3 startPos = new Vector3(-1395.205f, 0);
        Vector3 endPos = new Vector3(-540f, 0);
        const float offsetAlpha = 0.9f;
        public override IEnumerator OnEnter()
        {
            img_MusicSwitch.sprite =MG_Manager.Instance.Get_Save_MusicOn() ? sp_SwitchOn : sp_SwitchOff;
            img_SoundSwitch.sprite = MG_Manager.Instance.Get_Save_SoundOn() ? sp_SwitchOn : sp_SwitchOff;
            yield return null;
            Transform transAll = transform.GetChild(1);
            Image img_BG = transform.GetChild(0).GetComponent<Image>();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
            float progress = 0;
            float finalProgress;
            while (progress<1)
            {
                yield return null;
                progress += Time.unscaledDeltaTime * 2;
                finalProgress = -Mathf.Pow(progress - 1, 4f) + 1;
                transAll.localPosition = Vector3.Lerp(startPos, endPos, finalProgress);
                img_BG.color = new Color(0, 0, 0, offsetAlpha * finalProgress);
            }
            yield return null;
            transAll.localPosition = endPos;
            canvasGroup.interactable = true;

        }

        public override IEnumerator OnExit()
        {
            Transform transAll = transform.GetChild(1);
            Image img_BG = transform.GetChild(0).GetComponent<Image>();
            canvasGroup.interactable = false;
            float progress = 0;
            float finalProgress;
            while (progress<1)
            {
                yield return null;
                progress += Time.unscaledDeltaTime * 2;
                finalProgress = -Mathf.Pow(progress - 1, 4f) + 1;
                transAll.localPosition = Vector3.Lerp(endPos, startPos, finalProgress);
                img_BG.color = new Color(0, 0, 0, offsetAlpha *(1- finalProgress));
            }
            yield return null;
            transAll.localPosition = startPos;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
