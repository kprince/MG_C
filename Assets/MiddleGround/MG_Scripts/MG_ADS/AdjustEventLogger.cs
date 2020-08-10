using com.adjust.sdk;
using MiddleGround;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class AdjustEventLogger : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string Getidfa();
#endif
#if UNITY_IOS
    public const string APP_TOKEN = "";
    public const string TOKEN_open = "";
    public const string TOKEN_ad = "";
    public const string TOKEN_noads = "";
    public const string TOKEN_stage_end = "";
    public const string TOKEN_wheel="";
    public const string TOKEN_slots="";
    public const string TOKEN_ggl = "";
    public const string TOKEN_deeplink = "";
    public const string TOKEN_packb = "";
#elif UNITY_ANDROID
    public const string APP_TOKEN = "";
    public const string TOKEN_open = "";
    public const string TOKEN_ad = "";
    public const string TOKEN_noads = "";
    /// <summary>
    /// dice
    /// </summary>
    public const string TOKEN_stage_end = "";
    public const string TOKEN_wheel = "";
    public const string TOKEN_slots = "";
    /// <summary>
    /// scratch
    /// </summary>
    public const string TOKEN_ggl = "";
    public const string TOKEN_deeplink = "";
    public const string TOKEN_packb = "";
#endif
    public static AdjustEventLogger Instance;
    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Sandbox);
#else
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Production);
#endif
        adjustConfig.sendInBackground = true;
        adjustConfig.launchDeferredDeeplink = true;
        adjustConfig.eventBufferingEnabled = true;
        adjustConfig.logLevel = AdjustLogLevel.Info;
        adjustConfig.setAttributionChangedDelegate(OnAttributionChangedCallback);
        Adjust.start(adjustConfig);
    }
    private void Start()
    {
        GetAdID();
        StartCoroutine(CheckAttributeTo());
        MG_Manager.Instance.SendAdjustGameStartEvent();
    }
    public void AdjustEventNoParam(string token)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        Adjust.trackEvent(adjustEvent);
    }
    public void AdjustEvent(string token, params (string key, string value)[] list)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        foreach (var (key, value) in list)
        {
            adjustEvent.addCallbackParameter(key, value);
        }
        Adjust.trackEvent(adjustEvent);
    }
    void OnAttributionChangedCallback(AdjustAttribution attribution)
    {
        if (attribution.network.Equals("Organic"))
        {
        }
        else
        {
            if (!MG_Manager.Instance.loadEnd)
                MG_Manager.Instance.Set_Save_isPackB();
        }
    }
    private string AppName = Ads.AppName;
    private string ifa;
    private IEnumerator CheckAttributeTo()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(ifa));
        string url = string.Format("http://a.mobile-mafia.com:3838/adjust/is_unity_clicked?ifa={0}&app_name={1}", ifa, AppName);

        var web = new UnityWebRequest(url);
        web.downloadHandler = new DownloadHandlerBuffer();
        yield return web.SendWebRequest();
        if (web.responseCode == 200)
        {
            if (web.downloadHandler.text.Equals("1"))
            {
                if (!MG_Manager.Instance.loadEnd)
                    MG_Manager.Instance.Set_Save_isPackB();
            }
        }
    }
    private void GetAdID()
    {
#if UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               ifa = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
        ifa = Getidfa();
#endif
    }
}

