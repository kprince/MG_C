using System;
using System.Text;
using UnityEngine;

namespace MiddleGround.Save
{
    public class MG_SaveManager
    {
        public const int RevertDiceLifeTimePer = 60;

        const string Save_CurrentGamePanel_Key = "MG_GamePanel";
        const string Save_Gold_Key = "MG_Gold";
        const string Save_RedDiamond_Key = "MG_RedDiamond";


        const string Save_CurrentSignDay_Key = "MG_CurrentSignDay";
        const string Save_LastSignDate = "MG_LastSignDate";
        const string Save_SignState_Key = "MG_SignState";
        const string Save_MusicOn_Key = "MG_MusicOn";
        const string Save_SoundOn_Key = "MG_SoundOn";
        const string Save_PackB_Key = "MG_PackB";
        const string Save_CurrentBgIndex_Key = "MG_CurrentBgIndex";
        const string Save_LastGetExtraDate_Key = "MG_LastGetExtraDate";
        const int BgMaxIndex = 3;
        const string Save_Rateus_Key = "MG_Rateus";
        public const int DayMaxExtraRewardTimes = 50;

        const int DiceMaxBrickIndex = 25;
        public const int DiceMaxLife = 20;
        const string Save_DiceStep_Key = "MG_DiceStep";
        const string Save_DiceLife_Key = "MG_DiceLife";
        const string Save_DiceNextGiftTime_Key = "MG_DiceNextGiftTime";
        const string Save_DiceRewardEveryBrick_Key = "MG_DiceRewardEveryBrick";
        const string Save_DiceLastRevertEnergyDate_Key = "MG_DiceLastRevertEnergyDate";

        const string Save_TodayExtraRewardTimes = "MG_TodayExtraTimes";

        const int Scratch_DefaultTickets = 20;
        const string Save_ScratchTicket_Key = "MG_ScratchToken";
        const string Save_ScratchTimes_Key = "MG_ScratchTimes";
        const string Save_ScratchRewardCashNeedTime_Key = "MG_ScratchRewardCashNeedTime";
        const string Save_ScratchCardIndexs_Key = "MG_ScratchCardIndexs";
        const string Save_ScratchTargetCardIndex_Key = "MG_ScratchTargetCardIndex";
        const string Save_ScratchCardRewardNum_Key = "MG_ScratchCardRewardNum";
        const string Save_ScratchCardRewardType_Key = "MG_ScratchCardRewardIsGold";
        const string Save_ScratchBigPrizeNum_Key = "MG_ScratchBigPrizeNum";

        const string Save_ScratchUnlockPlayTimes_Key = "MG_ScratchUnlockPlayTimes";
        const string Save_ScratchLockDate_Key = "MG_ScratchLockDate";
        const string Save_SlotsUnlockPlayTimes_Key = "MG_SlotsUnlockPlayTimes";
        const string Save_SlotsLockDate_Key = "MG_SlotsLockDate";

        const string Save_DiceTotalPlayTimes_Key = "MG_DiceTotalPlayTimes";
        const string Save_ScratchTotalPlayTimes_Key = "MG_ScratchTotalPlayTimes";
        const string Save_SlotsTotalPlayTimes_Key = "MG_SlotsTotalPlayTimes";
        const string Save_WheelTotalPlayTimes_Key = "MG_WheelTotalPlayTimes";
        const string Save_OutWheelTotalPlayTimes_Key = "MG_OutWheelTotalPlayTimes";

        const int Wheel_DefaultTickets = 15;
        const string Save_WheelTicket_Key = "MG_WheelTicket";
        const string Save_WheelLastUseDate_Key = "MG_WheelLastUseDate";
        const string Save_WheelLockDate_Key = "MG_WheelLockDate";
        const string Save_WheelLastLockTimeIndex_Key = "MG_WheelLastLockTimeIndex";

        const string Save_GuidDice_Key = "MG_GuidDice";
        const string Save_GuidScratch_Key = "MG_GuidScratch";
        const string Save_GuidSlots_Key = "MG_GuidSlots";
        const string Save_FirstCome_Key = "MG_FirstCome";

        const string Save_Get777Times_Key = "MG_Get777Times";
        const string Save_GetFruitsTimes_Key = "MG_GetFruitsTimes";
        const string Save_GetAmazonTimes_Key = "MG_GetAmazonTimes";

