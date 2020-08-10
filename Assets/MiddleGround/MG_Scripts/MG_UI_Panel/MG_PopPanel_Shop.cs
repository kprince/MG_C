using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Shop : MG_UIBase
    {
        public RectTransform rect_Top;
        public RectTransform rect_Mid;
        public RectTransform rect_Bottom;
        public MG_PopPanel_Shop_Item[] items = new MG_PopPanel_Shop_Item[5];
        GameObject[] items_button = new GameObject[5];
        int[] items_redeemNum = new int[5] { 30000, 20, 10, 10, 10 };
        Sprite[] items_icon = new Sprite[5];
        const int redeemRDNum = 1000;

        public Button btn_back;
        public Button btn_toURL;
        public Text text_RDNum;

        SpriteAtlas shopAtlas;
        protected override void Awake()
        {
            base.Awake();
            float lwr = Screen.height / Screen.width;
            if (lwr > 4 / 3f)
            {
                rect_Top.anchoredPosition += new Vector2(0, -87);
                rect_Mid.sizeDelta += new Vector2(0, -87);
                rect_Mid.anchoredPosition += new Vector2(0, -43.5f);
            }
            btn_back.onClick.AddListener(OnBackButtonClick);
            btn_toURL.onClick.AddListener(OnOpenURL);

            shopAtlas = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);

            for (int i = 0; i < (int)MG_Shop_ItemType.TypeNum; i++)
            {
                items_icon[i] = shopAtlas.GetSprite("MG_Sprite_Shop_" + (MG_Shop_ItemType)i);
                items[i].Init(items_icon[i], "Collect " + items_redeemNum[i] + " " + ((MG_Shop_ItemType)i).ToString());
                items_button[i] = items[i].GetComponentInChildren<Button>().gameObject;
            }


        }
        void OnBackButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnOpenURL()
        {
            Application.OpenURL("http://cashout.vip/exchange/index.html?cash=" + MG_Manager.Instance.Get_Save_RedDiamond());
        }
        public void OnGetButtonClick(int _Shop_ItemType)
        {
            MG_Manager.Play_ButtonClick();
            if (float.Parse(MG_Manager.Instance.Get_Save_ShopItems((MG_Shop_ItemType)_Shop_ItemType)) >= items_redeemNum[_Shop_ItemType])
            {
                MG_Manager.Instance.Add_Save_ShopItems((MG_Shop_ItemType)_Shop_ItemType, -items_redeemNum[_Shop_ItemType]);
                MG_Manager.Instance.Add_Save_RedDiamond(redeemRDNum);
                RefreshText((MG_Shop_ItemType)_Shop_ItemType);
                MG_UIManager.Instance.UpdateMenuPanel_SpecialTokenText();
            }
        }
        public override IEnumerator OnEnter()
        {
            yield return null;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
#if UNITY_EDITOR
            rect_Bottom.gameObject.SetActive(true);
#else
            rect_Bottom.gameObject.SetActive(MG_Manager.Instance.NeedShowRedeemShopButton);
#endif
            RefreshAllText();
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            yield return null;
            MG_UIManager.Instance.MenuPanel.UpdateAllContent();
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        void RefreshText(MG_Shop_ItemType _ItemType)
        {
            int index = (int)_ItemType;
            string numStr = MG_Manager.Instance.Get_Save_ShopItems(_ItemType);
            float currentNum = float.Parse(numStr);
            int targetNum = items_redeemNum[index];
            items_button[index].SetActive(currentNum >= targetNum);
            float progress = Mathf.Clamp(currentNum / targetNum, 0, 1);
            items[index].RefreshProgress(numStr, targetNum.ToString(), progress);
            text_RDNum.text = MG_Manager.Instance.Get_Save_RedDiamond().ToString();
        }
        void RefreshAllText()
        {
            for(int i = 0; i < (int)MG_Shop_ItemType.TypeNum; i++)
            {
                string numStr = MG_Manager.Instance.Get_Save_ShopItems((MG_Shop_ItemType)i);
                float currentNum = float.Parse(numStr);
                int targetNum = items_redeemNum[i];
                items_button[i].SetActive(currentNum >= targetNum);
                float progress = Mathf.Clamp(currentNum / targetNum, 0, 1);
                items[i].RefreshProgress(numStr, targetNum.ToString(), progress);
            }
            text_RDNum.text = MG_Manager.Instance.Get_Save_RedDiamond().ToString();
        }
    }
}
[System.Serializable]
public enum MG_Shop_ItemType
{
    Diamond,
    Dollar,
    Fruits,
    Lucky777,
    StarCoin,
    TypeNum
}
