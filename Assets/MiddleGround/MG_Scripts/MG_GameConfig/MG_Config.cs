using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MiddleGround.GameConfig
{
    [CreateAssetMenu(fileName = "MG_Dice_Config", menuName = "Create MG_Dice_Config File", order = 1001)]
    public class MG_Config : ScriptableObject
    {
        public List<int> MG_Dice_CashRange = new List<int>();
        public List<MG_Dice_ExtraBonusConfig> MG_Dice_ExtraBonusConfigs = new List<MG_Dice_ExtraBonusConfig>();
        public List<MG_Dice_SpecialPropsConfig> MG_Dice_SpecialPropsConfigs = new List<MG_Dice_SpecialPropsConfig>();
        public List<MG_Dice_JackpotConfig> MG_Dice_JackpotConfigs = new List<MG_Dice_JackpotConfig>();
        public List<MG_Dice_BrickConfig> MG_Dice_BricksConfigs = new List<MG_Dice_BrickConfig>();
        public List<MG_Scratch_Config> MG_Scratch_Configs = new List<MG_Scratch_Config>();
        public List<MG_Slots_Config> MG_Slots_Configs = new List<MG_Slots_Config>();
        public List<MG_Wheel_Config> MG_Wheel_Configs = new List<MG_Wheel_Config>();
        public List<MG_Wheel_Config> MG_OutWheel_Configs = new List<MG_Wheel_Config>();
    }
    [System.Serializable]
    public struct MG_Dice_ExtraBonusConfig
    {
        public int rangeIndex;
        public int needTargetStep;
        public float goldBonusRate;
        public float cashBonusRate;
        public int maxGoldBonus;
        public int minGoldBonus;
        public int maxCashBonus;
        public int minCashBonus;
    }
    [System.Serializable]
    public struct MG_Dice_SpecialPropsConfig
    {
        public int rangeIndex;
        public int maxGoldReward;
        public int minGoldReward;
        public int maxCashReward;
        public int minCashReward;
        public List<float> goldMutiple;
        public List<float> cashMutiple;
    }
    [System.Serializable]
    public struct MG_Dice_JackpotConfig
    {
        public float noRewardRate;
        public float goldRewardRate;
        public float cashRewardRate;
        public List<int> goldPool;
        public List<int> cashPool;
        public List<float> mutiplePool;
    }
    [System.Serializable]
    public struct MG_Dice_BrickConfig
    {
        public MG_Dice_BrickType brickType;
        public int minRandomNum;
        public int maxRandomNum;
        public int weight;
        public int maxCanGetValue;
    }
    [System.Serializable]
    public struct MG_Scratch_Config
    {
        public int RnageCashStart;
        public int minScratchTimes;
        public int maxScratchTimes;
        public int minCash;
        public int maxCash;
        public int RnageSssStart;
        public int sssWeight;
        public int goldWeight;
        public int minGold;
        public int maxGold;
    }
    [System.Serializable]
    public struct MG_Slots_Config
    {
        public MG_Slots_RewardType rewardType;
        public int weight;
        public List<int> rewardThisWhereIndex;
        public int rewardPercentRangeMin;
        public int rewardPercentRangeMax;
        public int maxCanGetValue;
    }
    [System.Serializable]
    public struct MG_Wheel_Config
    {
        public MG_Wheel_RewardType rewardType;
        public int rewardNum;
        public int weight;
        public int rewardThisWhereIndex;
        public int maxCanGetValue;
    }
    public enum MG_Dice_BrickType
    {
        Cash = 0,
        Gold = 1,
        Slots = 2,
        Scratch = 3,
        StarCoin = 4,
        Empty = 5
    }
    public enum MG_Slots_RewardType
    {
        Gold,
        Diamond,
        Cash,
        Gift,
        Cherry,
        Orange,
        Watermalen,
        SSS,
        Null,
        SS_Other,
    }
    public enum MG_Wheel_RewardType
    {
        Gold,
        Cash,
        Scratch,
        Gift,
        StarCoin,
        WheelTicket,
        Diamond,
        RedDiamond,
        TypeNum
    }
}
