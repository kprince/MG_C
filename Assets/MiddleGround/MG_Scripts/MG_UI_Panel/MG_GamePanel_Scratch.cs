using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_GamePanel_Scratch : MG_UIBase
    {
        public Image[] img_Cards = new Image[9];
        private Transform[] trans_Cards = new Transform[9];
        public Image img_TargetCard;
        public Image img_BigPrizeIcon;
        public Image img_SpecialRewardIcon;
        public Image img_Mask;

        public RectTransform trans_Handle;
        private RectTransform rect_Mask;
        float f_halfH;
        float f_halfW;
        public Button btn_AD;
        public GameObject go_AddToken;
        public GameObject go_Lock;
        public GameObject go_SpecialRewardCard;
        public Text text_RewardNum;
        public Text text_BigPrizeNum;
        public Text text_LockDes;
        public Text text_btn;
        public Text text_LockTime;
        public Text text_ScratchTicketNum;

        const int CardSpriteCount = 10;
        Dictionary<int, Sprite> dic_index_TargetCard = new Dictionary<int, Sprite>();
        Dictionary<int, Sprite> dic_index_NormalCard = new Dictionary<int, Sprite>();
        int[] cardsIndex = new int[9];

        SpriteAtlas scratchAtlas;
        SpriteAtlas shopAtlas;
        SpriteAtlas menuAtlas;
        Sprite sp_sss;
        Sprite sp_gold;
        Sprite sp_cash;
        Sprite sp_dollar;
        int rewardNum = 0;
        int rewardType = 0;
        protected override void Awake()
        {
            base.Awake();
            btn_AD.onClick.AddListener(OnAdButtonClick);
            scratchAtlas = MG_UIManager.Instance.GetSpriteAtlas((int)MG_GamePanelType.ScratchPanel);
            shopAtlas = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);
            menuAtlas = MG_UIManager.Instance.GetMenuSpriteAtlas();
            sp_sss = shopAtlas.GetSprite("MG_Sprite_Shop_SSS");
            sp_gold = shopAtlas.GetSprite("MG_Sprite_Shop_Gold");
            sp_cash = shopAtlas.GetSprite("MG_Sprite_Shop_Cash");
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            sp_dollar = menuAtlas.GetSprite("MG_Sprite_Menu_Cash" + (packB ? "B" : "A"));
            img_BigPrizeIcon.sprite = sp_dollar;
            StartAwake();

            rect_Mask = img_Mask.GetComponent<RectTransform>();
            f_halfW = rect_Mask.sizeDelta.x * 0.5f;
            f_halfH = rect_Mask.sizeDelta.y * 0.5f;
            cam_brush.aspect = rect_Mask.sizeDelta.x / rect_Mask.sizeDelta.y;
            Material maskMat = img_Mask.material;
            maskMat.SetFloat(Mat_Alpha_Key, 1);
            hasRefesh = true;

        }
        private Vector3 VectorTransfer(Vector2 point)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect_Mask, point, null, out Vector2 screenpos);
            var viewPos = new Vector2((screenpos.x + f_halfW) / (2 * f_halfW), (screenpos.y + f_halfH) / (2 * f_halfH));
            var pos = cam_brush.ViewportToWorldPoint(viewPos);
            return pos + Vector3.forward;
        }
        int clickTime = 0;
        void OnAdButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isNoTicket)
            {
                clickTime++;
                MG_Manager.ShowRV(OnTicketAdCallback, clickTime, "scratch get ticket");
                return;
            }
            else if(isLock)
            {
                clickTime++;
                MG_Manager.ShowRV(OnLockAdCallback, clickTime, "scratch unlock");
                return;
            }
        }
        void OnTicketAdCallback()
        {
            clickTime = 0;
            MG_Manager.Instance.Add_Save_ScratchTicket(1);
            MG_UIManager.Instance.FlyEffectTo_MenuTarget(btn_AD.transform.position, MG_MenuFlyTarget.ScratchTicket, 1);
            if (!isLock)
            {
                btn_AD.gameObject.SetActive(false);
                go_Lock.SetActive(false);
                StartCoroutine("AutoMoveHandle");
                text_btn.text = "FREE";
            }
            else
            {
                text_LockDes.text = "";
                text_btn.text = "SPEED UP";
                text_LockTime.gameObject.SetActive(true);
                go_AddToken.SetActive(false);
            }
            isNoTicket = false;
        }
        void OnLockAdCallback()
        {
            clickTime = 0;
            MG_SaveManager.ScratchLockDate = System.DateTime.Now.AddSeconds(-3601);
            btn_AD.gameObject.SetActive(false);
            go_Lock.SetActive(false);
            isLock = false;
            StartCoroutine("AutoMoveHandle");
        }
        void ClearBrush()
        {
            StartCoroutine(WaitRefresh());
        }
        IEnumerator WaitRefresh()
        {
            cam_brush.clearFlags = CameraClearFlags.Color;
            yield return null;
            cam_brush.clearFlags = CameraClearFlags.Nothing;
        }
        bool isLock = false;
        bool isNoTicket = false;
        public override IEnumerator OnEnter()
        {
            CheckWehtherLock();
            CheckWetherNoTickets();
            UpdateScratchTicketNumText();
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            if (notTouch && !isNoTicket && !isLock)
                StartCoroutine("AutoMoveHandle");
            yield return null;
            clickTime = 0;
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            StopCoroutine("AutoMoveHandle");
            yield return null;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
            Material maskMat = img_Mask.material;
            maskMat.SetFloat(Mat_Alpha_Key, 1);
            OnRefresh();
        }
        Sprite GetTargetSprite(int index)
        {
            if (dic_index_TargetCard.TryGetValue(index, out Sprite target))
                return target;
            else
            {
                target = scratchAtlas.GetSprite("MG_Sprite_Scratch_T" + index);
                dic_index_TargetCard.Add(index, target);
                return target;
            }
        }
        Sprite GetNormalSprite(int index)
        {
            if (dic_index_NormalCard.TryGetValue(index, out Sprite target))
                return target;
            else
            {
                target = scratchAtlas.GetSprite("MG_Sprite_Scratch_N" + index);
                dic_index_NormalCard.Add(index, target);
                return target;
            }
        }
        void RandomCards()
        {
            trans_UnShow.Clear();
            foreach(Transform card in trans_Cards)
            {
                trans_UnShow.Add(card);
            }
            rewardNum = MG_Manager.Instance.Random_ScratchCardReward(out rewardType);
            List<int> hasSure = new List<int>();
            int placeIndex = Random.Range(0, 9);
            hasSure.Add(placeIndex);
            if (rewardType == -1)
            {
                img_Cards[placeIndex].gameObject.SetActive(false);
                cardsIndex[placeIndex] = -1;
                go_SpecialRewardCard.SetActive(true);
                go_SpecialRewardCard.transform.localPosition = img_Cards[placeIndex].transform.localPosition;
                img_SpecialRewardIcon.sprite = sp_gold;
                text_RewardNum.text = rewardNum.ToString();
            }
            else if (rewardType == -2)
            {
                img_Cards[placeIndex].gameObject.SetActive(false);
                cardsIndex[placeIndex] = -2;
                go_SpecialRewardCard.SetActive(true);
                go_SpecialRewardCard.transform.localPosition = img_Cards[placeIndex].transform.localPosition;
                img_SpecialRewardIcon.sprite = sp_cash;
                text_RewardNum.text = rewardNum.ToString();
            }
            else if (rewardType == -3)
            {
                img_Cards[placeIndex].gameObject.SetActive(true);
                go_SpecialRewardCard.SetActive(false);
                cardsIndex[placeIndex] = -3;
                img_Cards[placeIndex].sprite = sp_sss;
            }

            int targetIndex = Random.Range(0, CardSpriteCount);
            MG_SaveManager.ScratchTargetCardIndex = targetIndex;
            Sprite targetSprite = GetTargetSprite(targetIndex);
            img_TargetCard.sprite = targetSprite;
            int targetNum = Random.Range(1, 3);
            for (int i = 0; i < targetNum; i++)
            {
                int randomPlaceIndex;
                do { randomPlaceIndex = Random.Range(0, 9); }
                while (hasSure.Contains(randomPlaceIndex));
                hasSure.Add(randomPlaceIndex);
                cardsIndex[randomPlaceIndex] = targetIndex;
                img_Cards[randomPlaceIndex].sprite = targetSprite;
                img_Cards[randomPlaceIndex].gameObject.SetActive(true);
            }

            for (int i = 0; i < 9; i++)
            {
                if (hasSure.Contains(i)) continue;
                int randomNormalIndex = Random.Range(0, CardSpriteCount);
                cardsIndex[i] = randomNormalIndex + 10;
                img_Cards[i].sprite = GetNormalSprite(randomNormalIndex);
                img_Cards[i].gameObject.SetActive(true);
            }
            int prizeIndex = Random.Range(0, 4);
            switch (prizeIndex)
            {
                case 0:
                    text_BigPrizeNum.text = "500";
                    MG_SaveManager.ScratchBigPrizeNum = 500;
                    break;
                case 1:
                    text_BigPrizeNum.text = "1,000";
                    MG_SaveManager.ScratchBigPrizeNum = 1000;
                    break;
                case 2:
                    text_BigPrizeNum.text = "1,000,000";
                    MG_SaveManager.ScratchBigPrizeNum = 1000000;
                    break;
                case 3:
                    text_BigPrizeNum.text = "2,000,000";
                    MG_SaveManager.ScratchBigPrizeNum = 2000000;
                    break;
            }
            MG_SaveManager.ScratchCardIndexs = cardsIndex;
        }
        public RectTransform trans_brush;
        public Camera cam_brush;
        public void StartBrush(Vector3 pos)
        {
            if (isClearing) return;
            notTouch = false;
            hasRefesh = false;
            StopCoroutine("AutoMoveHandle");
            if (trans_Handle.gameObject.activeSelf)
                trans_Handle.gameObject.SetActive(false);
            pos.z = 0;
            trans_brush.GetComponent<TrailRenderer>().Clear();
            trans_brush.position = VectorTransfer(pos);
            lastPos = pos;
            trans_brush.gameObject.SetActive(true);
            CheckAllCardShowWithPoint(pos);
            if (trans_UnShow.Count <= 2)
            {
                isClearing = true;
                MG_Manager.Instance.SendAdjustScratchEvent();
                if (MG_SaveManager.ScratchTotalPlayTimes > 0 && MG_SaveManager.ScratchTotalPlayTimes % 2 == 0)
                    MG_Manager.ShowIV(OnPopAdCallback, "scratch per 2 time iv");
                else
                    OnPopAdCallback();
            }
        }
        Vector3 lastPos = Vector3.zero;
        public void SetBrushPos(Vector3 pos)
        {
            if (isClearing) return;
            pos.z = 0;
            trans_brush.position = VectorTransfer(pos);
            CheckAllCardShowWithLine(pos);
            lastPos = pos;
            if (trans_UnShow.Count <= 2)
            {
                isClearing = true;
                MG_Manager.Instance.SendAdjustScratchEvent();
                if (MG_SaveManager.ScratchTotalPlayTimes > 0 && MG_SaveManager.ScratchTotalPlayTimes % 2 == 0)
                    MG_Manager.ShowIV(OnPopAdCallback, "scratch per 2 time iv");
                else
                    OnPopAdCallback();
            }
        }
        public void EndBrush(Vector3 pos)
        {
            if (isClearing) return;
            trans_brush.gameObject.SetActive(false);
            SetBrushPos(pos);
        }
        bool isClearing = false;
        const string Mat_Alpha_Key = "_Alpha";
        IEnumerator WaitForClearScratchCard()
        {
            Material maskMat = img_Mask.material;
            float alpha = 1;
            yield return null;
            for (int i = 0; i < 9; i++)
            {
                if (cardsIndex[i] < 0)
                {
                    while (alpha > 0)
                    {
                        yield return null;
                        alpha -= Time.unscaledDeltaTime*5;
                        if (alpha < 0)
                            alpha = 0;
                        maskMat.SetFloat(Mat_Alpha_Key, alpha);
                    }
                    yield return MG_Manager.WaitForSeconds(0.5f);
                    switch (rewardType)
                    {
                        case -1:
                            MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.Gold, rewardNum);
                            break;
                        case -2:
                            MG_Manager.Instance.Show_CashRewardPanel(MG_RewardPanelType.AdClaim, rewardNum);
                            break;
                        case -3:
                            MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.SSS, rewardNum);
                            break;
                    }
                    MG_Manager.Instance.Add_Save_ScratchTicket(-1);
                    break;
                }
            }
            isClearing = false;
        }
        void OnPopAdCallback()
        {
            trans_brush.gameObject.SetActive(false);
            StartCoroutine(WaitForClearScratchCard());
        }
        static float Equation(float K, float C, float x)
        {
            return K * x + C;
        }
        static float ArcEquation(float K, float C, float Y)
        {
            return (Y - C) / K;
        }
        public static float DistancePoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2,float k,float c)
        {
            float y = Equation(k, c, point.x);
            Vector3 pointInLine = new Vector2(point.x, y);
            float angle = Vector2.Angle(point - pointInLine, linePoint1 - linePoint2);
            float stokePointY = (point.y - pointInLine.y) * Mathf.Cos(angle) * Mathf.Cos(angle) + pointInLine.y;
            float stokePointX = ArcEquation(k, c, stokePointY);
            if (stokePointX >= linePoint2.x)
                return (point - linePoint2).magnitude;
            else if (stokePointX <= linePoint1.x)
                return (point - linePoint1).magnitude;
            else
            {
                return (point.y - pointInLine.y) * Mathf.Sin(angle);
            }
        }
        readonly List<Transform> trans_UnShow = new List<Transform>();
        bool CheckAllCardShowWithLine(Vector3 pos)
        {
            bool showAll = false;
            int count = trans_UnShow.Count;
            float k = (pos.y - lastPos.y) / (pos.x - lastPos.x);
            float c = (pos.x * lastPos.y - pos.y * lastPos.x) / (pos.x - lastPos.y);
            List<Transform> willRemoveTrans = new List<Transform>();
            for(int i = 0; i < count; i++)
            {
                float distance = DistancePoint2Line(trans_UnShow[i].position, pos, lastPos, k, c);
                if (distance < 130)
                {
                    willRemoveTrans.Add(trans_UnShow[i]);
                }
            }
            foreach (Transform index in willRemoveTrans)
                trans_UnShow.Remove(index);
            if (trans_UnShow.Count == 0)
            {
                showAll = true;
            }
            return showAll;
        }
        bool CheckAllCardShowWithPoint(Vector3 pos)
        {
            bool showAll = false;
            int count = trans_UnShow.Count;
            List<Transform> willRemoveTrans = new List<Transform>();
            for (int i = 0; i < count; i++)
            {
                float distance = Vector3.Distance(pos, trans_UnShow[i].position);
                if (distance < 130)
                {
                    willRemoveTrans.Add(trans_UnShow[i]);
                }
            }
            foreach (Transform index in willRemoveTrans)
                trans_UnShow.Remove(index);
            if (trans_UnShow.Count == 0)
            {
                showAll = true;
            }
            return showAll;
        }
        void CheckWehtherLock()
        {
            if (MG_SaveManager.ScratchTotalPlayTimes > 0 && MG_SaveManager.ScratchTotalPlayTimes % 5 == 0)
            {
                if (MG_SaveManager.ScratchTotalPlayTimes > MG_SaveManager.ScratchLockPlayTimesIndex)
                {
                    isLock = true;
                    text_LockTime.gameObject.SetActive(true);
                    MG_SaveManager.ScratchLockPlayTimesIndex = MG_SaveManager.ScratchTotalPlayTimes;
                    MG_SaveManager.ScratchLockDate = System.DateTime.Now;
                    StopCoroutine("WaitForUnlock");
                    StartCoroutine("WaitForUnlock", 3599);
                    text_LockDes.text = "";
                    text_btn.text = "SPEED UP";
                }
                else if (MG_SaveManager.ScratchTotalPlayTimes == MG_SaveManager.ScratchLockPlayTimesIndex)
                {
                    System.DateTime now = System.DateTime.Now;
                    System.TimeSpan interval = now - MG_SaveManager.ScratchLockDate;
                    if (interval.TotalSeconds < 3600)
                    {
                        isLock = true;
                        text_LockDes.text = "";
                        text_btn.text = "SPEED UP";
                        text_LockTime.gameObject.SetActive(true);
                        StopCoroutine("WaitForUnlock");
                        StartCoroutine("WaitForUnlock", 3599 - interval.TotalSeconds);
                    }
                    else
                        isLock = false;
                }
            }
            else
                isLock = false;
            go_Lock.SetActive(isLock);
            go_AddToken.SetActive(!isLock);
            btn_AD.gameObject.SetActive(isLock);
        }
        void CheckWetherNoTickets()
        {
            if (MG_SaveManager.ScratchTickets <= 0)
            {
                isNoTicket = true;
                text_LockTime.gameObject.SetActive(false);
                if (isLock)
                {
                    go_AddToken.SetActive(true);
                }
                else
                {
                    go_Lock.SetActive(true);
                    btn_AD.gameObject.SetActive(true);
                    go_AddToken.SetActive(true);
                }
                text_LockDes.text = "Play more and collect more tickets.";
                text_btn.text = "FREE";
            }
            else
                isNoTicket = false;
        }
        bool notTouch = true;
        bool hasRefesh = false;
        void OnRefresh()
        {
            if (hasRefesh) return;
            hasRefesh = true;
            RandomCards();
            ClearBrush();
            CheckWehtherLock();
            CheckWetherNoTickets();
            notTouch = true;
            StopCoroutine("AutoMoveHandle");
            if (notTouch && !isNoTicket && !isLock)
                StartCoroutine("AutoMoveHandle");
        }
        void StartAwake()
        {
            for (int i = 0; i < 9; i++)
            {
                trans_Cards[i] = img_Cards[i].transform;
            }
            trans_UnShow.Clear();
            foreach (Transform card in trans_Cards)
            {
                trans_UnShow.Add(card);
            }
            CheckWehtherLock();
            CheckWetherNoTickets();

            rewardNum = MG_SaveManager.ScratchCardRewardNum;
            rewardType = MG_SaveManager.ScratchCardRewardType;
            int[] saveData = MG_SaveManager.ScratchCardIndexs;
            if (saveData is null || saveData.Length < 9)
            {
                RandomCards();
            }
            else
            {
                img_TargetCard.sprite = GetTargetSprite(MG_SaveManager.ScratchTargetCardIndex);
                cardsIndex = saveData;
                for (int i = 0; i < 9; i++)
                {
                    if (cardsIndex[i] == -3)
                    {
                        img_Cards[i].gameObject.SetActive(true);
                        go_SpecialRewardCard.SetActive(false);
                        img_Cards[i].sprite = sp_sss;
                    }
                    else if (cardsIndex[i] == -1)
                    {
                        img_Cards[i].gameObject.SetActive(false);
                        go_SpecialRewardCard.SetActive(true);
                        go_SpecialRewardCard.transform.localPosition = img_Cards[i].transform.localPosition;
                        img_SpecialRewardIcon.sprite = sp_gold;
                        text_RewardNum.text = rewardNum.ToString();
                    }
                    else if (cardsIndex[i] == -2)
                    {
                        img_Cards[i].gameObject.SetActive(false);
                        go_SpecialRewardCard.SetActive(true);
                        go_SpecialRewardCard.transform.localPosition = img_Cards[i].transform.localPosition;
                        img_SpecialRewardIcon.sprite = sp_cash;
                        text_RewardNum.text = rewardNum.ToString();
                    }
                    else
                    {
                        if (cardsIndex[i] < CardSpriteCount)
                        {
                            img_Cards[i].sprite = GetTargetSprite(cardsIndex[i]);
                        }
                        else
                        {
                            img_Cards[i].sprite = GetNormalSprite(cardsIndex[i] - CardSpriteCount);
                        }
                    }
                }
                switch (MG_SaveManager.ScratchBigPrizeNum)
                {
                    case 500:
                        text_BigPrizeNum.text = "500";
                        break;
                    case 1000:
                        text_BigPrizeNum.text = "1,000";
                        break;
                    case 1000000:
                        text_BigPrizeNum.text = "1,000,000";
                        break;
                    case 2000000:
                        text_BigPrizeNum.text = "2,000,000";
                        break;
                    default:
                        text_BigPrizeNum.text = "0";
                        break;
                }
            }
        }
        public void UpdateScratchTicketNumText()
        {
            text_ScratchTicketNum.text = MG_Manager.Instance.Get_Save_ScratchTicket().ToString();
        }
        IEnumerator AutoMoveHandle()
        {
            if (!trans_Handle.gameObject.activeSelf)
                trans_Handle.gameObject.SetActive(true);
            float angle = -3.8f;
            float r = 300;
            while (true)
            {
                yield return null;
                if (angle > 3.8f)
                {
                    trans_Handle.gameObject.SetActive(false);
                    yield return MG_Manager.WaitForSeconds(1);
                    trans_Handle.gameObject.SetActive(true);
                    angle = -3.8f;
                }
                angle += Time.unscaledDeltaTime*4;
               trans_Handle.localPosition = new Vector2(angle*100, Mathf.Sin(angle) * r);
            }
        }
        IEnumerator WaitForUnlock(int seconds)
        {
            if (seconds == 0)
            {
                CheckWehtherLock();
                yield break;
            }
            int showHours = seconds / 3600;
            int showMinutes = (seconds % 3600) / 60;
            int showSeconds = seconds % 60;
            text_LockTime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            while (seconds > 0)
            {
                yield return MG_Manager.WaitForSeconds(1);
                seconds--;
                if (seconds == 0)
                {
                    CheckWehtherLock();
                    yield break;
                }
                showHours = seconds / 3600;
                showMinutes = (seconds % 3600) / 60;
                showSeconds = seconds % 60;
                text_LockTime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            }
        }
    }
}
