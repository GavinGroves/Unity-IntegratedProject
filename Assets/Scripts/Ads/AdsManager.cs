using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance;
    [SerializeField] bool _testMode = false; //开启测试模式，发布时FALSE
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    private bool _adBannerLoaded = false;
    
#if UNITY_ANDROID
    private string gameID = "5482417";
    private string interPlacementID = "Interstitial_Android";
    private string rewardPlacementID = "Rewarded_Android";
    private string BannerPlacementId = "Banner_Android";
#elif UNITY_IOS
    private string gameID = "5482416";
    private string interPlacementID = "Interstitial_iOS";
    private string rewardPlacementID = "Rewarded_iOS";
    private string BannerPlacementId = "Banner_iOS";
#endif
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        Advertisement.Initialize(gameID, _testMode, this);
        Advertisement.Banner.SetPosition(_bannerPosition);
    }
    
    /// <summary>
    /// 调用显示插页广告
    /// </summary>
    public void ShowInterAds()
    {
        Advertisement.Show(interPlacementID, this);
    }
    
    /// <summary>
    /// 调用显示奖励广告
    /// </summary>
    public void ShowRewardAds()
    {
        Advertisement.Show(rewardPlacementID, this);
    }

    /// <summary>
    /// 调用显示横幅广告
    /// </summary>
    public void ShowBannerAds()
    {
        if (!_adBannerLoaded)
        {
            LoadBanner();
        }
        else
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };
 
            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(BannerPlacementId, options);
        }
    }
    
    /// <summary>
    /// 隐藏横幅广告Banner
    /// </summary>
    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }
    
    #region 初始化
    
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete. 广告初始化完成");

        Advertisement.Load(interPlacementID, this);
        Advertisement.Load(rewardPlacementID, this);
        // Advertisement.Load(BannerPlacementId, this);
        
        LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"广告初始化失败: {error.ToString()} - {message}");
    }
    #endregion 
    
    #region 加载
    
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //（可选）如果广告单元成功加载内容，执行代码。
        Debug.Log("加载广告完成ID: " + adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        //（可选）如果广告单元加载失败，执行代码（例如再次尝试）。
        Debug.Log($"Error Ads 加载广告失败 Unit: {_adUnitId} - {error.ToString()} - {message}");
    }
    
    /// <summary>
    /// 加载Banner
    /// </summary>
    private void LoadBanner()
    {
        if (_adBannerLoaded) return;
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(BannerPlacementId, options);
    }
 
    void OnBannerLoaded()
    {
        _adBannerLoaded = true;
        Debug.Log("Banner Ads 加载完成");  
    }
    
    void OnBannerError(string message)
    {
        _adBannerLoaded = false;
        Debug.Log($"Banner Ads 加载失败 Error: {message}");
        // 可选地执行附加代码，例如尝试加载另一个广告。
    }

    #endregion 
    
    #region 显示
    
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        //如果广告单元展示失败，执行代码（例如加载另一个广告）。
        Debug.Log($"Error Ads 展示广告失败 Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        //广告开始 可执行操作
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        //广告被点击时
    }

    // 广告显示完成后发放奖励：
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(rewardPlacementID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads 奖励广告播放完成");
            // 发放奖励
            
        }
       
    }
    
    void OnBannerClicked() { }

    void OnBannerShown()
    {
        Debug.Log("Banner 显示");
    }

    void OnBannerHidden()
    {
        LoadBanner();
    }

    #endregion
}
