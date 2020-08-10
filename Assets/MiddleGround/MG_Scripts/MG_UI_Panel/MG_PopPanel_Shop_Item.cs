using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Shop_Item : MonoBehaviour
    {
        public Image img_icon;
        public Text text_des;
        public Text text_progress;
        public Slider slider_progress;
        public void Init(Sprite icon,string des)
        {
            img_icon.sprite = icon;
            text_des.text = des;
        }
        public void RefreshProgress(string currentNum,string targetNum,float progress)
        {
            text_progress.text = "<color=#FF780E>" + currentNum + "</color>/" + targetNum;
            slider_progress.value = progress;
        }
    }
}
