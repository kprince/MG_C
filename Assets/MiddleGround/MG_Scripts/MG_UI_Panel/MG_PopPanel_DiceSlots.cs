using MiddleGround.UI.ButtonAnimation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_DiceSlots : MG_UIBase
    {
        public Button btn_Spin;
        public Image img_buttonText;
        public Image img_L;
        public Image img_M;
        public Image img_R;
        public Image img_Light;
        public Transform trans_spinbutton;
        public MG_Slots_ButtonAnimation _ButtonAnimation;
        Sprite sp_spin;
        Sprite sp_adAgain;
        Sprite sp_LightA;
        Sprite sp_LightB;
        static readonly Dictionary<string, float> dic_name_offsetY = new Dictionary<string, float>()
        {
            {"Cash5",0.848f },
            {"Gold5000",0.748f },
            {"Cash100",0.648f },
            {"Gold10000",0.548f },
            {"Cash50",0.448f },
            {"Gold1000",0.348f },
            {"Cash10",0.248f },
            {"Gold500",0.148f },
            {"Cash25",0.048f },
            {"7",0.948f}
        };
        const string mat_mainTex_Key = "_MainTex";
        float finalOffsetX = 0.25f;
        SpriteAtlas slotsSA;
        protected override void Awake()
        {
            base.Awake();
            btn_Spin.onClick.AddListener(OnSpinButtonClick);
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            finalOffsetX = 0;
            slotsSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_GamePanelType.SlotsPanel);
            sp_spin = slotsSA.GetSprite("MG_Sprite_Slots_Spin");
            sp_adAgain = slotsSA.GetSprite("MG_Sprite_Slots_Again");
            sp_LightA = slotsSA.GetSprite("MG_Sprite_Slots_LightA");
            sp_LightB = slotsSA.GetSprite("MG_Sprite_Slots_LightB");
            text_nothanks.GetComponent<Button>().onClick.AddListener(OnNothanksClick);
            _ButtonAnimation.Init(() => { img_buttonText.transform.localPosition = new Vector2(0, -11); }, () => { img_buttonText.transform.localPosition = new Vector2(0, 15); });
        }
        bool isSpining = false;
        int rewardNum = 0;
        bool rewardIsGold = false;
        float rewardMutiple = 1;
        int clickTime = 0;
        void OnSpinButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isSpining) return;
            if (!needAd)
            {
                needAd = true;
            }
            else
            {
                clickTime++;
                MG_Manager.ShowRV(OnSpinAdCallback, clickTime, "dice slots extra spin");
                return;
            }
            isSpining = true;

            rewardNum = MG_Manager.Instance.Random_DiceSlotsReward(out rewardIsGold, out rewardMutiple);
            StartCoroutine(StartSpin());

        }
        void OnSpinAdCallback()
        {
            clickTime = 0;
            isSpining = true;
            rewardNum = MG_Manager.Instance.Random_DiceSlotsReward(out rewardIsGold, out rewardMutiple);
            StartCoroutine(StartSpin());
        }
        void OnNothanksClick()
        {
            if (isSpining) return;
            MG_Manager.ShowIV(OnPopAdCallback, "dice slots nothanks");
        }
        void OnPopAdCallback()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DiceSlotsPanel);
        }
        bool needAd = false;
        IEnumerator StartSpin()
        {
            Material mt_L = img_L.material;
            Material mt_M = img_M.material;
            Material mt_R = img_R.materialForRendering;

            float endOffsetY_L = 0;
            float endOffsetY_M = 0;
            float endOffsetY_R = 0;

            if (rewardNum == 0)
            {
                int count = dic_name_offsetY.Count;
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
                foreach (float y in dic_name_offsetY.Values)
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
            else
            {
                string key = (rewardIsGold ? "Gold" : "Cash") + rewardNum;
                if (dic_name_offsetY.TryGetValue(key, out float offsetY))
                {
                    endOffsetY_L = endOffsetY_M = endOffsetY_R = offsetY;
                }
                else
                {
                    Debug.LogError("Get MG_DiceSlotsRewardOffsetY Error : key is not exist in dict. key : " + key);
                    isSpining = false;
                    yield break;
                }
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
                timer += Time.unscaledDeltaTime * 2;
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
            img_buttonText.sprite = sp_adAgain;
            yield return MG_Manager.WaitForSeconds(0.5f);
            StopCoroutine("AutoShiningLight");
            if (rewardNum == 0)
            {
                isSpining = false;
                StartCoroutine("WaitShowNothanks");
            }
            else
            {
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DiceSlotsPanel);
                if (rewardIsGold)
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Gold, rewardNum, rewardMutiple);
                else
                    MG_Manager.Instance.Show_CashRewardPanel(MG_RewardPanelType.AdClaim, rewardNum, rewardMutiple);
            }
        }
        public Text text_nothanks;
        Color32 color_EndThanks = new Color32(154, 154, 154, 255);
        Color32 color_StartThanks = new Color32(154, 154, 154, 0);
        IEnumerator WaitShowNothanks()
        {
            if (text_nothanks.color.a > 0)
                yield break;
            yield return MG_Manager.WaitForSeconds(1);
            float progress = 0;
            while (progress<1)
            {
                yield return null;
                progress+= Time.unscaledDeltaTime * 2;
                text_nothanks.color = new Color32(color_EndThanks.r, color_EndThanks.g, color_EndThanks.b, (byte)(255 * progress));
            }
            text_nothanks.color = color_EndThanks;
            text_nothanks.raycastTarget = true;
        }
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            isSpining = false;
            needAd = false;
            img_buttonText.sprite = sp_spin;
            img_L.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_name_offsetY["7"]));
            img_M.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_name_offsetY["7"]));
            img_R.material.SetTextureOffset(mat_mainTex_Key, new Vector2(finalOffsetX, dic_name_offsetY["7"]));
            text_nothanks.color = color_StartThanks;
            text_nothanks.raycastTarget = false;

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

            if (!isSpining)
            {
                if (MG_Manager.Instance.hasGift)
                {
                    MG_Manager.Instance.hasGift = false;
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                }
                else
                    MG_UIManager.Instance.MenuPanel.CheckGuid();
            }
            else
            {
                MG_Manager.Instance.canChangeGame = true;
            }
            StopCoroutine("WaitShowNothanks");
        }
        public override void OnPause()
        {

        }
        public override void OnResume()
        {

        }
    }
}
