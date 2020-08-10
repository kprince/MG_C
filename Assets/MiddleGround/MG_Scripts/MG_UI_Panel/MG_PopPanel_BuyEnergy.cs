using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_BuyEnergy : MG_UIBase
    {
        public Button btn_Close;
        public Button btn_Buy;
        Image img_Close;
        protected override void Awake()
        {
            base.Awake();
            btn_Close.onClick.AddListener(OnCloseClick);
            btn_Buy.onClick.AddListener(OnBuyClick);
            img_Close = btn_Close.image;
        }
        void OnCloseClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.BuyDiceEnergy);
        }
        int clickTime = 0;
        void OnBuyClick()
        {
            MG_Manager.Play_ButtonClick();
            clickTime++;
            MG_Manager.ShowRV(OnBuyCallback, clickTime, "buy dice energy");
        }
        void OnBuyCallback()
        {
            clickTime = 0;
            MG_Manager.Instance.Add_Save_DiceLife(10);
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.BuyDiceEnergy);
        }
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            img_Close.color = Color.clear;
            img_Close.raycastTarget = false;

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
            StartCoroutine("WaitShowNothanks");
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
            StopCoroutine("WaitShowNothanks");
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator WaitShowNothanks()
        {
            if (img_Close.color.a > 0)
                yield break;
            yield return MG_Manager.WaitForSeconds(1);
            while (img_Close.color.a < 1)
            {
                yield return null;
                img_Close.color += Color.white * Time.unscaledDeltaTime * 2;
            }
            img_Close.color = Color.white;
            img_Close.raycastTarget = true;
        }
    }
}
