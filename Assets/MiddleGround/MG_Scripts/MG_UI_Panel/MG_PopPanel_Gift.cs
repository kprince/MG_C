using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Gift : MG_UIBase
    {
        public Text text_remaining;
        public Text text_button;
        public Button btn_open;
        public Button btn_giveup;
        public Transform trans_light;
        public GameObject go_adIcon;
        Image img_giveup;
        bool isGiveup = false;
        protected override void Awake()
        {
            base.Awake();
            btn_open.onClick.AddListener(OnOpen);
            btn_giveup.onClick.AddListener(OnGiveup);
            img_giveup = btn_giveup.image;
        }
        int clickAdTime = 0;
        void OnOpen()
        {
            MG_Manager.Play_ButtonClick();
            if (MG_SaveManager.FirstCome && MG_Manager.Instance.NeedFirstComeReward)
            {
                MG_SaveManager.FirstCome = false;
                OpenGift();
            }
            else
            {
                clickAdTime++;
                MG_Manager.ShowRV(OpenGift, clickAdTime, "Open Gift , reward is " + MG_Manager.Instance.RewardType);
            }
        }
        void OnGiveup()
        {
            MG_Manager.Play_ButtonClick();
            MG_Manager.ShowIV(Giveup, "Giveup Gift , reward is " + MG_Manager.Instance.RewardType);
        }
        void OpenGift()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.GiftPanel);
            switch (MG_Manager.Instance.RewardType)
            {
                case MG_RewardType.Gold:
                    MG_SaveManager.TodayExtraRewardTimes--;
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.FreeClaim, MG_RewardType.Gold, MG_Manager.Instance.RewardNum);
                    break;
                case MG_RewardType.Cash:
                    MG_Manager.Instance.Show_CashRewardPanel(MG_RewardPanelType.FreeClaim, MG_Manager.Instance.RewardNum);
                    break;
                default:
                    Debug.LogError("Open MG_Gift Error : rewardType is error.");
                    break;
            }
        }
        void Giveup()
        {
            isGiveup = true;
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.GiftPanel);
        }

        public override IEnumerator OnEnter()
        {
            isGiveup = false;
            bool needShowGiveup = false;
            img_giveup.color = Color.clear;
            img_giveup.raycastTarget = false;
            text_remaining.text = "Remaining : " + MG_SaveManager.TodayExtraRewardTimes;
            StartCoroutine("AutoRotateGiftLight");
            if (MG_SaveManager.FirstCome && MG_Manager.Instance.NeedFirstComeReward)
            {
                go_adIcon.SetActive(false);
                text_remaining.gameObject.SetActive(false);
                text_button.text = "OPEN   ";
            }
            else
            {
                go_adIcon.SetActive(true);
                text_remaining.gameObject.SetActive(true);
                text_button.text = "OPEN";
                btn_giveup.gameObject.SetActive(true);
                needShowGiveup = true;
            }

            Transform transAll = transform.GetChild(1);
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = true;
            while (transAll.localScale.x < 1)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale += new Vector3(addValue, addValue);
                canvasGroup.alpha += addValue;
            }
            transAll.localScale = Vector3.one;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            if (needShowGiveup)
                StartCoroutine("WaitShowNothanks");
        }

        public override IEnumerator OnExit()
        {

            Transform transAll = transform.GetChild(1);
            canvasGroup.interactable = false;
            while (transAll.localScale.x > 0.8f)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale -= new Vector3(addValue, addValue);
                canvasGroup.alpha -= addValue;
            }
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            StopCoroutine("AutoRotateGiftLight");
            StopCoroutine("WaitShowNothanks");

            if (isGiveup)
            {
                if (MG_Manager.Instance.hasGift)
                {
                    MG_Manager.Instance.hasGift = false;
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                }
                else if (MG_Manager.Instance.willRateus)
                {
                    MG_Manager.Instance.willRateus = false;
                    MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.Rateus);
                }
                else
                    MG_UIManager.Instance.MenuPanel.CheckGuid();
            }
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator AutoRotateGiftLight()
        {
            while (true)
            {
                yield return null;
                trans_light.Rotate(0, 0, -Time.unscaledDeltaTime * 4);
            }
        }
        IEnumerator WaitShowNothanks()
        {
            if (img_giveup.color.a > 0)
                yield break;
            yield return MG_Manager.WaitForSeconds(1);
            while (img_giveup.color.a < 1)
            {
                yield return null;
                img_giveup.color += Color.white * Time.unscaledDeltaTime * 2;
            }
            img_giveup.color = Color.white;
            img_giveup.raycastTarget = true;
        }
    }
}
