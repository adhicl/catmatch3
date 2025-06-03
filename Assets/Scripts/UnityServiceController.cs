using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityServiceController : MonoBehaviour
{

    #region singleton

    public static UnityServiceController Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    #endregion

    public GameSetting gameSetting;
    
    private string m_ExternalIds;

    private string Leaderboard_id = "World_Wide_Leaderboard";

    public delegate void mDelegate();

    public mDelegate dUserSignedIn;

    public delegate void mJSONReturn(string result);

    public mJSONReturn dLeaderboardResult;
    public mJSONReturn dLeaderboardSentResult;
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity service initialized");

        AuthenticationService.Instance.SignedIn += UserSignedIn;
        
        //if start from game scene
        if (SceneManager.GetActiveScene().name != "LoadingScene")
        {
            gameSetting.Reset();
            SignUpAnonymouslyAsync();
        }
    }

    private void UserSignedIn()
    {
        // Shows how to get the playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId} {gameSetting.curPlayerId}");

        if (gameSetting.curPlayerId != AuthenticationService.Instance.PlayerId)
        {
            gameSetting.curPlayerId = AuthenticationService.Instance.PlayerId;
            gameSetting.curPlayerName = "";
        }
        
        //turn on user analytic now
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Unity analytic start");

        if (dUserSignedIn != null) dUserSignedIn.Invoke();
    }

    private async void GetPlayerName()
    {
        string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        gameSetting.curPlayerName = playerName;
        Debug.Log($"GetPlayerName: '{playerName}'");
    }
    
    public async void SignUpAnonymouslyAsync()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // if not, then do nothing
            Debug.Log("session token exist");
        
            //turn on user analytic now
            AnalyticsService.Instance.StartDataCollection();
            if (dUserSignedIn != null) dUserSignedIn.Invoke();
            return;
        }
        
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void RenamePlayer(string newPlayerName)
    {
        Debug.Log(@"Rename player $newPlayerName");
        await AuthenticationService.Instance.UpdatePlayerNameAsync(newPlayerName);
        
        Debug.Log("Get player name "+AuthenticationService.Instance.PlayerName);
        gameSetting.curPlayerName = AuthenticationService.Instance.PlayerName;
        
        GetPlayerName();
    }
    

    public async void AddScore(float score)
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(Leaderboard_id, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        if (dLeaderboardSentResult != null) dLeaderboardSentResult.Invoke(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void GetPaginatedScores()
    {
        var rangeLimit = 20;
        var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
            Leaderboard_id,
            new GetPlayerRangeOptions{ RangeLimit = rangeLimit }
        );
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        if (dLeaderboardResult != null) dLeaderboardResult.Invoke(JsonConvert.SerializeObject(scoresResponse));
    }
}