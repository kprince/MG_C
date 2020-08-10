using MiddleGround.GameConfig;
using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_GamePanel_Dice : MG_UIBase
    {
        public Image[] img_Bricks = new Image[26];
        public Transform trans_CurrentStep;
        public Button btn_Roll;
        public Text text_RollNum;
        public Text text_GiftNum;

        public Image img_Dice;
        Animator ac_Dice;
        readonly Sprite[] sp_DiceResults = new Sprite[6];
        List<Sprite> sp_BrickTypes = new List<Sprite>();
        Sprite sp_oldbrick;

        Dictionary<int, MG_Dice_BrickConfig> dic_DiceConfig;

        int[] brick_reward;
        SpriteAtlas DiceAltas;
        protected override void Awake()
        {
            base.Awake();
            btn_Roll.onClick.AddListener(OnRollButtonClick);

            ac_Dice = img_Dice.GetComponent<Animator>();

            DiceAltas = Resources.Load<SpriteAtlas>("MG_SpriteAltas/MG_GamePanel_Dice");

            int length = sp_DiceResults.Length;
            for (int i = 0; i < length; i++)
            {
                sp_DiceResults[i] = DiceAltas.GetSprite("MG_Sprites_Dice" + i);
            }
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            sp_BrickTypes.Add(DiceAltas.GetSprite("MG_Sprite_DiceCash"));
            sp_BrickTypes.Add(DiceAltas.GetSprite("MG_Sprite_DiceGold"));
            sp_BrickTypes.Add(DiceAltas.GetSprite("MG_Sprite_DiceSlots"));
            sp_BrickTypes.Add(DiceAltas.GetSprite("MG_Sprite_DiceScratch"));
            sp_BrickTypes.Add(DiceAltas.GetSprite("MG_Sprite_DiceAmazon"));
            Sprite loadedEmptyBrick = DiceAltas.GetSprite("MG_Sprite_DiceEmpty");
            sp_BrickTypes.Add(loadedEmptyBrick);
            sp_oldbrick = DiceAltas.GetSprite("MG_Sprites_DiceOldBrick");

            dic_DiceConfig = MG_Manager.Instance.Get_Config_DiceBrick();
            brick_reward = MG_SaveManager.DiceRewardEveryBrick;
            if(brick_reward is null)
            {
                RandomBrickReward();
            }
            SetBrickSprite();
            trans_CurrentStep.localPosition = img_Bricks[MG_SaveManager.DiceCurrentStep].transform.localPosition;

            MG_Manager.Instance.Get_OfflineDiceLifeAndNextRevertTime(out int nextNeedSeconds);
            StartCoroutine(TimeClock(nextNeedSeconds));
        }
        int nextStep = 0;
        void OnRollButtonClick()
        {
            MG_Manager.Play_SpinDice();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (MG_Manager.Instance.Get_Save_DiceLife() > 0)
            {
                ac_Dice.enabled = true;
                MG_Manager.Instance.Add_Save_DiceLife(-1);
                ac_Dice.SetBool("RollStart", true);
                nextStep = RandomStep();
                if (MG_SaveManager.TodayExtraRewardTimes > 0)
                    MG_SaveManager.DiceNextGiftTime--;
                MG_Manager.Instance.canChangeGame = false;
                MG_Manager.Instance.SendAdjustDiceEvent();
            }
            else
            {
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.BuyDiceEnergy);
            }
        }
        public void RollEnd()
        {
            ac_Dice.SetBool("RollStart", false);
            ac_Dice.enabled = false;
            if (nextStep > 0)
            {
                img_Dice.sprite = sp_DiceResults[nextStep - 1];
                StartCoroutine(WaitMoveStep());
            }
        }
        int RandomStep()
        {
            int currentStep = MG_SaveManager.DiceCurrentStep;

            const int emptyIndex = (int)MG_Dice_BrickType.Empty;
            const int cashIndex = (int)MG_Dice_BrickType.Cash;
            const int goldIndex = (int)MG_Dice_BrickType.Gold;
            const int slotsIndex = (int)MG_Dice_BrickType.Slots;
            const int scratchIndex = (int)MG_Dice_BrickType.Scratch;
            const int amazonIndex = (int)MG_Dice_BrickType.StarCoin;
            bool hasEmpty = false;
            int emptyWeight = dic_DiceConfig[emptyIndex].weight;
            int emptyStartNum = -1;
            int emptyEndNum = -1;
            bool hasGold = false;
            int goldWeight = dic_DiceConfig[goldIndex].weight;
            int goldStartNum = -1;
            int goldEndNum = -1;
            bool hasCash = false;
            int cashWeight = dic_DiceConfig[cashIndex].weight;
            int cashStartNum = -1;
            int cashEndNum = -1;
            bool hasSlots = false;
            int slotsWeight = dic_DiceConfig[slotsIndex].weight;
            int slotsStartNum = -1;
            int slotsEndNum = -1;
            bool hasScratch = false;
            int scratchWeight = dic_DiceConfig[scratchIndex].weight;
            int scratchStartNum = -1;
            int scratchEndNum = -1;
            bool hasAmazon = false;
            int amazonWeight = dic_DiceConfig[amazonIndex].weight;
            int amazonStartNum = -1;
            int amamzonEndNum = -1;

            List<int> emptyBricks = new List<int>();
            List<int> goldBricks = new List<int>();
            List<int> cashBricks = new List<int>();
            List<int> scratchBricks = new List<int>();
            List<int> slotsBricks = new List<int>();
            List<int> amazonBricks = new List<int>();

            int total = 0;
            int endIndex = currentStep + 6;
            if (endIndex > brick_reward.Length - 1)
            {
                endIndex = brick_reward.Length - 1;
                hasEmpty = true;
                emptyStartNum = total;
                total += emptyWeight;
                emptyEndNum = total;
                emptyBricks.Add(0);
            }
            for (int i = currentStep+1; i < endIndex; i++)
            {
                int rewardTypeIndex = brick_reward[i];
                switch (rewardTypeIndex)
                {
                    case emptyIndex:
                        if (!hasEmpty)
                        {
                            hasEmpty = true;
                            emptyStartNum = total;
                            total += emptyWeight;
                            emptyEndNum = total;
                        }
                        emptyBricks.Add(i);
                        break;
                    case goldIndex:
                        if (!hasGold)
                        {
                            hasGold = true;
                            goldStartNum = total;
                            total += goldWeight;
                            goldEndNum = total;
                        }
                        goldBricks.Add(i);
                        break;
                    case cashIndex:
                        if (MG_Manager.Instance.Get_Save_TotalCash() >= dic_DiceConfig[cashIndex].maxCanGetValue)
                            break;
                        if (!hasCash)
                        {
                            hasCash = true;
                            cashStartNum = total;
                            total += cashWeight;
                            cashEndNum = total;
                        }
                        cashBricks.Add(i);
                        break;
                    case slotsIndex:
                        if (!hasSlots)
                        {
                            hasSlots = true;
                            slotsStartNum = total;
                            total += slotsWeight;
                            slotsEndNum = total;
                        }
                        slotsBricks.Add(i);
                        break;
                    case scratchIndex:
                        if (!hasScratch)
                        {
                            hasScratch = true;
                            scratchStartNum = total;
                            total += scratchWeight;
                            scratchEndNum = total;
                        }
                        scratchBricks.Add(i);
                        break;
                    case amazonIndex:
                        if (MG_Manager.Instance.Get_Save_TotalStarCoin() >= dic_DiceConfig[amazonIndex].maxCanGetValue)
                            break;
                        if (!hasAmazon)
                        {
                            hasAmazon = true;
                            amazonStartNum = total;
                            total += amazonWeight;
                            amamzonEndNum = total;
                        }
                        amazonBricks.Add(i);
                        break;
                }
            }

            int result = Random.Range(0, total);
            if (hasEmpty && result >= emptyStartNum && result < emptyEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.Empty;
                int endStep = emptyBricks[Random.Range(0, emptyBricks.Count)];
                if (endStep == 0)
                    return brick_reward.Length - currentStep;
                else
                    return endStep - currentStep;
            }
            if (hasGold && result >= goldStartNum && result < goldEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.Gold;
                return goldBricks[Random.Range(0, goldBricks.Count)] - currentStep;
            }
            if (hasCash && result >= cashStartNum && result < cashEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.Cash;
                return cashBricks[Random.Range(0, cashBricks.Count)] - currentStep;
            }
            if (hasSlots && result >= slotsStartNum && result < slotsEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.Slots;
                return slotsBricks[Random.Range(0, slotsBricks.Count)] - currentStep;
            }
            if (hasScratch && result >= scratchStartNum && result < scratchEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.Scratch;
                return scratchBricks[Random.Range(0, scratchBricks.Count)] - currentStep;
            }
            if (hasAmazon && result >= amazonStartNum && result < amamzonEndNum)
            {
                _moveOverReward = MG_Dice_BrickType.StarCoin;
                return amazonBricks[Random.Range(0, amazonBricks.Count)] - currentStep;
            }
            Debug.LogError("Random MG_DiceStep Error : step is zero.");
            _moveOverReward = MG_Dice_BrickType.Empty;
            return 0;
        }
        MG_Dice_BrickType _moveOverReward = MG_Dice_BrickType.Empty;
        const float upMaxY = 100;
        IEnumerator WaitMoveStep()
        {
            int currentStep = MG_SaveManager.DiceCurrentStep;
            for(int i = 0; i < nextStep; i++)
            {
                float progress = 0;
                Vector3 startPos = img_Bricks[currentStep + i].transform.localPosition;
                Vector3 endPos = img_Bricks[currentStep + i + 1 > img_Bricks.Length - 1 ? 0 : currentStep + i + 1].transform.localPosition;
                while (progress < 1)
                {
                    yield return null;
                    progress += Time.unscaledDeltaTime * 4;
                    trans_CurrentStep.localPosition = Vector3.Lerp(startPos, endPos, progress) + new Vector3(0, progress <= 0.5f ? upMaxY * progress : upMaxY * (1 - progress));
                }
                if (currentStep + i + 1 <= img_Bricks.Length - 1)
                    img_Bricks[currentStep + i + 1].sprite = sp_oldbrick;
            }
            if (MG_SaveManager.DiceCurrentStep + nextStep == brick_reward.Length)
            {
                MG_SaveManager.DiceCurrentStep = 0;
                MG_SaveManager.CurrentBgIndex++;
                RandomBrickReward();
                SetBrickSprite();
                if (MG_SaveManager.TodayExtraRewardTimes > 0)
                {
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                    if (!MG_SaveManager.HasRateus && MG_Manager.Instance.Get_Save_PackB() && MG_Manager.Instance.NeedRateusGuid)
                        MG_Manager.Instance.willRateus = true;
                }
                else
                {
                    if (!MG_SaveManager.HasRateus && MG_Manager.Instance.Get_Save_PackB() && MG_Manager.Instance.NeedRateusGuid)
                        MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.Rateus);
                }
            }
            else
            {
                MG_SaveManager.DiceCurrentStep += nextStep;
                //MG_UIManager.Instance.MenuPanel.CheckGuid();
                switch (_moveOverReward)
                {
                    case MG_Dice_BrickType.Gold:
                        MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Gold);
                        break;
                    case MG_Dice_BrickType.Cash:
                        MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Cash);
                        break;
                    case MG_Dice_BrickType.Slots:
                        MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.DiceSlotsPanel);
                        break;
                    case MG_Dice_BrickType.Scratch:
                        MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdDouble, MG_RewardType.ScratchTicket, 1);
                        break;
                    case MG_Dice_BrickType.StarCoin:
                        MG_Manager.Instance.Show_MostRewardPanel(MG_RewardPanelType.AdRandom, MG_RewardType.StarCoin, 1);
                        break;
                    default:
                        if (MG_SaveManager.TodayExtraRewardTimes > 0 && MG_Manager.Instance.hasGift)
                        {
                            MG_Manager.Instance.hasGift = false;
                            MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                        }
                        else
                            MG_UIManager.Instance.MenuPanel.CheckGuid();
                        break;
                }
            }
            MG_Manager.Instance.canChangeGame = true;
        }
        void RandomBrickReward()
        {
            brick_reward = new int[img_Bricks.Length];
            List<int> noRewardBrick = new List<int>();
            for(int i = 0; i < img_Bricks.Length; i++)
            {
                noRewardBrick.Add(i);
            }
            MG_Dice_BrickConfig goldBrickConfig = dic_DiceConfig[(int)MG_Dice_BrickType.Gold];
            MG_Dice_BrickConfig cashBrickConfig = dic_DiceConfig[(int)MG_Dice_BrickType.Cash];
            MG_Dice_BrickConfig slotsBrickConfig = dic_DiceConfig[(int)MG_Dice_BrickType.Slots];
            MG_Dice_BrickConfig scratchBrickConfig = dic_DiceConfig[(int)MG_Dice_BrickType.Scratch];
            MG_Dice_BrickConfig amazonBrickConfig = dic_DiceConfig[(int)MG_Dice_BrickType.StarCoin];
            int spawnGoldBrickCount = Random.Range(goldBrickConfig.minRandomNum, goldBrickConfig.maxRandomNum);
            int spawnCashBrickCount = Random.Range(cashBrickConfig.minRandomNum, cashBrickConfig.maxRandomNum);
            int spawnSlotsBrickCount = Random.Range(slotsBrickConfig.minRandomNum, slotsBrickConfig.maxRandomNum);
            int spawnScratchBrickCount = Random.Range(scratchBrickConfig.minRandomNum, scratchBrickConfig.maxRandomNum);
            int spawnAmazonBrickCount = Random.Range(amazonBrickConfig.minRandomNum, amazonBrickConfig.maxRandomNum);
            if (!MG_Manager.Instance.Get_Save_PackB())
                spawnAmazonBrickCount = 0;
            if (spawnGoldBrickCount > 0)
            {
                for(int i = 0; i < spawnGoldBrickCount; i++)
                {
                    int randomIndex = Random.Range(1, noRewardBrick.Count);
                    int result = noRewardBrick[randomIndex];
                    brick_reward[result] = (int)MG_Dice_BrickType.Gold;
                    noRewardBrick.RemoveAt(randomIndex);
                }
            }
            if (spawnCashBrickCount > 0)
            {
                for(int i = 0; i < spawnCashBrickCount; i++)
                {
                    int randomIndex = Random.Range(1, noRewardBrick.Count);
                    int result = noRewardBrick[randomIndex];
                    brick_reward[result] = (int)MG_Dice_BrickType.Cash;
                    noRewardBrick.RemoveAt(randomIndex);
                }
            }
            if (spawnSlotsBrickCount > 0)
            {
                for (int i = 0; i < spawnSlotsBrickCount; i++)
                {
                    int randomIndex = Random.Range(1, noRewardBrick.Count);
                    int result = noRewardBrick[randomIndex];
                    brick_reward[result] = (int)MG_Dice_BrickType.Slots;
                    noRewardBrick.RemoveAt(randomIndex);
                }
            }
            if (spawnScratchBrickCount > 0)
            {
                for (int i = 0; i < spawnScratchBrickCount; i++)
                {
                    int randomIndex = Random.Range(1, noRewardBrick.Count);
                    int result = noRewardBrick[randomIndex];
                    brick_reward[result] = (int)MG_Dice_BrickType.Scratch;
                    noRewardBrick.RemoveAt(randomIndex);
                }
            }
            if (spawnAmazonBrickCount > 0)
            {
                for (int i = 0; i < spawnAmazonBrickCount; i++)
                {
                    int randomIndex = Random.Range(1, noRewardBrick.Count);
                    int result = noRewardBrick[randomIndex];
                    brick_reward[result] = (int)MG_Dice_BrickType.StarCoin;
                    noRewardBrick.RemoveAt(randomIndex);
                }
            }
            if (noRewardBrick.Count > 0)
            {
                for(int i = 0; i < noRewardBrick.Count; i++)
                {
                    int emptyIndex = noRewardBrick[i];
                    brick_reward[emptyIndex] = (int)MG_Dice_BrickType.Empty;
                }
            }
            MG_SaveManager.DiceRewardEveryBrick = brick_reward;
        }
        void SetBrickSprite()
        {
            int currentStep = MG_SaveManager.DiceCurrentStep;
            for(int i = 1; i < brick_reward.Length; i++)
            {
                if (i > currentStep)
                    img_Bricks[i].sprite = sp_BrickTypes[brick_reward[i]];
                else
                    img_Bricks[i].sprite = sp_oldbrick;
            }
        }
        public void UpdateDiceLifeAndGiftStepText()
        {
            if (MG_SaveManager.DiceNextGiftTime == 0)
            {
                MG_SaveManager.DiceNextGiftTime = MG_Manager.Instance.Get_Config_NextGiftStep();
                MG_Manager.Instance.hasGift = true;
            }
            text_RollNum.text = MG_Manager.Instance.Get_Save_DiceLife() + " / " + MG_SaveManager.DiceMaxLife;
            if (MG_SaveManager.TodayExtraRewardTimes > 0)
            {
                text_GiftNum.text = "Next in <color=#7CFFBB>" + MG_SaveManager.DiceNextGiftTime + "</color> rolls";
                noGift = false;
            }
            else
            {
                noGift = true;
                text_GiftNum.text = "After " + GetStringTime();
            }
        }
        bool noGift = false;
        public override IEnumerator OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            trans_CurrentStep.localPosition = img_Bricks[MG_SaveManager.DiceCurrentStep].transform.localPosition;
            UpdateDiceLifeAndGiftStepText();
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
        IEnumerator TimeClock(int nextEnergyTime)
        {
            while (true)
            {
                yield return MG_Manager.WaitForSeconds(1);
                if (nextEnergyTime == 1)
                {
                    int currentEnergy = MG_Manager.Instance.AddEnergyNatural();
                    nextEnergyTime = MG_SaveManager.RevertDiceLifeTimePer;
                    text_RollNum.text = currentEnergy + " / " + MG_SaveManager.DiceMaxLife;
                }
                else
                    nextEnergyTime--;
                if (noGift)
                {
                    text_GiftNum.text = "After " + GetStringTime();
                }
            }
        }
        string GetStringTime()
        {
            System.DateTime now = System.DateTime.Now;
            string result = "";
            if (23 - now.Hour > 9)
                result += 23 - now.Hour;
            else
                result += "0" + (23 - now.Hour);
            if (59 - now.Minute > 9)
                result += ":" + (59 - now.Minute);
            else
                result += ":0" + (59 - now.Minute);
            if (59 - now.Second > 9)
                result += ":" + (59 - now.Second);
            else
                result += ":0" + (59 - now.Second);
            return result;
        }
    }
}
