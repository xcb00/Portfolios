using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using GoogleMobileAds.Api;

public class GPGSManager : MonoBehaviour
{
    ISavedGameClient saveClient;
    ISavedGameMetadata savedData;
    bool login = false;

#if UNITY_EDITOR
    readonly string bannerID = "ca-app-pub-3940256099942544/6300978111";
    readonly string rewardID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_ANDROID
    readonly string bannerID = "ca-app-pub-5917477148046429/5415332722";
    readonly string rewardID = "ca-app-pub-5917477148046429/2553365663";
#endif

    BannerView bannerView = null;
    RewardedAd rewardAd = null;

    public void StartGoogleSDK()
    {
        MobileAds.Initialize((status) => {
            CreateBanner();
            LoadRewardAd();
        });

        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                saveClient = PlayGamesPlatform.Instance.SavedGame;
                GameManager.Inst.login = true;
            }
            EventHandler.ShowMoneyMessageEvent($"Login {status}");
        });
    }


    #region Save/Load
    void OpenSavedGame(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        saveClient.OpenWithAutomaticConflictResolution(
            Social.localUser.id, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, callback);
    }

    public void SaveData(byte[] playerData)
    {
        OpenSavedGame((status, data) =>
        {
            if (status == SavedGameRequestStatus.Success)
                SaveData(data, playerData);
            else
            {
                EventHandler.CallLoadingPanelInactiveEvent();
                EventHandler.CallShowMoenyMessageEvent(status.ToString());
            }
        });
    }

    void SaveData(ISavedGameMetadata data, byte[] playerData)
    {
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(TimeSpan.Zero.Add(DateTime.Now.TimeOfDay))
            .WithUpdatedDescription(DateTime.Now.ToString());
        SavedGameMetadataUpdate updater = builder.Build();
        saveClient.CommitUpdate(data, updater, playerData,
            (SavedGameRequestStatus status, ISavedGameMetadata _data) => { });

        EventHandler.CallLoadingPanelInactiveEvent();
        EventHandler.CallShowMoenyMessageEvent("Save Success");
    }

    public void LoadData()
    {
        OpenSavedGame((status, data) =>
        {
            if (status == SavedGameRequestStatus.Success)
                LoadData(data);
            else
            {
                EventHandler.CallLoadingPanelInactiveEvent();
                EventHandler.CallShowMoenyMessageEvent("Load Failed");
            }
        });
    }

    void LoadData(ISavedGameMetadata data)
    {
        saveClient.ReadBinaryData(data, (_status, _loadData) =>
        {
            string _msg = "Load Failed";
            if (_status == SavedGameRequestStatus.Success)
            {
                GameManager.Inst.LoadCloudData(_loadData);
                _msg = "Load Success";
            }

            EventHandler.CallLoadingPanelInactiveEvent();
            EventHandler.CallShowMoenyMessageEvent(_msg);
        });
    }
    #endregion

    #region AdMob
    void CreateBanner()
    {
        if(bannerView!=null)
            DestoryBanner();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerView = new BannerView(bannerID, adaptiveSize, AdPosition.Bottom);
        bannerView.OnAdClicked += () => GameManager.Inst.AddGold(50);
        bannerView.OnBannerAdLoadFailed += (status) => EventHandler.CallShowMoenyMessageEvent($"Banner Load Failed {status}");
    }

    public void DestoryBanner()
    {
        bannerView.Destroy();
        bannerView = null;
    }

    public void LoadBanner()
    {
        if (bannerView == null) CreateBanner();
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    void LoadRewardAd()
    {
        if(rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardID, request, (ad, error) =>
        {
            if (error != null || ad == null)
                return;
            rewardAd = ad;
            rewardAd.OnAdFullScreenContentClosed += () => LoadRewardAd();
            rewardAd.OnAdFullScreenContentFailed += (status) => EventHandler.CallShowMoenyMessageEvent($"Reward Load Failed {status}");
        });
    }

    public void ShowReward(int gold, Action<int> addGold)
    {
        if (rewardAd == null || !rewardAd.CanShowAd())
            LoadRewardAd();

        rewardAd.Show((reward) => { addGold?.Invoke(gold); });
    }
    #endregion
}
