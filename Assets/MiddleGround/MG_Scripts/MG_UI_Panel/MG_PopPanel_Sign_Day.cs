using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Sign_Day : MonoBehaviour
    {
        Image img_rewardIcon;
        Image img_bg;
        GameObject go_sure;
        Text text_rewardNum;
        Text text_day;
        private void Awake()
        {
            img_bg = GetComponent<Image>();
            img_rewardIcon = transform.GetChild(1).GetComponent<Image>();
            text_rewardNum = transform.GetChild(2).GetComponent<Text>();
            text_day = transform.GetChild(0).GetComponent<Text>();
            go_sure = transform.GetChild(3).gameObject;
        }
        public void SetDay(int day,bool get, Sprite bgSp,Sprite rewardSp,string rewardNum)
        {
            text_day.text = "Day " + day;
            img_bg.sprite = bgSp;
            img_rewardIcon.sprite = rewardSp;
            text_rewardNum.text = rewardNum;
            go_sure.SetActive(get);
        }
    }
}
