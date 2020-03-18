﻿using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public static class GoogleController
{
    public delegate void CallBack(bool success);

    /// <summary>
    /// Fetch Google API for user information and inialize account data
    /// (Should be done successfuly for using user id and Google Play services)
    /// </summary>
    /// <param name="callback">Method what wil be called when authentification process done</param>
    public static void Authentificate(CallBack callback)
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) => {
            callback(success);
        });
    }

    /// <summary>
    /// Fully unlocking (reveals hidden also) achivement and showing achivement graphics to user.
    /// NOT USE THIS METHOD FOR INCREMENTALL ACHIVEMENT USE UnlockAchivementIncremental INSTEAD!!!
    /// </summary>
    /// <param name="achivementId">Achivement ID (only from GPGSid class)</param>
    /// <param name="callback">Method what wil be called when unlocking process done</param>
    public static void UnlockAchivement(string achivementId, CallBack callback)
    {
        Social.ReportProgress(achivementId, 100.0f, (bool success) => {
            callback(success);
        });
    }

    /// <summary>
    /// Reveals hidden achivement and showing achivement graphics to user, does nothing to visible achivement
    /// </summary>
    /// <param name="achivementId">Achivement ID (only from GPGSid class)</param>
    /// <param name="callback">Method what wil be called when unlocking process done</param>
    public static void RevealAchivement(string achivementId, CallBack callback)
    {
        Social.ReportProgress(achivementId, 0.0f, (bool success) =>{
            callback(success);
        });
    }

    /// <summary>
    /// Use this method to add progrss to incrementall achivement
    /// </summary>
    /// <param name="achivementId">Achivement ID (only from GPGSid class)</param>
    /// <param name="steps">How many step of achivement must be unlocked</param>
    /// <param name="callback">Method what wil be called when unlocking process done</param>
    public static void UnlockAchivementIncremental(string achivementId, int steps, CallBack callback)
    {
        //PlayGamesPlatform.Instance.IncrementAchievement(achivementId, steps, (bool success) => {
        //    callback(success);
        //});
    }

    /// <summary>
    /// Post score to given leaderboard (Can be used for every score, lower scores will be ignored automatecally)
    /// </summary>
    /// <param name="score">Score value</param>
    /// <param name="leaderboardId">Leaderboard ID (only from GPGSid class)</param>
    /// <param name="callback">Method what wil be called when unlocking process done</param>
    public static void PostSoreToLeaderboard(long score, string leaderboardId, CallBack callback)
    {
        Social.ReportScore(score, leaderboardId, (bool success) => {
            callback(success);
        });
    }

    /// <summary>
    /// Showing a particular leaderboard window to user
    /// </summary>
    /// <param name="leaderboardId">Leaderboard ID (only from GPGSid class)</param>
    //public static void ShowLeaderboardUI(string leaderboardId)
    //{
    //    PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
    //}

    /// <summary>
    /// Showing achivement window to user
    /// </summary>
    public static void ShowAnchivementUI()
    {
        Social.ShowAchievementsUI();
    }

    /// <summary>
    /// Showing leaderboards window to user
    /// </summary>
    public static void ShowLeaderboadrsUI()
    {
        Social.ShowLeaderboardUI();
    }
}
