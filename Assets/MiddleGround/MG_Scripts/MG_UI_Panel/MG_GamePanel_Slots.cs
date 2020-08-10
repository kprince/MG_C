using MiddleGround.GameConfig;
using MiddleGround.Save;
using MiddleGround.UI.ButtonAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_GamePanel_Slots : MG_UIBase
    {
        public Button btn_Spin;
        public Button btn_addMutiple;
        public Button btn_Fruits;

        public Image img_L;
        public Image img_M;
        public Image img_R;
        public Image img_Light;
        public Image img_ButtonText;
        public Text text_FruitNum;
        public GameObject go_lock;
        public Transform trans_spin;
        public MG_Slots_ButtonAnimation _ButtonAnimation;

        Sprite sp_LightA;
        Sprite sp_LightB;
        Sprite sp_spin;
        Sprite sp_adSpin;
        Sprite sp_adSpeedup;

        SpriteAtlas slotsSA;

        public Text text_SpinGoldNum;
        public Text text_X10;
        public Text text_Locktime;
        static readonly Dictionary<int, float> dic_type_offsetY = new Dictionary<int, float>()
        {
            { (int)MG_Slots_RewardType.Gift,0.065f },
            { (int)MG_Slots_RewardType.Cash,0.19f },
            { (int)MG_Slots_RewardType.Orange,0.31f },
            { (int)MG_Slots_RewardType.Diamond,0.44f },
            { (int)MG_Slots_RewardType.Cherry,0.56f },
            { (int)MG_Slots_RewardType.Gold,0.69f },
            { (int)MG_Slots_RewardType.Watermalen,0.82f },
            { (int)MG_Slots_RewardType.SSS,0.94f }
        };

        const string mat_mainTex_Key = "_MainTex";
        float finalOffsetX = 0.25f;
        protected override void Awake()
        {
            base.Awake();

            btn_Spin.onClick.AddListener(OnSpinButtonClick);
            btn_addMutiple.onClick.AddListener(OnX10ButtonClick);
            btn_Fruits.onClick.AddListener(() => { if (MG_Manager.Instance.canChangeGame) MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel); });
            MG_UIManager.Instance.MenuPanel.dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Cherry, text_FruitNum.transform);
            MG_UIManager.Instance.MenuPanel.dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Orange, text_FruitNum.transform);
            MG_UIManager.Instance.MenuPanel.dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Watermalen, text_FruitNum.transform);
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            mutiplesIndex = 0;
            text_X10.text = "x" + mutiples[mutiplesIndex];
            finalOffsetX = 0;
            slotsSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_GamePanelType.SlotsPanel);
            sp_adSpeedup = slotsSA.GetSprite("MG_Sprite_Slots_Speedup");
            sp_adSpin = slotsSA.GetSprite("MG_Sprite_Slots_AdSpin");
            sp_spin = slotsSA.GetSprite("MG_Sprite_Slots_Spin");
            sp_LightA = slotsSA.GetSprite("MG_Sprite_Slots_LightA");
            sp_LightB = slotsSA.GetSprite("MG_Sprite_Slots_LightB");
            _ButtonAnimation.Init(() => { img_ButtonText.transform.localPosition = new Vector2(0, -11); }, () => { img_ButtonText.transform.localPosition = new Vector2(0, 15); });
        }
        int clickTime = 0;
        void OnSpinButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isSpining) return;
            if (isLocked)
            {
                clickTime++;
                MG_Manager.ShowRV(OnUnlockAdCallback, clickTime,"slots unlock");
                return;
            }
            if (needAd)
            {
                clickTime++;
                MG_Manager.ShowRV(OnNoGoldAdCallback, clickTime,"slots adSpin");
                return;
            }
            isSpining = true;
            MG_SaveManager.SlotsTotalPlayTimes++;
            rewardType = MG_Manager.Instance.Random_SlotsReward(baseNum * mutiples[mutiplesIndex], out rewardNum);
            MG_Manager.Instance.Add_Save_Gold(-baseNum * mutiples[mutiplesIndex]);
            StartCoroutine("StartSpin");
        }
        void OnNoGoldAdCallback()
        {
            clickTime = 0;
            isSpining = true;
            MG_SaveManager.SlotsTotalPlayTimes++;
            rewardType = MG_Manager.Instance.Random_SlotsReward(baseNum * mutiples[mutiplesIndex], out rewardNum);
            UpdateSpinButtonState(MG_Manager.Instance.Get_Save_Gold());
            StartCoroutine("StartSpin");
        }
        void OnUnlockAdCallback()
        {
            clickTime = 0;
            isLocked = false;
            MG_SaveManager.SlotsLockDate = System.DateTime.Now.AddSeconds(-3601);
            CheckIsLock();
            UpdateSpinButtonState(MG_Manager.Instance.Get_Save_Gold());
        }
        readonly int[] mutiples = new int[6] { 1, 5, 10, 20, 50, 100 };
        int mutiplesIndex = 0;
        void OnX10ButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (mutiplesIndex < mutiples.Length - 1 && MG_Manager.Instance.Get_Save_Gold() >= baseNum * mutiples[mutiplesIndex + 1])
            {
                mutiplesIndex++;
                if (mutiplesIndex > mutiples.Length - 1)
                    mutiplesIndex = mutiples.Length - 1;
            }
            else
            {
                mutiplesIndex = 0;
            }
            text_X10.text = "x" + mutiples[mutiplesIndex];
        }
        const int baseNum = 1000;
        bool isSpining = false;
        int rewardNum = 0;
        MG_Slots_RewardType rewardType = MG_Slots_RewardType.Null;
        IEnumerator StartSpin()
        {
            MG_Manager.Instance.SendAdjustSlotsEvent();
            MG_Manager.Instance.canChangeGame = false;
            Material mt_L = img_L.material;
            Material mt_M = img_M.material;
            Material mt_R = img_R.materialForRendering;

            float endOffsetY_L = 0;
            float endOffsetY_M = 0;
            float endOffsetY_R = 0;
            if (rewardType == MG_Slots_RewardType.Null)
            {
                int count = dic_type_offsetY.Count;
                int random_L_Index = Random.Range(0, count);
                int random_M_Index = Random.Range(0, count);
                int random_R_Index;
                if (random_L_Index == random_M_Index)
                {
                    do { random_R_Index = Random.Range(0, count); }
                    while (random_R_Index == random_L_Index);
                }
                else
                    random_R_Index = Random.Range(0, count);
                int indexOrder = 0;
                foreach (float y in dic_type_offsetY.Values)
                {
                    if (indexOrder == random_L_Index)
                        endOffsetY_L = y;
                    if (indexOrder == random_M_Index)
                        endOffsetY_M = y;
                    if (indexOrder == random_R_Index)
                        endOffsetY_R = y;
                    indexOrder++;
                }
            }
            else if (rewardType == MG_Slots_RewardType.SS_Other)
            {
                endOffsetY_L = endOffsetY_M = dic_type_offsetY[(int)MG_Slots_RewardType.SSS];
                int count = dic_type_offsetY.Count;
                int random_R_Index;
                do
                {
                    random_R_Index = Random.Range(0, count);
                    int index = 0;
                    foreach(float y in dic_type_offsetY.Values)
                    {
                        if (index == random_R_Index)
                        {
                            endOffsetY_R = y;
                            break;
                        }
                        index++;
                    }
                }
                while (endOffsetY_R == endOffsetY_L);
            }
            else
            {
                endOffsetY_L = endOffsetY_M = endOffsetY_R = dic_type_offsetY[(int)rewardType];
            }

            float spinTime_L = 2;
            float spinTime_M = 3f;
            float spinTime_R = 4;
            float timer = 0;
            float spinSpeed;
            float backSpeed_L = 0.005f;
            float backSpeed_M = 0.005f;
            float backSpeed_R = 0.005f;
            float backTimer_L = 0;
            float backTimer_M = 0;
            float backTimer_R = 0;
            float startOffsetY_L = mt_L.GetTextureOffset(mat_mainTex_Key).y;
            float startOffsetY_M = mt_M.GetTextureOffset(mat_mainTex_Key).y;
            float startOffsetY_R = mt_R.GetTextureOffset(mat_mainTex_Key).y;
            bool stop_L = false;
            bool back_L = false;
            bool stop_M = false;
            bool back_M = false;
            bool stop_R = false;
            bool back_R = false;
            StartCoroutine("AutoShiningLight");
            AudioSource as_Spin = MG_Manager.Play_SpinSlots();
            while (!stop_R || !stop_M || !stop_L)
            {
                yield return null;
                timer += Time.unscaledDeltaTime*2;
                spinSpeed = Time.unscaledDeltaTime * 2.6f;
                startOffsetY_L += spinSpeed;
                startOffsetY_M += spinSpeed;
                startOffsetY_R += spinSpeed;
                if (!stop_L)
                    if (timer < spinTime_L)
                        mt_L.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, startOffsetY_L));
                    else
                    {
                        if (!back_L)
                        {
                            backSpeed_L -= 0.0005f;
                            backTimer_L += backSpeed_L;
                            mt_L.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_L + backTimer_L));
                            if (backSpeed_L <= 0)
                                back_L = true;
                        }
                        else
                        {
                            backSpeed_L -= 0.002f;
                            backTimer_L += backSpeed_L;
                            mt_L.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_L + backTimer_L));
                            if (backTimer_L <= 0)
                            {
                                mt_L.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_L));
                                stop_L = true;
                            }
                        }
                    }
                if (!stop_M)
                    if (timer < spinTime_M)
                        mt_M.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, startOffsetY_M));
                    else
                    {
                        if (!back_M)
                        {
                            backSpeed_M -= 0.0005f;
                            backTimer_M += backSpeed_M;
                            mt_M.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_M + backTimer_M));
                            if (backSpeed_M <= 0)
                                back_M = true;
                        }
                        else
                        {
                            backSpeed_M -= 0.002f;
                            backTimer_M += backSpeed_M;
                            mt_M.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_M + backTimer_M));
                            if (backTimer_M <= 0)
                            {
                                mt_M.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_M));
                                stop_M = true;
                            }
                        }
                    }
                if (!stop_R)
                    if (timer < spinTime_R)
                        mt_R.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, startOffsetY_R));
                    else
                    {
                        if (!back_R)
                        {
                            backSpeed_R -= 0.0005f;
                            backTimer_R += backSpeed_R;
                            mt_R.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_R + backTimer_R));
                            if (backSpeed_R <= 0)
                                back_R = true;
                        }
                        else
                        {
                            backSpeed_R -= 0.002f;
                            backTimer_R += backSpeed_R;
                            mt_R.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_R + backTimer_R));
                            if (backTimer_R <= 0)
                            {
                                mt_R.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, endOffsetY_R));
                                stop_R = true;
                            }
                        }
                    }
            }
            as_Spin.Stop();
            yield return MG_Manager.WaitForSeconds(0.5f);
            StopCoroutine("AutoShiningLight");
            switch (rewardType)
            {
                case MG_Slots_RewardType.Cash:
                    MG_Manager.Instance.Show_CashRewardPanel(MG_RewardPanelType.AdClaim, rewardNum);
                    break;
                case MG_Slots_RewardType.Diamond:
                    rewardNum -= rewardNum % 10;
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Diamond, rewardNum);
                    break;
                case MG_Slots_RewardType.Gold:
                    rewardNum -= rewardNum % 10;
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.Gold, rewardNum);
                    break;
                case MG_Slots_RewardType.Cherry:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Cherry, rewardNum);
                    break;
                case MG_Slots_RewardType.Orange:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Orange, rewardNum);
                    break;
                case MG_Slots_RewardType.Watermalen:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Watermalen, rewardNum);
                    break;
                case MG_Slots_RewardType.Gift:
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                    break;
                default:
                    break;
            }
            yield return null;
            CheckIsLock();
            UpdateSpinButtonState(MG_SaveManager.Gold);
            isSpining = false;
            MG_Manager.Instance.canChangeGame = true;
        }
        bool spinAnimationPlaying = false;
        public override IEnumerator OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            img_L.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_type_offsetY[(int)MG_Slots_RewardType.SSS]));
            img_M.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_type_offsetY[(int)MG_Slots_RewardType.SSS]));
            img_R.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_type_offsetY[(int)MG_Slots_RewardType.SSS]));
            CheckIsLock();
            UpdateSpinButtonState(MG_SaveManager.Gold);
            UpdateFruitNumText();
            yield return null;
            clickTime = 0;
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
        bool needAd = false;
        public void UpdateSpinButtonState(int gold)
        {
            if (isLocked) return;
            if (gold >= baseNum * mutiples[mutiplesIndex])
            {
                text_X10.text = "x" + mutiples[mutiplesIndex];
            }
            else
            {
                mutiplesIndex = 0;
                text_X10.text = "x" + mutiples[0];
            }
            if (gold >= baseNum * mutiples[0])
            {
                img_ButtonText.sprite = sp_spin;
                needAd = false;
            }
            else
            {
                img_ButtonText.sprite = sp_adSpin;
                needAd = true;
            }
        }
        public void UpdateFruitNumText()
        {
            text_FruitNum.text = MG_Manager.Instance.Get_Save_CurrentFruits().ToString();
        }
        IEnumerator AutoShiningLight()
        {
            bool isA = false;
            while (true)
            {
                yield return MG_Manager.WaitForSeconds(0.1f);
                isA = !isA;
                img_Light.sprite = isA ? sp_LightA : sp_LightB;
            }
        }
        bool isLocked = false;
        void CheckIsLock()
        {
            if (MG_SaveManager.SlotsTotalPlayTimes > 0 && MG_SaveManager.SlotsTotalPlayTimes % 5 == 0)
            {
                if (MG_SaveManager.SlotsTotalPlayTimes > MG_SaveManager.SlotsLockPlayTimeIndex)
                {
                    isLocked = true;
                    MG_SaveManager.SlotsLockPlayTimeIndex = MG_SaveManager.SlotsTotalPlayTimes;
                    MG_SaveManager.SlotsLockDate = System.DateTime.Now;
                    StopCoroutine("WaitForUnlock");
                    StartCoroutine("WaitForUnlock", 3599);
                }
                else if (MG_SaveManager.SlotsTotalPlayTimes == MG_SaveManager.SlotsLockPlayTimeIndex)
                {
                    System.DateTime now = System.DateTime.Now;
                    System.TimeSpan interval = now - MG_SaveManager.SlotsLockDate;
                    if (interval.TotalSeconds < 3600)
                    {
                        isLocked = true;
                        StopCoroutine("WaitForUnlock");
                        StartCoroutine("WaitForUnlock", 3599 - interval.TotalSeconds);
                    }
                    else
                        isLocked = false;
                }
            }
            else
            {
                isLocked = false;
            }
            if (isLocked)
            {
                if (!go_lock.activeSelf)
                    go_lock.SetActive(true);
                img_ButtonText.sprite = sp_adSpeedup;
            }
            else
            {
                if (go_lock.activeSelf)
                    go_lock.SetActive(false);
                img_ButtonText.sprite = sp_spin;
            }
        }
        IEnumerator WaitForUnlock(int seconds)
        {
            if (seconds == 0)
            {
                CheckIsLock();
                yield break;
            }
            int showHours = seconds / 3600;
            int showMinutes = (seconds % 3600) / 60;
            int showSeconds = seconds % 60;
            text_Locktime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            while (seconds > 0)
            {
                yield return MG_Manager.WaitForSeconds(1);
                seconds--;
                if (seconds == 0)
                {
                    CheckIsLock();
                    yield break;
                }
                showHours = seconds / 3600;
                showMinutes = (seconds % 3600) / 60;
                showSeconds = seconds % 60;
                text_Locktime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            }
        }
    }
}
