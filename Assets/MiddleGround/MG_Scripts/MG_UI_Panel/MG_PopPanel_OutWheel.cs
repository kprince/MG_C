using MiddleGround.GameConfig;
using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_OutWheel : MG_UIBase
    {
        public List<Image> img_AllIcons = new List<Image>();
        public List<Text> text_AllTexts = new List<Text>();

        readonly Dictionary<int, Sprite> dic_type_sprite = new Dictionary<int, Sprite>();

        public RectTransform rect_wheel;
        public GameObject go_lock;
        public Image img_midhandle;
        public Text text_locktime;
        Image img_giveup;

        public Button btn_Speedup;
        public Button btn_Close;
        MG_Wheel_RewardType[] _Wheel_RewardTypes;
        int[] _Wheel_RewardNums;
        protected override void Awake()
        {
            base.Awake();
            img_giveup = btn_Close.image;
            btn_Speedup.onClick.AddListener(OnSpinClick);
            btn_Close.onClick.AddListener(OnCloseClick);

            SpriteAtlas wheelSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_GamePanelType.WheelPanel);
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            int typeCount = (int)MG_Wheel_RewardType.TypeNum;
            for(int i = 0; i < typeCount; i++)
            {
                string name = ((MG_Wheel_RewardType)i).ToString();
                dic_type_sprite.Add(i, wheelSA.GetSprite("MG_Sprite_Wheel_" + name));
            }

            _Wheel_RewardTypes = MG_Manager.Instance.Get_Config_WheelReward(out _Wheel_RewardNums, true);
            if (_Wheel_RewardTypes.Length != img_AllIcons.Count || _Wheel_RewardTypes.Length != text_AllTexts.Count)
            {
                Debug.LogError("Config MG_Wheel Reward Error : config file is not matched in wheel.");
            }
            else
            {
                int totalWheelReward = _Wheel_RewardTypes.Length;
                for(int i = 0; i < totalWheelReward; i++)
                {
                    img_AllIcons[i].sprite = dic_type_sprite[(int)_Wheel_RewardTypes[i]];
                    text_AllTexts[i].text = "x" + GetShowNumString(_Wheel_RewardNums[i]);
                }
            }
        }
        bool isRotating = false;
        int rewardIndex = -1;
        int clickTime = 0;
        void OnSpinClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isRotating) return;
            clickTime++;
            MG_Manager.ShowRV(AdCallback, clickTime, "out wheel spin");
        }
        void OnCloseClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isRotating) return;
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.OutWheelPanel);
        }
        void AdCallback()
        {
            clickTime = 0;
            isRotating = true;
            MG_Manager.Instance.SendAdjustWheelEvent();
            rewardIndex = MG_Manager.Instance.Random_WheelReward(true);
            MG_Manager.Instance.Add_Save_OutWheelTotalTimes();
            StartCoroutine("WaitRoateEnd");
        }
        IEnumerator WaitRoateEnd()
        {
            img_midhandle.color = Color.clear;
            float endAngleZ = (-90 - 30 * rewardIndex) % 360;
            if (endAngleZ < 0)
                endAngleZ += 360;
            float rotateSpeed = 800;
            float rotateBackSpeed = 300;
            float time = 0.6f;
            bool isStop = false;
            bool isBack = false;
            bool isRight = false;
            AudioSource spinAS = MG_Manager.Play_SpinSlots();
            while (!isStop)
            {
                yield return null;
                time -= Time.unscaledDeltaTime;
                if (time <= 0)
                {
                    if (!isRight)
                    {
                        rect_wheel.localEulerAngles = new Vector3(0, 0, endAngleZ);
                        isRight = true;
                    }
                    if (!isBack)
                    {
                        rotateBackSpeed -= 40;
                        rect_wheel.Rotate(new Vector3(0, 0, -Time.unscaledDeltaTime * rotateBackSpeed));
                        if (rotateBackSpeed <= 0)
                            isBack = true;
                    }
                    else
                    {
                        rotateBackSpeed += 20;
                        rect_wheel.Rotate(new Vector3(0, 0, Time.unscaledDeltaTime * rotateBackSpeed));
                        float angleZ = rect_wheel.localEulerAngles.z % 360;
                        if (angleZ < 0)
                            angleZ += 360;
                        if (Mathf.Abs(angleZ - endAngleZ) < 4)
                        {
                            rect_wheel.localEulerAngles = new Vector3(0, 0, endAngleZ);
                            isStop = true;
                        }
                    }
                }
                else
                    rect_wheel.Rotate(new Vector3(0, 0, -Time.unscaledDeltaTime * rotateSpeed));
            }
            spinAS.Stop();
            yield return null;
            img_midhandle.color = Color.white;
            yield return MG_Manager.WaitForSeconds(0.3f);
            img_midhandle.color = Color.clear;
            yield return MG_Manager.WaitForSeconds(0.3f);
            img_midhandle.color = Color.white;
            yield return MG_Manager.WaitForSeconds(0.3f);
            OnIntersititialCallback();
        }
        void OnIntersititialCallback()
        {
            switch (_Wheel_RewardTypes[rewardIndex])
            {
                case MG_Wheel_RewardType.Gold:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.Gold, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Cash:
                    MG_Manager.Instance.Show_CashRewardPanel(MG_RewardPanelType.AdClaim, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Scratch:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.ScratchTicket, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Gift:
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                    break;
                case MG_Wheel_RewardType.StarCoin:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.StarCoin, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.WheelTicket:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.WheelTicket, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Diamond:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.Diamond, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.RedDiamond:
                    MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.RedDiamond, _Wheel_RewardNums[rewardIndex]);
                    break;
            }
            StopCoroutine("WaitUnLock");
            img_giveup.color = Color.white;
            img_giveup.raycastTarget = true;

            isRotating = false;
        }


        public override IEnumerator OnEnter()
        {
            img_midhandle.color = Color.clear;
            bool canRotate = MG_UIManager.Instance.MenuPanel.canFreeRotateWheel;
            StopCoroutine("WaitShowNothanks");
            StopCoroutine("WaitUnLock");
            if (canRotate)
            {
                img_giveup.color = Color.clear;
                img_giveup.raycastTarget = false;
                StartCoroutine("WaitShowNothanks");
            }
            else
            {
                img_giveup.color = Color.white;
                img_giveup.raycastTarget = true;
                StartCoroutine("WaitUnLock");
            }
            btn_Speedup.gameObject.SetActive(canRotate);
            go_lock.gameObject.SetActive(!canRotate);

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

        public override IEnumerator OnExit()
        {
            clickTime = 0;
            Transform transAll = transform.GetChild(1);
            MG_UIManager.Instance.MenuPanel.ResetTimer();
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

        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
            go_lock.gameObject.SetActive(true);
            btn_Speedup.gameObject.SetActive(false);
            MG_UIManager.Instance.MenuPanel.ResetTimer();
            StartCoroutine("WaitUnLock");

        }
        string GetShowNumString(int num)
        {
            string str = num.ToString();
            if (num < 1000)
                return str;
            else if (num < 1000000)
                str = str.Insert(str.Length - 3, ",");
            return str;
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
        IEnumerator WaitUnLock()
        {
            MG_MenuPanel _MenuPanel = MG_UIManager.Instance.MenuPanel;
            while (true)
            {
                yield return null;
                float remainingTime = _MenuPanel.freeWheeltimer;
                if (remainingTime > 0)
                {
                    int minutes = (int)(remainingTime / 60);
                    int seconds = (int)(remainingTime % 60);
                    text_locktime.text = (minutes > 9 ? minutes.ToString() : "0" + minutes) + ":" + (seconds > 9 ? seconds.ToString() : "0" + seconds);
                }
                else
                {
                    go_lock.SetActive(false);
                    btn_Speedup.gameObject.SetActive(true);
                    yield break;
                }
            }
        }
    }
}
