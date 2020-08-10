using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_CashReward : MG_UIBase
    {
        public Image img_cashIcon;
        public Text text_rewardnum;
        public Text text_get;
        public Text text_reaminTime;
        public Transform trans_light;
        public GameObject go_adicon;
        public GameObject go_redeem;
        public Button btn_get;
        public Button btn_giveup;
        Image img_giveup;
        bool isPackB;

        MG_RewardType RewardType = MG_RewardType.Gold;
        MG_RewardPanelType RewardPanelType = MG_RewardPanelType.AdClaim;
        int RewardNum = 1;
        float RewardMutiple = 1;

        protected override void Awake()
        {
            base.Awake();
            btn_get.onClick.AddListener(OnGet);
            btn_giveup.onClick.AddListener(OnGiveup);
            img_giveup = btn_giveup.image;
            isPackB = MG_Manager.Instance.Get_Save_PackB();
            Sprite sp_manyCash;
            if (isPackB)
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.CashRewardPanel).GetSprite("MG_Sprite_CashReward_CashIconB");
            else
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.CashRewardPanel).GetSprite("MG_Sprite_CashReward_CashIconA");
            img_cashIcon.sprite = sp_manyCash;
            go_redeem.SetActive(isPackB);
        }
        int clikcAdTime = 0;
        void OnGet()
        {
            MG_Manager.Play_ButtonClick();
            switch (RewardPanelType)
            {
                case MG_RewardPanelType.AdClaim:
                    clikcAdTime++;
                    RewardMutiple = 1;
                    MG_Manager.ShowRV(GetReward, clikcAdTime, "Get Cash Reward in " + RewardPanelType + " RewardPanel");
                    break;
                case MG_RewardPanelType.AdRandom:
                    clikcAdTime++;
                    MG_Manager.ShowRV(GetReward, clikcAdTime, "Get Cash Reward in " + RewardPanelType + " RewardPanel");
                    break;
                case MG_RewardPanelType.FreeMutipleClaim:
                    GetReward();
                    break;
                case MG_RewardPanelType.FreeClaim:
                    RewardMutiple = 1;
                    GetReward();
                    break;
                default:
                    Debug.LogError("Get MG_Cash Reward Error : panelType is error.");
                    break;
            }
        }
        void GetReward()
        {
            clikcAdTime = 0;
            int finalRewardNum = (int)(RewardNum * RewardMutiple);
            Vector3 flyStartPos = img_cashIcon.transform.position;
            MG_Manager.Instance.Add_Save_CurrentCash(finalRewardNum);
            MG_SaveManager.TodayExtraRewardTimes--;
            MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.Cash, finalRewardNum);
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.CashRewardPanel);
        }
        void OnGiveup()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.CashRewardPanel);
        }
        public override IEnumerator OnEnter()
        {
            bool needShowNothanks = false;
            RewardType = MG_Manager.Instance.RewardType;
            RewardPanelType = MG_Manager.Instance.RewardPanelType;
            RewardNum = MG_Manager.Instance.RewardNum;
            RewardMutiple = MG_Manager.Instance.RewardMutiple;
            if (RewardType != MG_RewardType.Cash)
                Debug.LogError("Show MG_CashReward Panel Error : rewardType is error.");

            text_reaminTime.text = "Remaining:" + MG_SaveManager.TodayExtraRewardTimes;
            text_rewardnum.text = MG_Manager.Get_CashShowText((int)(RewardNum * RewardMutiple));
            switch (RewardPanelType)
            {
                case MG_RewardPanelType.AdClaim:
                    go_adicon.SetActive(true);
                    needShowNothanks = true;
                    text_get.text = "      Save in wallet";
                    break;
                case MG_RewardPanelType.AdRandom:
                    go_adicon.SetActive(true);
                    needShowNothanks = true;
                    text_get.text = "      Random x1~5";
                    break;
                case MG_RewardPanelType.FreeMutipleClaim:
                    go_adicon.SetActive(false);
                    text_get.text = "Save in wallet";
                    break;
                case MG_RewardPanelType.FreeClaim:
                    go_adicon.SetActive(false);
                    text_get.text = "Save in wallet";
                    break;
                default:
                    Debug.LogError("Show MG_CashReward Panel Error : panelType is error.");
                    break;
            }
            StartCoroutine("AutoScaleLight");
            img_giveup.raycastTarget = false;
            img_giveup.color = Color.clear;

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
            MG_Manager.Instance.Play_Effect();
            if (needShowNothanks)
                StartCoroutine("WaitShowNothanks");
        }

        public override IEnumerator OnExit()
        {
            clikcAdTime = 0;
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
            {
                MG_UIManager.Instance.MenuPanel.CheckGuid();
            }

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
            StopCoroutine("AutoScaleLight");
            StopCoroutine("WaitShowNothanks");
        }
        IEnumerator AutoScaleLight()
        {
            float maxScale = 1.3f;
            trans_light.localScale = Vector3.one;
            bool isUp = true;
            Vector3 offset = new Vector2(0.003f, 0.003f);
            while (true)
            {
                yield return null;
                trans_light.localScale += isUp ? offset : -offset;
                if (trans_light.localScale.x >= maxScale)
                    isUp = false;
                if (trans_light.localScale.x <= 1)
                    isUp = true;
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
                img_giveup.color += Color.white * Time.unscaledDeltaTime;
            }
            img_giveup.color = Color.white;
            img_giveup.raycastTarget = true;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
