using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;

namespace MiddleGround.GameConfig
{
    public class MG_ReadConfig_Excel : Editor
    {
        static string XlsxPath;
        [MenuItem("MG Tools/Read Config")]
        public static void ReadConfig()
        {
            MG_Config _Dice_Config = Resources.Load<ScriptableObject>("MG_ConfigAssets/MG_Dice_Config") as MG_Config;
            _Dice_Config.MG_Dice_CashRange.Clear();
            _Dice_Config.MG_Dice_ExtraBonusConfigs.Clear();
            _Dice_Config.MG_Dice_SpecialPropsConfigs.Clear();
            _Dice_Config.MG_Dice_JackpotConfigs.Clear();
            _Dice_Config.MG_Dice_BricksConfigs.Clear();
            _Dice_Config.MG_Scratch_Configs.Clear();
            _Dice_Config.MG_Slots_Configs.Clear();
            _Dice_Config.MG_Wheel_Configs.Clear();
            _Dice_Config.MG_OutWheel_Configs.Clear();

            XlsxPath = Application.dataPath + "/MiddleGround/MG_Config.xlsx";
            if (!File.Exists(XlsxPath))
            {
                Debug.LogError("Read MG_Config.xlsx File Error : file is not exist.");
                return;
            }
            FileStream fs = new FileStream(XlsxPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet dataSet = reader.AsDataSet();
            reader.Dispose();
            if(dataSet is null)
            {
                Debug.LogError("Read MG_ConfigXlsx File Error : file is empty.");
                return;
            }
            #region zeroTable
            DataTable zeroTable = dataSet.Tables[0];
            int rowsCount0 = zeroTable.Rows.Count;
            for(int i = 1; i < rowsCount0; i++)
            {
                var tempRow = zeroTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                MG_Dice_BrickConfig _Dice_BrickConfig = new MG_Dice_BrickConfig()
                {
                    brickType = (MG_Dice_BrickType)System.Enum.Parse(typeof(MG_Dice_BrickType), tempRow[0].ToString()),
                    minRandomNum = int.Parse(tempRow[1].ToString()),
                    maxRandomNum = int.Parse(tempRow[2].ToString()),
                    weight = int.Parse(tempRow[3].ToString()),
                    maxCanGetValue = int.Parse(tempRow[4].ToString())
                };
                _Dice_Config.MG_Dice_BricksConfigs.Add(_Dice_BrickConfig);
            }
            #endregion
            #region firstTable
            DataTable firstTable = dataSet.Tables[1];
            int rowsCount1 = firstTable.Rows.Count;
            for (int i = 2; i < rowsCount1; i++)
            {
                var tempRow = firstTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                //范围
                _Dice_Config.MG_Dice_CashRange.Add(int.Parse(tempRow[1].ToString()) * 100);
                int configIndex = i - 2;
                MG_Dice_ExtraBonusConfig extraBonusConfig = new MG_Dice_ExtraBonusConfig
                {
                    rangeIndex = configIndex,
                    needTargetStep = int.Parse(tempRow[2].ToString()),
                    cashBonusRate = float.Parse(tempRow[3].ToString()),
                    minCashBonus = int.Parse(tempRow[4].ToString()),
                    maxCashBonus = int.Parse(tempRow[5].ToString()),
                    goldBonusRate = float.Parse(tempRow[6].ToString()),
                    minGoldBonus = int.Parse(tempRow[7].ToString()),
                    maxGoldBonus = int.Parse(tempRow[8].ToString()),
                };
                _Dice_Config.MG_Dice_ExtraBonusConfigs.Add(extraBonusConfig);
                MG_Dice_SpecialPropsConfig specialPropsConfig = new MG_Dice_SpecialPropsConfig
                {
                    rangeIndex = configIndex,
                    minCashReward = int.Parse(tempRow[9].ToString()),
                    maxCashReward = int.Parse(tempRow[10].ToString()),
                    minGoldReward = int.Parse(tempRow[12].ToString()),
                    maxGoldReward = int.Parse(tempRow[13].ToString())
                };
                specialPropsConfig.cashMutiple = new List<float>();
                string[] cashmutiples = tempRow[11].ToString().Split(';');
                for (int j = 0; j < cashmutiples.Length; j++)
                {
                    specialPropsConfig.cashMutiple.Add(float.Parse(cashmutiples[j]));
                }
                specialPropsConfig.goldMutiple = new List<float>();
                string[] goldmutiples = tempRow[14].ToString().Split(';');
                for (int j = 0; j < goldmutiples.Length; j++)
                {
                    specialPropsConfig.goldMutiple.Add(float.Parse(goldmutiples[j]));
                }
                _Dice_Config.MG_Dice_SpecialPropsConfigs.Add(specialPropsConfig);
                MG_Dice_JackpotConfig jackpotConfig = new MG_Dice_JackpotConfig()
                {
                    noRewardRate = float.Parse(tempRow[15].ToString()),
                    cashRewardRate = float.Parse(tempRow[16].ToString()),
                    goldRewardRate = float.Parse(tempRow[18].ToString())
                };
                jackpotConfig.cashPool = new List<int>();
                string[] cashNums = tempRow[17].ToString().Split(';');
                for (int j = 0; j < cashNums.Length; j++)
                {
                    jackpotConfig.cashPool.Add(int.Parse(cashNums[j]));
                }
                jackpotConfig.goldPool = new List<int>();
                string[] goldNums = tempRow[19].ToString().Split(';');
                for (int j = 0; j < goldNums.Length; j++)
                {
                    jackpotConfig.goldPool.Add(int.Parse(goldNums[j]));
                }
                jackpotConfig.mutiplePool = new List<float>();
                string[] mutipleNums = tempRow[20].ToString().Split(';');
                for(int j = 0; j < mutipleNums.Length; j++)
                {
                    jackpotConfig.mutiplePool.Add(float.Parse(mutipleNums[j]));
                }
                _Dice_Config.MG_Dice_JackpotConfigs.Add(jackpotConfig);
            }
            #endregion
            #region secondTable
            DataTable secondTable = dataSet.Tables[2];
            int rowCount2 = secondTable.Rows.Count;
            for(int i = 1; i < rowCount2; i++)
            {
                var tempRow = secondTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                MG_Scratch_Config _Scratch_Config = new MG_Scratch_Config()
                {
                    RnageCashStart = int.Parse(tempRow[1].ToString())*100,
                    minScratchTimes = int.Parse(tempRow[2].ToString()),
                    maxScratchTimes = int.Parse(tempRow[3].ToString()),
                    minCash = int.Parse(tempRow[4].ToString()),
                    maxCash = int.Parse(tempRow[5].ToString()),
                    RnageSssStart=int.Parse(tempRow[6].ToString()),
                    sssWeight = int.Parse(tempRow[7].ToString()),
                    goldWeight = int.Parse(tempRow[8].ToString()),
                    minGold = int.Parse(tempRow[9].ToString()),
                    maxGold = int.Parse(tempRow[10].ToString())
                };
                _Dice_Config.MG_Scratch_Configs.Add(_Scratch_Config);
            }
            #endregion
            #region thirdTabl
            DataTable thirdTable = dataSet.Tables[3];
            int rowCount3 = thirdTable.Rows.Count;
            for(int i = 1; i < rowCount3; i++)
            {
                var tempRow = thirdTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                MG_Slots_Config _Slots_Config = new MG_Slots_Config()
                {
                    rewardType = (MG_Slots_RewardType)System.Enum.Parse(typeof(MG_Slots_RewardType), tempRow[0].ToString()),
                    weight = int.Parse(tempRow[1].ToString()),
                    rewardPercentRangeMin = int.Parse(tempRow[3].ToString()),
                    rewardPercentRangeMax = int.Parse(tempRow[4].ToString()),
                    maxCanGetValue = int.Parse(tempRow[5].ToString()),
                };
                if (_Slots_Config.rewardType == MG_Slots_RewardType.Cash ||
                    _Slots_Config.rewardType == MG_Slots_RewardType.Gift)
                    _Slots_Config.maxCanGetValue *= 100;
                _Slots_Config.rewardThisWhereIndex = new List<int>();
                string[] whereIndexs = tempRow[2].ToString().Split(',');
                for(int j = 0; j < whereIndexs.Length; j++)
                {
                    _Slots_Config.rewardThisWhereIndex.Add(int.Parse(whereIndexs[j]));
                }
                _Dice_Config.MG_Slots_Configs.Add(_Slots_Config);
            }
            #endregion
            #region forthTable
            DataTable forthTable = dataSet.Tables[4];
            int rowCount4 = forthTable.Rows.Count;
            for(int i = 1; i < rowCount4; i++)
            {
                var tempRow = forthTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                MG_Wheel_Config _Wheel_Config = new MG_Wheel_Config()
                {
                    rewardType = (MG_Wheel_RewardType)System.Enum.Parse(typeof(MG_Wheel_RewardType), tempRow[1].ToString()),
                    rewardNum = int.Parse(tempRow[2].ToString()),
                    weight = int.Parse(tempRow[3].ToString()),
                    rewardThisWhereIndex = int.Parse(tempRow[4].ToString()),
                    maxCanGetValue = int.Parse(tempRow[5].ToString())
                };
                _Dice_Config.MG_Wheel_Configs.Add(_Wheel_Config);
            }
            #endregion
            #region fifTable
            DataTable fifTable = dataSet.Tables[5];
            int rowCount5= fifTable.Rows.Count;
            for(int i = 1; i < rowCount5; i++)
            {
                var tempRow = fifTable.Rows[i];
                if (string.IsNullOrEmpty(tempRow[0].ToString()))
                    continue;
                MG_Wheel_Config _OutWheel_Config = new MG_Wheel_Config()
                {
                    rewardType = (MG_Wheel_RewardType)System.Enum.Parse(typeof(MG_Wheel_RewardType), tempRow[1].ToString()),
                    rewardNum = int.Parse(tempRow[2].ToString()),
                    weight = int.Parse(tempRow[3].ToString()),
                    rewardThisWhereIndex = int.Parse(tempRow[4].ToString()),
                    maxCanGetValue = int.Parse(tempRow[5].ToString())
                };
                _Dice_Config.MG_OutWheel_Configs.Add(_OutWheel_Config);
            }
            #endregion

            EditorUtility.SetDirty(_Dice_Config);
            AssetDatabase.SaveAssets();
        }
    }
}