        public static int Current_GamePanel
        {
            get
            {
                return PlayerPrefs.GetInt(Save_CurrentGamePanel_Key, 1);
            }
            set
            {
                PlayerPrefs.SetInt(Save_CurrentGamePanel_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int Gold
        {
            get
            {
                return PlayerPrefs.GetInt(Save_Gold_Key, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                PlayerPrefs.SetInt(Save_Gold_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int RedDiamond
        {
            get
            {
                return PlayerPrefs.GetInt(Save_RedDiamond_Key, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                PlayerPrefs.SetInt(Save_RedDiamond_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int Get_Token_Num(MG_Shop_ItemType _tokenType,bool isTotal)
        {
            return PlayerPrefs.GetInt("MG_" + (isTotal ? "Total" : "Current") + _tokenType.ToString(), 0);
        }
        public static void Set_Token_Num(MG_Shop_ItemType _tokenType,bool isTotal,int value)
        {
            if (value < 0)
                value = 0;
            PlayerPrefs.SetInt("MG_" + (isTotal ? "Total" : "Current") + _tokenType.ToString(), value);
            PlayerPrefs.Save();
        }

        public static bool MusicOn
        {
            get
            {
                return PlayerPrefs.GetInt(Save_MusicOn_Key, 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(Save_MusicOn_Key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public static bool SoundOn
        {
            get
            {
                return PlayerPrefs.GetInt(Save_SoundOn_Key, 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(Save_SoundOn_Key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public static bool PackB
        {
            get
            {
                return true;
                return PlayerPrefs.GetInt(Save_PackB_Key, 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(Save_PackB_Key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public static int CurrentBgIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_CurrentBgIndex_Key, 0);
            }
            set
            {
                if (value > BgMaxIndex)
                    value = 0;
                PlayerPrefs.SetInt(Save_CurrentBgIndex_Key, value);
                PlayerPrefs.Save();
            }
        }

        public static int DiceCurrentStep
        {
            get
            {
                return PlayerPrefs.GetInt(Save_DiceStep_Key, 0);
            }
            set
            {
                if (value > DiceMaxBrickIndex || value < 0)
                {
                    Debug.LogError("Set MG_DiceCurrentStep Error : value is out of index.");
                    PlayerPrefs.SetInt(Save_DiceStep_Key, 0);
                }
                else
                {
                    PlayerPrefs.SetInt(Save_DiceStep_Key, value);
                }
                PlayerPrefs.Save();
            }
        }
        public static int DiceLife
        {
            get
            {
                return PlayerPrefs.GetInt(Save_DiceLife_Key, DiceMaxLife);
            }
            set
            {
                value = Mathf.Clamp(value, 0, DiceMaxLife);
                PlayerPrefs.SetInt(Save_DiceLife_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int DiceNextGiftTime
        {
            get
            {
                return PlayerPrefs.GetInt(Save_DiceNextGiftTime_Key, 2);
            }
            set
            {
                if (value < 0)
                    value = 0;
                PlayerPrefs.SetInt(Save_DiceNextGiftTime_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int[] DiceRewardEveryBrick
        {
            get
            {
                string saveStr = PlayerPrefs.GetString(Save_DiceRewardEveryBrick_Key, string.Empty);
                if (string.IsNullOrEmpty(saveStr))
                {
                    return null;
                }
                else
                {
                    int[] result = new int[DiceMaxBrickIndex + 1];
                    if (saveStr.Length != DiceMaxBrickIndex + 1)
                    {
                        Debug.LogError("Read MG_SaveDiceBrickReward Error : lose data.");
                        return null;
                    }
                    for(int i = 0; i < DiceMaxBrickIndex + 1; i++)
                    {
                        result[i] = int.Parse(saveStr[i].ToString());
                    }
                    return result;
                }
            }
            set
            {
                if (value.Length != DiceMaxBrickIndex + 1)
                {
                    Debug.LogError("Save MG_SaveDiceBrickReward Error : lose data.");
                    return;
                }
                StringBuilder saveStr = new StringBuilder();
                for(int i = 0; i < DiceMaxBrickIndex + 1; i++)
                {
                    saveStr.Append(value[i].ToString());
                }
                PlayerPrefs.SetString(Save_DiceRewardEveryBrick_Key, saveStr.ToString());
                PlayerPrefs.Save();
            }
        }
        public static DateTime LastRevertEnergyDate
        {
            get
            {
                string saveValue = PlayerPrefs.GetString(Save_DiceLastRevertEnergyDate_Key, string.Empty);
                if (string.IsNullOrEmpty(saveValue))
                {
                    DateTime now = DateTime.Now;
                    PlayerPrefs.SetString(Save_DiceLastRevertEnergyDate_Key, now.ToString());
                    PlayerPrefs.Save();
                    return now;
                }
                else
                {
                    return DateTime.Parse(saveValue);
                }
            }
            set
            {
                PlayerPrefs.SetString(Save_DiceLastRevertEnergyDate_Key, value.ToString());
                PlayerPrefs.Save();
            }
        }

        public static int TodayExtraRewardTimes
        {
            get
            {
                int temp= PlayerPrefs.GetInt(Save_TodayExtraRewardTimes, DayMaxExtraRewardTimes);
                if (Tomorrow)
                {
                    temp = DayMaxExtraRewardTimes;
                    PlayerPrefs.SetInt(Save_TodayExtraRewardTimes, DayMaxExtraRewardTimes);
                    PlayerPrefs.Save();
                }
                return temp;
            }
            set
            {
                if (value >= TodayExtraRewardTimes) return;
                value = Mathf.Clamp(value, 0, DayMaxExtraRewardTimes);
                PlayerPrefs.SetInt(Save_TodayExtraRewardTimes, value);
                PlayerPrefs.SetString(Save_LastGetExtraDate_Key, DateTime.Now.ToString());
                PlayerPrefs.Save();
            }
        }
        static bool Tomorrow
        {
            get
            {
                DateTime now = DateTime.Now;
                string saveStr = PlayerPrefs.GetString(Save_LastGetExtraDate_Key, string.Empty);
                if (string.IsNullOrEmpty(saveStr))
                {
                    return true;
                }
                DateTime last = DateTime.Parse(saveStr);
                if (now.Year > last.Year)
                    return true;
                else if (now.Year == last.Year)
                {
                    if (now.Month > last.Month)
                        return true;
                    else if (now.Month == last.Month)
                    {
                        if (now.Day > last.Day)
                            return true;
                    }
                }
                return false;
            }
        }
        public static int LastSignDay
        {
            get
            {
                return PlayerPrefs.GetInt(Save_CurrentSignDay_Key, 0);
            }
        }
        public static DateTime LastSignDate
        {
            get
            {
                string saveValue = PlayerPrefs.GetString(Save_LastSignDate, string.Empty);
                if (string.IsNullOrEmpty(saveValue))
                {
                    DateTime now = DateTime.Now.AddDays(-1);
                    PlayerPrefs.SetString(Save_LastSignDate, now.ToString());
                    PlayerPrefs.Save();
                    return now;
                }
                else
                {
                    return DateTime.Parse(saveValue);
                }
            }
            set
            {
                PlayerPrefs.SetString(Save_LastSignDate, value.ToString());
                PlayerPrefs.SetInt(Save_CurrentSignDay_Key, LastSignDay + 1);
                PlayerPrefs.Save();
            }
        }
        public static string SignState
        {
            get
            {
                return PlayerPrefs.GetString(Save_SignState_Key, "0000000");
            }
            set
            {
                PlayerPrefs.SetString(Save_SignState_Key, value);
                PlayerPrefs.Save();
            }
        }

        public static int ScratchTickets
        {
            get
            {
                int currentNum = PlayerPrefs.GetInt(Save_ScratchTicket_Key, Scratch_DefaultTickets);
                if (Tomorrow && currentNum <= 15)
                    currentNum += 5;
                PlayerPrefs.SetInt(Save_ScratchTicket_Key, currentNum);
                PlayerPrefs.SetString(Save_LastGetExtraDate_Key, DateTime.Now.ToString());
                return currentNum;
            }
            set
            {
                if (value < 0)
                    value = 0;
                PlayerPrefs.SetInt(Save_ScratchTicket_Key, value);
                PlayerPrefs.SetString(Save_LastGetExtraDate_Key, DateTime.Now.ToString());
                PlayerPrefs.Save();
            }
        }
        public static int ScratchedTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchTimes_Key, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                PlayerPrefs.SetInt(Save_ScratchTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int ScratchRewardCashAsTimeIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchRewardCashNeedTime_Key, 2);
            }
            set
            {
                if (value < 0)
                    value = 2;
                PlayerPrefs.SetInt(Save_ScratchRewardCashNeedTime_Key, value);
            }
        }
        public static int[] ScratchCardIndexs
        {
            get
            {
                string saveStr = PlayerPrefs.GetString(Save_ScratchCardIndexs_Key, string.Empty);
                if (string.IsNullOrEmpty(saveStr))
                    return null;
                else
                {
                    int[] temp = new int[9];
                    string[] splitResult = saveStr.Split(',');
                    for(int i = 0; i < 9; i++)
                    {
                        temp[i] = int.Parse(splitResult[i]);
                    }
                    return temp;
                }
            }
            set
            {
                if(value is null || value.Length < 9)
                {
                    Debug.LogError("Save MG_ScratchCardIndex Error : saveValue is null or not enough length.");
                    return;
                }
                StringBuilder temp = new StringBuilder();
                for(int i = 0; i < 9; i++)
                {
                    if (i < 8)
                        temp.Append(value[i] + ",");
                    else
                        temp.Append(value[i]);
                }
                PlayerPrefs.SetString(Save_ScratchCardIndexs_Key, temp.ToString());
                PlayerPrefs.Save();
            }
        }
        public static int ScratchTargetCardIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchTargetCardIndex_Key, 0);
            }
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt(Save_ScratchTargetCardIndex_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int ScratchCardRewardNum
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchCardRewardNum_Key, 200);
            }
            set
            {
                if (value < 0)
                    return;
                PlayerPrefs.SetInt(Save_ScratchCardRewardNum_Key, value);
                PlayerPrefs.Save();
            }
        }
        /// <summary>
        /// -1 is gold, -2 is cash, -3 is sss
        /// </summary>
        public static int ScratchCardRewardType
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchCardRewardType_Key, -1);
            }
            set
            {
                PlayerPrefs.SetInt(Save_ScratchCardRewardType_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int ScratchBigPrizeNum
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchBigPrizeNum_Key, 0);
            }
            set
            {
                if (value < 0)
                    return;
                PlayerPrefs.SetInt(Save_ScratchBigPrizeNum_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int ScratchLockPlayTimesIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchUnlockPlayTimes_Key, 0);
            }
            set
            {
                if (value > ScratchTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_ScratchUnlockPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static DateTime ScratchLockDate
        {
            get
            {
                return DateTime.Parse(PlayerPrefs.GetString(Save_ScratchLockDate_Key, DateTime.Now.ToString()));
            }
            set
            {
                PlayerPrefs.SetString(Save_ScratchLockDate_Key, value.ToString());
                PlayerPrefs.Save();
            }
        }
        public static int SlotsLockPlayTimeIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_SlotsUnlockPlayTimes_Key, 0);
            }
            set
            {
                if (value > SlotsTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_SlotsUnlockPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static DateTime SlotsLockDate
        {
            get
            {
                return DateTime.Parse(PlayerPrefs.GetString(Save_SlotsLockDate_Key, DateTime.Now.ToString()));
            }
            set
            {
                PlayerPrefs.SetString(Save_SlotsLockDate_Key, value.ToString());
                PlayerPrefs.Save();
            }
        }


        public static int WheelTickets
        {
            get
            {
                string lastUseDateStr = PlayerPrefs.GetString(Save_WheelLastUseDate_Key, "");
                DateTime lastUseDate;
                if (string.IsNullOrEmpty(lastUseDateStr))
                {
                    lastUseDate = DateTime.Now;
                    PlayerPrefs.SetString(Save_WheelLastUseDate_Key, lastUseDate.ToString());
                }
                else
                    lastUseDate = DateTime.Parse(lastUseDateStr);
                int currentTickets = PlayerPrefs.GetInt(Save_WheelTicket_Key, Wheel_DefaultTickets);
                DateTime now = DateTime.Now;
                if (lastUseDate.Year != now.Year)
                    currentTickets = Wheel_DefaultTickets;
                else
                {
                    if (lastUseDate.Month != now.Month)
                        currentTickets = Wheel_DefaultTickets;
                    else
                    {
                        if (lastUseDate.Day != now.Day)
                            currentTickets = Wheel_DefaultTickets;
                    }
                }
                PlayerPrefs.SetInt(Save_WheelTicket_Key, currentTickets);
                PlayerPrefs.Save();

                return currentTickets;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value < WheelTickets)
                    PlayerPrefs.SetString(Save_WheelLastUseDate_Key, DateTime.Now.ToString());
                PlayerPrefs.SetInt(Save_WheelTicket_Key, value);
            }
        }
        public static DateTime WheelLastLockDate
        {
            get
            {
                return DateTime.Parse(PlayerPrefs.GetString(Save_WheelLockDate_Key, DateTime.Now.ToString()));
            }
            set
            {
                PlayerPrefs.SetString(Save_WheelLockDate_Key, value.ToString());
                PlayerPrefs.Save();
            }
        }
        public static int WheelLockTimeIndex
        {
            get
            {
                return PlayerPrefs.GetInt(Save_WheelLastLockTimeIndex_Key, -1);
            }
            set
            {
                if (value % 3 != 0)
                {
                    Debug.LogError("Set MG_WheelLockIndex Error : index is not 3's mutiple.");
                    return;
                }
                PlayerPrefs.SetInt(Save_WheelLastLockTimeIndex_Key, value);
                PlayerPrefs.Save();
            }
        }


        public static int DiceTotalPlayTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_DiceTotalPlayTimes_Key, 0);
            }
            set
            {
                if (value <= DiceTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_DiceTotalPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int ScratchTotalPlayTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_ScratchTotalPlayTimes_Key, 0);
            }
            set
            {
                if (value <= ScratchTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_ScratchTotalPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int SlotsTotalPlayTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_SlotsTotalPlayTimes_Key, 0);
            }
            set
            {
                if (value <= SlotsTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_SlotsTotalPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int WheelTotalPlayTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_WheelTotalPlayTimes_Key, 0);
            }
            set
            {
                if (value <= WheelTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_WheelTotalPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int OutWheelTotalPlayTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_OutWheelTotalPlayTimes_Key, 0);
            }
            set
            {
                if (value <= OutWheelTotalPlayTimes)
                    return;
                PlayerPrefs.SetInt(Save_OutWheelTotalPlayTimes_Key, value);
                PlayerPrefs.Save();
            }
        }
        public static int TotalPlayTimes
        {
            get
            {
                return DiceTotalPlayTimes + ScratchTotalPlayTimes + WheelTotalPlayTimes + SlotsTotalPlayTimes;
            }
        }
        public static bool HasRateus
        {
            get
            {
                return PlayerPrefs.GetInt(Save_Rateus_Key, 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(Save_Rateus_Key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public static bool GuidDice
        {
            get
            {
                return PlayerPrefs.GetInt(Save_GuidDice_Key, 0) == 1;
            }
            set
            {
                if (value)
                {
                    PlayerPrefs.SetInt(Save_GuidDice_Key, 1);
                    PlayerPrefs.Save();
                }
            }
        }
        public static bool GuidScratch
        {
            get
            {
                return PlayerPrefs.GetInt(Save_GuidScratch_Key, 0) == 1;
            }
            set
            {
                if (value)
                {
                    PlayerPrefs.SetInt(Save_GuidScratch_Key, 1);
                    PlayerPrefs.Save();
                }
            }
        }
        public static bool GuidSlots
        {
            get
            {
                return PlayerPrefs.GetInt(Save_GuidSlots_Key, 0) == 1;
            }
            set
            {
                if (value)
                {
                    PlayerPrefs.SetInt(Save_GuidSlots_Key, 1);
                    PlayerPrefs.Save();
                }
            }
        }
        public static bool FirstCome
        {
            get
            {
                return PlayerPrefs.GetInt(Save_FirstCome_Key, 1) == 1;
            }
            set
            {
                if (!value)
                {
                    PlayerPrefs.SetInt(Save_FirstCome_Key, 0);
                    PlayerPrefs.Save();
                }
            }
        }
        public static int Get777Times
        {
            get
            {
                return PlayerPrefs.GetInt(Save_Get777Times_Key, 0);
            }
            set
            {
                if (value > 0)
                {
                    PlayerPrefs.SetInt(Save_Get777Times_Key, value);
                    PlayerPrefs.Save();
                }
            }
        }
        public static int GetFruitsTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_GetFruitsTimes_Key, 0);
            }
            set
            {
                if (value > 0)
                {
                    PlayerPrefs.SetInt(Save_GetFruitsTimes_Key, value);
                    PlayerPrefs.Save();
                }
            }
        }
        public static int GetStarcoinTimes
        {
            get
            {
                return PlayerPrefs.GetInt(Save_GetAmazonTimes_Key, 0);
            }
            set
            {
                if (value > 0)
                {
                    PlayerPrefs.SetInt(Save_GetAmazonTimes_Key, value);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}
