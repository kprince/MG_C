using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Reward : MG_UIBase
    {
        public Image img_rewardIcon;
        public Text text_rewardNum;
        public Text text_buttonText;
        public Text text_claim;
        public Text text_claimUnderline;
        public Text text_title;
        public Button btn_;
        public Button btn_claim;
        public GameObject go_adIcon;
        public CanvasGroup cg_nothanks;
        Transform trans_btn;
        readonly Vector2 v2_nogiveup_btnPos = new Vector2(0, -284);
        readonly Vector2 v2_giveup_btnPos = new Vector2(0, -238);

        MG_RewardType RewardType = MG_RewardType.Gold;
        MG_RewardPanelType RewardPanelType = MG_RewardPanelType.AdClaim;
        int RewardNum = 1;
        float RewardMutiple = 1;
        protected override void Awake()
        {
            base.Awake();
            btn_.onClick.AddListener(OnButtonClick);
            btn_claim.onClick.AddListener(OnClaim);
            trans_btn = btn_.transform;
        }
        int clickAdTime = 0;
        void OnButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (needAd)
            {
                switch (RewardPanelType)
                {
                    case MG_RewardPanelType.AdDouble:
                        RewardMutiple = 2;
                        clickAdTime++;
                        MG_Manager.ShowRV(GetReward, clickAdTime, "Get " + RewardType + " Reward in " + RewardPanelType + " RewardPanel");
                        break;
                    case MG_RewardPanelType.AdClaim:
                        clickAdTime++;
                        MG_Manager.ShowRV(GetReward, clickAdTime, "Get " + RewardType + " Reward in " + RewardPanelType + " RewardPanel");
                        break;
                    case MG_RewardPanelType.AdRandom:
                        clickAdTime++;
                        if (RewardType == MG_RewardType.Gold)
                            MG_Manager.ShowRV(OnAdCallback, clickAdTime, "Get " + RewardType + " Reward in " + RewardPanelType + " RewardPanel");
                        else
                            MG_Manager.ShowRV(GetReward, clickAdTime, "Get " + RewardType + " Reward in " + RewardPanelType + " RewardPanel");
                        break;
                }
            }
            else
            {
                switch (RewardPanelType)
                {
                    case MG_RewardPanelType.AdDouble:
                        RewardMutiple = 2;
                        GetReward();
                        break;
                    case MG_RewardPanelType.AdClaim:
                    case MG_RewardPanelType.AdRandom:
                    case MG_RewardPanelType.FreeMutipleClaim:
                        GetReward();
                        break;
                }
            }
        }
        void OnAdCallback()
        {
            StopCoroutine("WaitShowNothanks");
            needAd = false;
            text_buttonText.text = "Claim";
            text_rewardNum.text = "+" + (int)(RewardNum * RewardMutiple);
            cg_nothanks.alpha = 0;
            cg_nothanks.blocksRaycasts = false;
            trans_btn.localPosition = v2_nogiveup_btnPos;
            go_adIcon.SetActive(false);
        }
        bool hasLookAd = false;
        void GetReward()
        {
            hasLookAd = true;
            clickAdTime = 0;
            int finalRewardNum = (int)(RewardNum * RewardMutiple);
            Vector3 flyStartPos = img_rewardIcon.transform.position;
            switch (RewardType)
            {
                case MG_RewardType.Gold:
                    MG_Manager.Instance.Add_Save_Gold(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.OneGold, finalRewardNum);
                    break;
                case MG_RewardType.Diamond:
                    MG_Manager.Instance.Add_Save_CurrentDiamond(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.Diamond, finalRewardNum);
                    break;
                case MG_RewardType.RedDiamond:
                    MG_Manager.Instance.Add_Save_RedDiamond(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.RedDiamond, finalRewardNum);
                    break;
                case MG_RewardType.StarCoin:
                    MG_Manager.Instance.Add_Save_CurrentStarCoin(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.StarCoin, finalRewardNum);
                    break;
                case MG_RewardType.Cherry:
                    MG_Manager.Instance.Add_Save_CurrentFruits(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.Cherry, finalRewardNum);
                    break;
                case MG_RewardType.Orange:
                    MG_Manager.Instance.Add_Save_CurrentFruits(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.Orange, finalRewardNum);
                    break;
                case MG_RewardType.Watermalen:
                    MG_Manager.Instance.Add_Save_CurrentFruits(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.Watermalen, finalRewardNum);
                    break;
                case MG_RewardType.ScratchTicket:
                    MG_Manager.Instance.Add_Save_ScratchTicket(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.ScratchTicket, finalRewardNum);
                    break;
                case MG_RewardType.WheelTicket:
                    MG_Manager.Instance.Add_Save_WheelTickets(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.WheelTicket, finalRewardNum);
                    break;
                case MG_RewardType.SSS:
                    MG_Manager.Instance.Add_Save_CurrentSSS(finalRewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(flyStartPos, MG_MenuFlyTarget.SSS, finalRewardNum);
                    break;
            }
            if(hasLookAd)
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.MostRewardPanel);
        }
        void OnClaim()
        {
            MG_Manager.Play_ButtonClick();
            MG_Manager.ShowIV(GiveupReward, "Givp up " + RewardType + " Reward in " + RewardPanelType + " RewardPanel");
        }
        void GiveupReward()
        {
            RewardMutiple = 1;
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.MostRewardPanel);
        }
        bool needShowNothanks = false;
        public override IEnumerator OnEnter()
        {
            RewardType = MG_Manager.Instance.RewardType;
            RewardPanelType = MG_Manager.Instance.RewardPanelType;
            RewardNum = MG_Manager.Instance.RewardNum;
            RewardMutiple = MG_Manager.Instance.RewardMutiple;

            hasLookAd = false;

            img_rewardIcon.sprite = MG_Manager.Instance.Get_RewardSprite(RewardType);
            cg_nothanks.alpha = 0;
            cg_nothanks.blocksRaycasts = false;
            needShowNothanks = false;
            switch (RewardPanelType)
            {
                case MG_RewardPanelType.AdClaim:
                    needAd = true;
                    text_buttonText.text = "    Claim";
                    text_claim.text = "Give up";
                    text_claimUnderline.text = "─────";
                    go_adIcon.SetActive(true);
                    trans_btn.localPosition = v2_giveup_btnPos;
                    text_rewardNum.text = "+" + RewardNum;
                    break;
                case MG_RewardPanelType.AdDouble:
                    needAd = true;
                    text_buttonText.text = "    Claim x2";
                    text_claim.text = "Claim Reward!";
                    text_claimUnderline.text = "─────────";
                    needShowNothanks = true;
                    go_adIcon.SetActive(true);
                    trans_btn.localPosition = v2_giveup_btnPos;
                    text_rewardNum.text = "+" + RewardNum;
                    break;
                case MG_RewardPanelType.AdRandom:
                    needAd = true;
                    text_claim.text = "Claim Reward!";
                    text_claimUnderline.text = "─────────";
                    needShowNothanks = true;
                    go_adIcon.SetActive(true);
                    trans_btn.localPosition = v2_giveup_btnPos;
                    text_rewardNum.text = "+" + RewardNum;
                    if (RewardType == MG_RewardType.Gold &&MG_SaveManager.Current_GamePanel==(int)MG_GamePanelType.DicePanel)
                    {
                        text_buttonText.text = "    Random x1~5";
                    }
                    else
                    {
                        text_buttonText.text = "    Random x2~10";
                        RewardMutiple = Random.Range(2, 11);
                    }
                    if (RewardType == MG_RewardType.Diamond && !MG_SaveManager.GuidSlots && MG_SaveManager.Current_GamePanel != -1)
                        MG_Manager.Instance.next_GuidType = MG_Guid_Type.SlotsGuid;
                    SetSpecialTokenInfo();
                    break;
                case MG_RewardPanelType.FreeMutipleClaim:
                    needAd = false;
                    text_buttonText.text = "Claim";
                    go_adIcon.SetActive(false);
                    trans_btn.localPosition = v2_nogiveup_btnPos;
                    text_rewardNum.text = "+" + (int)(RewardNum * RewardMutiple);
                    break;
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

            MG_Manager.Instance.Play_Effect();
            if (needShowNothanks)
                StartCoroutine("WaitShowNothanks");
        }
        bool needAd = false;
        void SetSpecialTokenInfo()
        {
            switch (RewardType)
            {
                case MG_RewardType.SSS:
                    SetSpecialShowTextButton(MG_SaveManager.Get777Times);
                    if (!MG_SaveManager.GuidScratch)
                        MG_Manager.Instance.next_GuidType = MG_Guid_Type.ScratchGuid;
                    break;
                case MG_RewardType.StarCoin:
                    SetSpecialShowTextButton(MG_SaveManager.GetStarcoinTimes);
                    break;
                case MG_RewardType.Cherry:
                case MG_RewardType.Orange:
                case MG_RewardType.Watermalen:
                    SetSpecialShowTextButton(MG_SaveManager.GetFruitsTimes);
                    break;
                default:
                    needShowNothanks = true;
                    break;
            }
        }
        void SetSpecialShowTextButton(int times)
        {
            if (times == 0)
            {
                go_adIcon.SetActive(false);
                RewardNum = 3;
                RewardMutiple = 1;
                text_buttonText.text = "Claim";
                text_rewardNum.text = "+" + RewardNum;
                needAd = false;
                trans_btn.localPosition = v2_nogiveup_btnPos;
            }
            else if (times == 1)
            {
                go_adIcon.SetActive(false);
                RewardNum = 2;
                RewardMutiple = 1;
                text_buttonText.text = "Claim";
                text_rewardNum.text = "+" + RewardNum;
                needAd = false;
                trans_btn.localPosition = v2_nogiveup_btnPos;
            }
            else if (times == 2)
            {
                go_adIcon.SetActive(true);
                RewardNum = 2;
                RewardMutiple = 1;
                text_buttonText.text = "    Claim";
                text_rewardNum.text = "+" + RewardNum;
                needAd = true;
                needShowNothanks = true;
            }
            else
            {
                go_adIcon.SetActive(true);
                text_buttonText.text = "    Random x2~10";
                needAd = true;
                needShowNothanks = true;
            }
        }

        public override IEnumerator OnExit()
        {
            clickAdTime = 0;
            if(!hasLookAd)
                switch (RewardPanelType)
                {
                    case MG_RewardPanelType.AdDouble:
                    case MG_RewardPanelType.AdRandom:
                        RewardMutiple = 1;
                        GetReward();
                        break;
                }

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
            StopCoroutine("WaitShowNothanks");
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator WaitShowNothanks()
        {
            if (cg_nothanks.alpha > 0)
                yield break;
            yield return MG_Manager.WaitForSeconds(1);
            while (cg_nothanks.alpha < 1)
            {
                yield return null;
                cg_nothanks.alpha += Time.unscaledDeltaTime * 2;
            }
            cg_nothanks.alpha = 1;
            cg_nothanks.blocksRaycasts = true;
        }
    }
}
