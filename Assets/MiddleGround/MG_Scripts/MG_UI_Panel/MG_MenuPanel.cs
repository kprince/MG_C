using MiddleGround.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_MenuPanel : MG_UIBase
    {
        public Button btn_Back;
        public Button btn_Wheel;
        public Button btn_Scratch;
        public Button btn_Dice;
        public Button btn_Slots;
        public Button btn_Gold;
        public Button btn_Cash;
        public Button btn_SpecialToken;
        public Button btn_RD;
        public Button btn_SlotsOut;
        public Button btn_SlotsOut2;
        public Button btn_WheelOut;

        public Text text_Gold;
        public Text text_Cash;
        public Text text_ScratchTicketNum;
        public Text text_SpecialToken;
        public Text text_rdNum;
        public Text text_wheelOutTime;

        public Image img_SpecialToken;
        public Image img_CashIcon;
        public GameObject go_SpecialToken;

        public RectTransform rect_Top;
        RectTransform rect_OutWheel;
        RectTransform rect_OutSlots2;
        public RectTransform rect_OutWheelBase;
        public GameObject go_bottom;
        public Transform trans_guidMask;
        public Transform trans_guidBase;
        public Transform trans_guidDown;
        public Image img_guidBG;
        public Image img_guidIcon;
        public Text text_guidDes;

        public GameObject go_wheelRP;
        public GameObject go_signRP;
        public GameObject go_scratchRP;
        public GameObject go_cashoutTips_cash;
        public GameObject go_cashoutTips_special;
        public GameObject go_cashoutTips_redDiamond;

        Sprite sp_scratchOn;
        Sprite sp_scratchOff;
        Sprite sp_diceOn;
        Sprite sp_diceOff;
        Sprite sp_slotsOn;
        Sprite sp_slotsOff;
        Sprite sp_wheelOn;
        Sprite sp_wheelOff;
        Sprite sp_back;
        Sprite sp_setting;
        Sprite sp_slotsOutA;
        Sprite sp_slotsOutB;
        Image img_scratchbutton;
        Image img_dicebutton;
        Image img_wheelbutton;
        Image img_slotsbutton;
        Image img_slotsOut;
        Image img_slotsOut2;

        SpriteAtlas MenuAtlas;
        [NonSerialized]
        public bool canFreeRotateWheel = false;
        protected override void Awake()
        {
            base.Awake();
            img_scratchbutton = btn_Scratch.image;
            img_dicebutton = btn_Dice.image;
            img_wheelbutton = btn_Wheel.image;
            img_slotsbutton = btn_Slots.image;
            img_slotsOut = btn_SlotsOut.image;
            img_slotsOut2 = btn_SlotsOut2.image;
            rect_OutWheel = btn_WheelOut.GetComponent<RectTransform>();
            rect_OutSlots2 = btn_SlotsOut2.transform.parent.GetComponent<RectTransform>();

            btn_Back.onClick.AddListener(OnBackOrSettingButtonClick);
            btn_Wheel.onClick.AddListener(OnWheelButtonClick);
            btn_WheelOut.onClick.AddListener(OnWheelOutButtonClick);
            btn_Scratch.onClick.AddListener(OnScratchButtonClick);
            btn_Dice.onClick.AddListener(OnDiceButtonClick);
            btn_Slots.onClick.AddListener(OnSlotsButtonClick);
            btn_SlotsOut.onClick.AddListener(OnSlotsButtonClick);
            btn_SlotsOut2.onClick.AddListener(OnSlotsButtonClick);
            btn_Gold.onClick.AddListener(OnGoldButtonClick);
            btn_Cash.onClick.AddListener(OnCashButtonClick);
            btn_SpecialToken.onClick.AddListener(OnSpecialButtonClick);
            trans_guidMask.GetComponent<Button>().onClick.AddListener(OnMaskButtonClick);
            btn_RD.onClick.AddListener(OnRDButtonClick);

            float lwr = Screen.height / Screen.width;
            if (lwr > 4 / 3f)
            {
                rect_Top.anchoredPosition = new Vector2(0, -87);
                f_guidY = 600;
            }
            else
                f_guidY = 513;
            upPos = new Vector2(0, Screen.height / 2f + 200);
            downPos = rect_Top.localPosition;
            rightPos = new Vector2(Screen.width / 2f + 250, rect_OutWheel.localPosition.y);
            leftPos = rect_OutWheel.localPosition;
            rightPos2 = new Vector2(Screen.width / 2f + 250, rect_OutSlots2.localPosition.y);
            leftPos2 = rect_OutSlots2.localPosition;

            trans_guidBase.localPosition = new Vector2(0, f_guidY);
            packB = MG_Manager.Instance.Get_Save_PackB();

            MenuAtlas = MG_UIManager.Instance.GetMenuSpriteAtlas();

            sp_diceOff = MenuAtlas.GetSprite("MG_Sprite_Menu_DiceOff");
            sp_diceOn = MenuAtlas.GetSprite("MG_Sprite_Menu_DiceOn");
            sp_scratchOff = MenuAtlas.GetSprite("MG_Sprite_Menu_ScratchOff");
            sp_scratchOn = MenuAtlas.GetSprite("MG_Sprite_Menu_ScratchOn");
            sp_slotsOff = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsOff");
            sp_slotsOn = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsOn");
            sp_wheelOff = MenuAtlas.GetSprite("MG_Sprite_Menu_WheelOff");
            sp_wheelOn = MenuAtlas.GetSprite("MG_Sprite_Menu_WheelOn");
            sp_back = MenuAtlas.GetSprite("MG_Sprite_Menu_Back");
            sp_setting = MenuAtlas.GetSprite("MG_Sprite_Menu_Setting");
            sp_slotsOutA = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsA");
            sp_slotsOutB = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsB");

            sp_ScratchToken = MenuAtlas.GetSprite("MG_Sprite_Menu_ScratchToken");
            sp_SlotsToken = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsToken");
            sp_DiceToken = MenuAtlas.GetSprite("MG_Sprite_Menu_DiceToken");
            img_CashIcon.sprite = MenuAtlas.GetSprite("MG_Sprite_Menu_Cash" + (packB ? "B" : "A"));
            packB = MG_Manager.Instance.Get_Save_PackB();

            go_cashoutTips_cash.SetActive(packB);
            go_cashoutTips_special.SetActive(packB);
            go_cashoutTips_redDiamond.SetActive(packB);

            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.RedDiamond, btn_RD.transform);
            go_scratchRP.SetActive(false);
        }
        Vector2 upPos;
        Vector2 downPos;
        Vector2 leftPos;
        Vector2 rightPos;
        Vector2 leftPos2;
        Vector2 rightPos2;
        Coroutine cor_Menu;
        public void HideOrShowMenuPanel(bool show)
        {
            if (cor_Menu is object)
                StopCoroutine(cor_Menu);
            cor_Menu = StartCoroutine(AutoHideOrShowMenu(show));
            if (show)
            {
                hasPop = false;
                StartCoroutine("CheckCanShowOutWheel");
            }
            else
                StopCoroutine("CheckCanShowOutWheel");
        }
        public void OnGameRewardWheel()
        {
            hasPop = false;
            if (cor_Menu is object)
                StopCoroutine(cor_Menu);
            cor_Menu = StartCoroutine(AutoHideOrShowMenu(true));
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.OutWheel_F_Panel);
            StartCoroutine("CheckCanShowOutWheel");
        }
        IEnumerator AutoHideOrShowMenu(bool show)
        {
            Vector2 targetPos = show ? downPos : upPos;
            Vector2 targetOutWheelPos = show ? leftPos : rightPos;
            Vector2 targetOutSlotsPos2 = show ? leftPos2 : rightPos2;
            Vector2 currentPos = rect_Top.localPosition;
            Vector2 currentOutWheelPos = rect_OutWheel.localPosition;
            Vector2 currentOutSlotsPos2 = rect_OutSlots2.localPosition;
            float finalProgress = 1 - Mathf.Abs((currentPos.y - targetPos.y) / (downPos.y - upPos.y));
            if (finalProgress < 0)
            {
                Debug.LogError("MG Menu Pos is Error.");
                yield break;
            }
            float progress = 1 - Mathf.Pow(1 - finalProgress, 1f / 4);
            while (progress < 1)
            {
                yield return null;
                progress += Time.unscaledDeltaTime * 2;
                finalProgress = -Mathf.Pow(progress - 1, 4f) + 1;
                rect_Top.localPosition = Vector2.Lerp(currentPos, targetPos, finalProgress);
                rect_OutWheel.localPosition = Vector2.Lerp(currentOutWheelPos, targetOutWheelPos, finalProgress);
                rect_OutSlots2.localPosition = Vector2.Lerp(currentOutSlotsPos2, targetOutSlotsPos2, finalProgress);
            }
            yield return null;
            rect_Top.localPosition = targetPos;
            rect_OutWheel.localPosition = targetOutWheelPos;
        }
        IEnumerator CheckCanShowOutWheel()
        {
            while (true)
            {
                yield return null;
                if (freeWheeltimer == 0 && !isInMG&&!hasPop)
                {
                    if (!MG_UIManager.Instance.HasShow_PopPanel(MG_PopPanelType.OutWheel_F_Panel))
                    {
                        canFreeRotateWheel = true;
                        hasPop = true;
                        MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.OutWheelPanel);
                    }
                }
            }
        }
        bool packB = false;
        void OnBackOrSettingButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (isInMG)
            {
                MG_UIManager.Instance.CloseCurrentGamePanel();
                SetMGButtonState(false);
            }
            else
            {
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.SettingPanel);
            }
        }
        public void OnWheelButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (!isInMG)
                SetMGButtonState(true);
            MG_UIManager.Instance.ShowGamePanel(MG_GamePanelType.WheelPanel);
            UpdateBottomButtonState(MG_GamePanelType.WheelPanel);
            SetSpecialToken(MG_SpecialTokenType.WheelToken);
        }
        void OnWheelOutButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.OutWheelPanel);
        }
        public void OnScratchButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (!isInMG)
                SetMGButtonState(true);
            if (go_scratchRP.activeSelf)
                go_scratchRP.SetActive(false);
            MG_UIManager.Instance.ShowGamePanel(MG_GamePanelType.ScratchPanel);
            UpdateBottomButtonState(MG_GamePanelType.ScratchPanel);
            SetSpecialToken(MG_SpecialTokenType.ScratchToken);
        }
        public void OnDiceButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (!isInMG)
                SetMGButtonState(true);
            if (!isInMG)
                SetMGButtonState(true);
            MG_UIManager.Instance.ShowGamePanel(MG_GamePanelType.DicePanel);
            UpdateBottomButtonState(MG_GamePanelType.DicePanel);
            SetSpecialToken(MG_SpecialTokenType.DiceToken);
        }
        public void OnSlotsButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (!isInMG)
                SetMGButtonState(true);
            MG_UIManager.Instance.ShowGamePanel(MG_GamePanelType.SlotsPanel);
            UpdateBottomButtonState(MG_GamePanelType.SlotsPanel);
            SetSpecialToken(MG_SpecialTokenType.SlotsToken);
        }
        void OnGoldButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnCashButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnSpecialButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnRDButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            //OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        public void UpdateAllContent()
        {
            UpdateGoldText();
            UpdateCashText();
            UpdateScratchTicketText();
            UpdateSpecialTokenText();
            UpdateWheelRP();
            UpdateSignRP();
            MG_UIManager.Instance.UpdateSlotsPanel_FruitText();
        }
        Sprite sp_ScratchToken = null;
        Sprite sp_SlotsToken = null;
        Sprite sp_DiceToken = null;
        void SetSpecialToken(MG_SpecialTokenType _SpecialTokenType)
        {
            switch (_SpecialTokenType)
            {
                case MG_SpecialTokenType.ScratchToken:
                    if(!packB)
                        go_SpecialToken.SetActive(false);
                    else
                    {
                        go_SpecialToken.SetActive(true);
                        img_SpecialToken.sprite = sp_ScratchToken;
                        text_SpecialToken.text = MG_Manager.Instance.Get_Save_CurrentSSS().ToString();
                    }
                    break;
                case MG_SpecialTokenType.SlotsToken:
                    img_SpecialToken.sprite = sp_SlotsToken;
                    text_SpecialToken.text = MG_Manager.Instance.Get_Save_CurrentDiamond().ToString();
                    go_SpecialToken.SetActive(true);
                    break;
                case MG_SpecialTokenType.WheelToken:
                case MG_SpecialTokenType.DiceToken:
                    if (!packB)
                        go_SpecialToken.SetActive(false);
                    else
                    {
                        go_SpecialToken.SetActive(true);
                        img_SpecialToken.sprite = sp_DiceToken;
                        text_SpecialToken.text = MG_Manager.Instance.Get_Save_CurrentStarCoin().ToString();
                    }
                    break;
                case MG_SpecialTokenType.Null:
                    break;
            }
        }
        public void FlyToTarget(Vector3 startPos,MG_MenuFlyTarget flyTarget,int num)
        {
            MG_Manager.Instance.MG_Fly.FlyToTarget(startPos, GetFlyTargetPos(flyTarget), num, flyTarget, FlyToTargetCallback);
        }
        public void UpdateCashText()
        {
            text_Cash.text = MG_Manager.Get_CashShowText(MG_Manager.Instance.Get_Save_CurrentCash());
        }
        public void UpdateGoldText()
        {
            text_Gold.text = MG_Manager.Instance.Get_Save_Gold().ToString();
        }
        public void UpdateScratchTicketText()
        {
            text_ScratchTicketNum.text = MG_Manager.Instance.Get_Save_ScratchTicket().ToString();
        }
        public void UpdateWheelRP()
        {
            go_wheelRP.SetActive(MG_Manager.Instance.Get_Save_WheelTickets() > 0);
        }
        public void UpdateSignRP()
        {
            go_signRP.SetActive(MG_Manager.Instance.Get_Save_WetherSign());
        }
        public void UpdateSpecialTokenText()
        {
            int panelIndex = MG_SaveManager.Current_GamePanel;
            if (panelIndex == (int)MG_GamePanelType.ScratchPanel)
            {
                SetSpecialToken(MG_SpecialTokenType.ScratchToken);
            }
            else if (panelIndex == (int)MG_GamePanelType.DicePanel || panelIndex == (int)MG_GamePanelType.WheelPanel)
            {
                SetSpecialToken(MG_SpecialTokenType.DiceToken);
            }
            else if (panelIndex == (int)MG_GamePanelType.SlotsPanel)
            {
                SetSpecialToken(MG_SpecialTokenType.SlotsToken);
            }
        }
        public void UpdateRedDiamondText()
        {
            text_rdNum.text = MG_Manager.Instance.Get_Save_RedDiamond().ToString();
        }
        void UpdateBottomButtonState(MG_GamePanelType clickbuttonType)
        {
            switch (clickbuttonType)
            {
                case MG_GamePanelType.DicePanel:
                    img_scratchbutton.sprite = sp_scratchOff;
                    img_dicebutton.sprite = sp_diceOn;
                    img_slotsbutton.sprite = sp_slotsOff;
                    img_wheelbutton.sprite = sp_wheelOff;
                    break;
                case MG_GamePanelType.ScratchPanel:
                    img_scratchbutton.sprite = sp_scratchOn;
                    img_dicebutton.sprite = sp_diceOff;
                    img_slotsbutton.sprite = sp_slotsOff;
                    img_wheelbutton.sprite = sp_wheelOff;
                    break;
                case MG_GamePanelType.SlotsPanel:
                    img_scratchbutton.sprite = sp_scratchOff;
                    img_dicebutton.sprite = sp_diceOff;
                    img_slotsbutton.sprite = sp_slotsOn;
                    img_wheelbutton.sprite = sp_wheelOff;
                    break;
                case MG_GamePanelType.WheelPanel:
                    img_scratchbutton.sprite = sp_scratchOff;
                    img_dicebutton.sprite = sp_diceOff;
                    img_slotsbutton.sprite = sp_slotsOff;
                    img_wheelbutton.sprite = sp_wheelOn;
                    break;
            }
        }
        public readonly Dictionary<int, Transform> dic_flytarget_transform = new Dictionary<int, Transform>();
        Vector3 GetFlyTargetPos(MG_MenuFlyTarget _flyTarget)
        {
            if(dic_flytarget_transform.TryGetValue((int)_flyTarget,out Transform trans_Target))
            {
                return trans_Target.position;
            }
            return Vector3.zero;
        }
        void FlyToTargetCallback(MG_MenuFlyTarget _flyTarget)
        {
            switch (_flyTarget)
            {
                case MG_MenuFlyTarget.WheelTicket:
                    MG_UIManager.Instance.UpdateWheelTicketText();
                    return;
                case MG_MenuFlyTarget.Orange:
                case MG_MenuFlyTarget.Cherry:
                case MG_MenuFlyTarget.Watermalen:
                    MG_UIManager.Instance.UpdateSlotsPanel_FruitText();
                    return;
            }
            StopCoroutine("ExpandTarget");
            StartCoroutine("ExpandTarget", _flyTarget);
        }
        IEnumerator ExpandTarget(MG_MenuFlyTarget _flyTarget)
        {
            if (!dic_flytarget_transform.TryGetValue((int)_flyTarget, out Transform tempTrans))
                yield break;
            bool toBiger = true;
            while (true)
            {
                yield return null;
                if (toBiger)
                {
                    if (tempTrans.localScale.x >= 1.3f)
                    {
                        toBiger = false;
                        if (isInMG)
                            switch (_flyTarget)
                            {
                                case MG_MenuFlyTarget.OneGold:
                                    UpdateGoldText();
                                    break;
                                case MG_MenuFlyTarget.Cash:
                                    UpdateCashText();
                                    break;
                                case MG_MenuFlyTarget.Scratch:
                                case MG_MenuFlyTarget.ScratchTicket:
                                    MG_UIManager.Instance.Update_ScratchTicketText();
                                    break;
                                case MG_MenuFlyTarget.RedDiamond:
                                    UpdateRedDiamondText();
                                    break;
                                default:
                                    UpdateSpecialTokenText();
                                    break;
                            }
                        else
                        {
                            if (_flyTarget == MG_MenuFlyTarget.RedDiamond)
                                UpdateRedDiamondText();
                        }
                    }
                    else
                    {
                        if (Time.unscaledDeltaTime > 0.04f)
                            tempTrans.localScale += Vector3.one * 0.02f * 3;
                        else
                            tempTrans.localScale += Vector3.one * Time.unscaledDeltaTime * 3;
                    }
                }
                else
                {
                    if (tempTrans.localScale.x <= 1f)
                        break;
                    else
                    {
                        if (Time.unscaledDeltaTime > 0.04f)
                            tempTrans.localScale -= Vector3.one * 0.02f * 3;
                        else
                            tempTrans.localScale -= Vector3.one * Time.unscaledDeltaTime * 3;
                    }
                }
            }
            if (_flyTarget == MG_MenuFlyTarget.ScratchTicket && MG_SaveManager.Current_GamePanel != (int)MG_GamePanelType.ScratchPanel)
            {
                if (!go_scratchRP.activeSelf)
                    go_scratchRP.SetActive(true);
            }
            yield return null;
            tempTrans.localScale = Vector3.one;
        }
        public override IEnumerator OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            if (MG_SaveManager.FirstCome)
            {
                if (MG_Manager.Instance.NeedFirstComeReward)
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
            }
            SetMGButtonState(false);
            StartCoroutine(OutWheelTimeDown());
            StartCoroutine(AutoRotateOutWheelBase());
            HideOrShowMenuPanel(true);
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            yield return null;
        }

        public override void OnPause()
        {

        }

        public override void OnResume()
        {
        }
        public void CheckGuid()
        {
            if (MG_Manager.Instance.next_GuidType != MG_Guid_Type.Null && MG_Manager.Instance.NeedForceCashoutGuid)
            {
                MG_Manager.Instance.isGuid = true;
                if (!MG_Manager.Instance.Get_Save_PackB())
                {
                    MG_Manager.Instance.isGuid = false;
                    return;
                }
                switch (MG_Manager.Instance.next_GuidType)
                {
                    case MG_Guid_Type.DiceGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_Cash.transform.SetAsLastSibling();
                        trans_guidBase.localScale = new Vector3(-1, 1, 1);
                        text_guidDes.transform.localScale = new Vector3(-1, 1, 1);
                        trans_guidDown.transform.localScale = new Vector3(-1, 1, 1);
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_GuidDollar_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_BlueBgSP_name);
                        text_guidDes.text = str_guidCashout;
                        MG_SaveManager.GuidDice = true;
                        StartCoroutine(WaitForClickDiceGuid());
                        break;
                    case MG_Guid_Type.ScratchGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_SpecialToken.transform.SetAsLastSibling();
                        trans_guidBase.localScale = Vector3.one;
                        text_guidDes.transform.localScale = Vector3.one;
                        trans_guidDown.transform.localScale = Vector3.one;
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_GuidSSS_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_BlueBgSP_name);
                        text_guidDes.text = str_guid7;
                        MG_SaveManager.GuidScratch = true;
                        StartCoroutine(WaitForClickScratchGuid());
                        break;
                    case MG_Guid_Type.SlotsGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_SpecialToken.transform.SetAsLastSibling();
                        trans_guidBase.localScale = Vector3.one;
                        text_guidDes.transform.localScale = Vector3.one;
                        trans_guidDown.transform.localScale = Vector3.one;
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_GuidDiamond_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_GreenBgSP_name);
                        text_guidDes.text = str_guidDimond;
                        MG_SaveManager.GuidSlots = true;
                        StartCoroutine(WaitForClickScratchGuid());
                        break;
                }
            }
        }
        const string str_guidCashout = "<size=70><color=#FFF408>You'have won bonus!</color></size>\nOnce you get up to specified amount\nyou can redeem red diamond.";
        float f_guidY = 0;
        const string str_guidStarcoin = "<size=70><color=#FFF408>Win the big prize</color></size>\nOnce you meet the requirements,\nyou can get a huge bonus.";
        const string str_guid7 = "<size=70><color=#FFF408>Lucky Seven Day</color></size>\n Collect lucky 7 to redeem\nred diamond rewards.";
        const string str_guidDimond = "<size=70><color=#FFF408>Redeem gifts</color></size>\nYou can use coins to redeem\nred diamond!";

        const string str_BlueBgSP_name = "MG_Sprite_Menu_Guid_B";
        const string str_GreenBgSP_name = "MG_Sprite_Menu_Guid_G";
        const string str_OrangeBgSP_name = "MG_Sprite_Menu_Guid_O";
        const string str_GuidDiamond_name = "MG_Sprite_Menu_GuidDiamond";
        const string str_GuidDollar_name = "MG_Sprite_Menu_GuidDollar";
        const string str_GuidStarcoin_name = "MG_Sprite_Menu_GuidStarcoin";
        const string str_GuidSSS_name = "MG_Sprite_Menu_GuidSSS";
        bool hasClickGuid = false;
        bool canClickButton = false;
        IEnumerator WaitForClickDiceGuid()
        {
            hasClickGuid = false;
            canClickButton = false;
            yield return MG_Manager.WaitForSeconds(1);
            canClickButton = true;
            while (true)
            {
                if (hasClickGuid)
                    break;
                yield return null;
            }
            trans_guidMask.SetAsLastSibling();
            btn_SpecialToken.transform.SetAsLastSibling();
            trans_guidBase.localScale = Vector3.one;
            text_guidDes.transform.localScale = Vector3.one;
            trans_guidDown.transform.localScale = Vector3.one;
            img_guidIcon.sprite = MenuAtlas.GetSprite(str_GuidStarcoin_name);
            img_guidBG.sprite = MenuAtlas.GetSprite(str_OrangeBgSP_name);
            text_guidDes.text = str_guidStarcoin;
            hasClickGuid = false;
            canClickButton = false;
            yield return MG_Manager.WaitForSeconds(1);
            canClickButton = true;
            while (true)
            {
                yield return null;
                if (hasClickGuid)
                    break;
            }
            trans_guidMask.SetParent(transform);
            trans_guidMask.gameObject.SetActive(false);
            MG_Manager.Instance.isGuid = false;
            MG_Manager.Instance.next_GuidType = MG_Guid_Type.Null;
        }
        IEnumerator WaitForClickScratchGuid()
        {
            hasClickGuid = false;
            canClickButton = false;
            yield return MG_Manager.WaitForSeconds(1);
            canClickButton = true;
            while (true)
            {
                if (hasClickGuid)
                    break;
                yield return null;
            }
            trans_guidMask.SetParent(transform);
            trans_guidMask.gameObject.SetActive(false);
            MG_Manager.Instance.isGuid = false;
            MG_Manager.Instance.next_GuidType = MG_Guid_Type.Null;
        }
        void OnMaskButtonClick()
        {
            if (canClickButton)
                hasClickGuid = true;
        }
        [NonSerialized]
        public bool isInMG = false;
        void SetMGButtonState(bool show)
        {
            isInMG = show;
            if (show)
            {
                UpdateAllContent();
                StopCoroutine("SlotsOutAnimation");

                int goldIndex = (int)MG_MenuFlyTarget.OneGold;
                if (dic_flytarget_transform.ContainsKey(goldIndex))
                    dic_flytarget_transform[goldIndex] = btn_Gold.transform;
                else
                    dic_flytarget_transform.Add(goldIndex, btn_Gold.transform);

                int cashIndex = (int)MG_MenuFlyTarget.Cash;
                if (dic_flytarget_transform.ContainsKey(cashIndex))
                    dic_flytarget_transform[cashIndex] = btn_Cash.transform;
                else
                    dic_flytarget_transform.Add(cashIndex, btn_Cash.transform);

                int starcoinIndex = (int)MG_MenuFlyTarget.StarCoin;
                if (dic_flytarget_transform.ContainsKey(starcoinIndex))
                    dic_flytarget_transform[starcoinIndex] = btn_SpecialToken.transform;
                else
                    dic_flytarget_transform.Add(starcoinIndex, btn_SpecialToken.transform);
                
                int scratchTicketIndex = (int)MG_MenuFlyTarget.ScratchTicket;
                if (dic_flytarget_transform.ContainsKey(scratchTicketIndex))
                    dic_flytarget_transform[scratchTicketIndex] = btn_Scratch.transform;
                else
                    dic_flytarget_transform.Add(scratchTicketIndex, btn_Scratch.transform);
                
                int scratchIndex = (int)MG_MenuFlyTarget.Scratch;
                if (dic_flytarget_transform.ContainsKey(scratchIndex))
                    dic_flytarget_transform[scratchIndex] = btn_Scratch.transform;
                else
                    dic_flytarget_transform.Add(scratchIndex, btn_Scratch.transform);
                
                int sssIndex = (int)MG_MenuFlyTarget.SSS;
                if (dic_flytarget_transform.ContainsKey(sssIndex))
                    dic_flytarget_transform[sssIndex] = btn_SpecialToken.transform;
                else
                    dic_flytarget_transform.Add(sssIndex, btn_SpecialToken.transform);
                
                int diamondIndex = (int)MG_MenuFlyTarget.Diamond;
                if (dic_flytarget_transform.ContainsKey(diamondIndex))
                    dic_flytarget_transform[diamondIndex] = btn_SpecialToken.transform;
                else
                    dic_flytarget_transform.Add(diamondIndex, btn_SpecialToken.transform);
            }
            else
            {
                UpdateRedDiamondText();
                StartCoroutine("SlotsOutAnimation");

                int goldIndex = (int)MG_MenuFlyTarget.OneGold;
                if (dic_flytarget_transform.ContainsKey(goldIndex))
                    dic_flytarget_transform[goldIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(goldIndex, btn_SlotsOut.transform);

                int cashIndex = (int)MG_MenuFlyTarget.Cash;
                if (dic_flytarget_transform.ContainsKey(cashIndex))
                    dic_flytarget_transform[cashIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(cashIndex, btn_SlotsOut.transform);

                int starcoinIndex = (int)MG_MenuFlyTarget.StarCoin;
                if (dic_flytarget_transform.ContainsKey(starcoinIndex))
                    dic_flytarget_transform[starcoinIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(starcoinIndex, btn_SlotsOut.transform);

                int scratchTicketIndex = (int)MG_MenuFlyTarget.ScratchTicket;
                if (dic_flytarget_transform.ContainsKey(scratchTicketIndex))
                    dic_flytarget_transform[scratchTicketIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(scratchTicketIndex, btn_SlotsOut.transform);

                int scratchIndex = (int)MG_MenuFlyTarget.Scratch;
                if (dic_flytarget_transform.ContainsKey(scratchIndex))
                    dic_flytarget_transform[scratchIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(scratchIndex, btn_SlotsOut.transform);

                int sssIndex = (int)MG_MenuFlyTarget.SSS;
                if (dic_flytarget_transform.ContainsKey(sssIndex))
                    dic_flytarget_transform[sssIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(sssIndex, btn_SlotsOut.transform);

                int diamondIndex = (int)MG_MenuFlyTarget.Diamond;
                if (dic_flytarget_transform.ContainsKey(diamondIndex))
                    dic_flytarget_transform[diamondIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(diamondIndex, btn_SlotsOut.transform);

                int wheelTicketIndex = (int)MG_MenuFlyTarget.WheelTicket;
                if (dic_flytarget_transform.ContainsKey(wheelTicketIndex))
                    dic_flytarget_transform[wheelTicketIndex] = btn_SlotsOut.transform;
                else
                    dic_flytarget_transform.Add(wheelTicketIndex, btn_SlotsOut.transform);
            }
            go_bottom.SetActive(show);
            btn_Cash.gameObject.SetActive(show);
            btn_Gold.gameObject.SetActive(show);
            btn_SpecialToken.gameObject.SetActive(show);
            btn_RD.gameObject.SetActive(!show);
            btn_SlotsOut.gameObject.SetActive(!show);
            btn_WheelOut.gameObject.SetActive(!show);
            btn_SlotsOut2.transform.parent.gameObject.SetActive(!show);
            btn_Back.image.sprite = show ? sp_back : sp_setting;
            MG_Manager.Instance.SetBGState(show);
            //SoundManager._Instance.ChangeBGM(show);
        }
        bool slotsOutisA = true;
        const float timeInterval = 0.4f;
        IEnumerator SlotsOutAnimation()
        {
            float timer = 0;
            while (true)
            {
                yield return null;
                timer += Time.unscaledDeltaTime;
                if (timer >= timeInterval)
                {
                    timer -= timeInterval;
                    slotsOutisA = !slotsOutisA;
                    img_slotsOut.sprite = slotsOutisA ? sp_slotsOutA : sp_slotsOutB;
                    img_slotsOut2.sprite = slotsOutisA ? sp_slotsOutA : sp_slotsOutB;
                }
            }
        }
#if UNITY_EDITOR
        const float freeWheelTime =20;
#else
        const float freeWheelTime =120;
#endif
        [NonSerialized]
        public float freeWheeltimer = 0;
        bool hasPop = false;
        IEnumerator OutWheelTimeDown()
        {
            freeWheeltimer = freeWheelTime;
            while (true)
            {
                yield return null;
                freeWheeltimer -= Time.unscaledDeltaTime;
                if (freeWheeltimer < 0)
                {
                    freeWheeltimer = 0;
                    if (text_wheelOutTime.transform.parent.gameObject.activeSelf)
                        text_wheelOutTime.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    if (!text_wheelOutTime.transform.parent.gameObject.activeSelf)
                        text_wheelOutTime.transform.parent.gameObject.SetActive(true);
                    int minutes = (int)(freeWheeltimer / 60);
                    int seconds = (int)(freeWheeltimer % 60);
                    text_wheelOutTime.text = (minutes > 9 ? minutes.ToString() : "0" + minutes) + ":" + (seconds > 9 ? seconds.ToString() : "0" + seconds);
                }
            }
        }
        public void ResetTimer()
        {
            if (freeWheeltimer == 0)
                freeWheeltimer = freeWheelTime;
            canFreeRotateWheel = false;
            hasPop = false;
        }
        IEnumerator AutoRotateOutWheelBase()
        {
            while (true)
            {
                yield return null;
                rect_OutWheelBase.Rotate(-Vector3.forward * Time.unscaledDeltaTime * 200);
            }
        }
    }
}
