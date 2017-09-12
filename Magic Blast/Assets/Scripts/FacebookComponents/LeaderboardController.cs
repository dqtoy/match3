using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FacebookComponents
{
    public class LeaderboardController : MonoBehaviour
    {
        private static LeaderboardController _instance;

        private PlayFabManager _playFabManager;

        private List<UserLeaderboardData> _levelsLeaderboard = new List<UserLeaderboardData>();

        public Action<List<UserLeaderboardData>> OnLevelsLeaderbordUpdated { get; set; }
        public Action<List<UserLeaderboardData>> OnLeaderbordForLevelUpdated { get; set; }

        public static LeaderboardController Instance
        {
            get { return _instance; }
        }

        public List<UserLeaderboardData> LevelsLeaderboard
        {
            get { return _levelsLeaderboard; }
        }

        #region Unity methods
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        private void Start()
        {
            _playFabManager = PlayFabManager.instanse;
            _playFabManager.OnLoggedIn += OnPlayFabLoggedIn;

            _playFabManager.OnLevelsLeaderboardLoaded += OnLevelsLeaderboardLoaded;
            _playFabManager.OnGetLevelScoresLeaderboardLoaded += OnGetLevelScoresLeaderboardLoaded;
        }

        #endregion

        #region Private methods

        private void OnPlayFabLoggedIn()
        {
            UpdateLevelsLeaderboard();
        }

        private void OnLevelsLeaderboardLoaded(List<UserLeaderboardData> userLeaderboardDatas)
        {
            _levelsLeaderboard = userLeaderboardDatas;
            if (OnLevelsLeaderbordUpdated != null)
            {
                OnLevelsLeaderbordUpdated.Invoke(_levelsLeaderboard);
            }
        }

        private void OnGetLevelScoresLeaderboardLoaded(List<UserLeaderboardData> userLeaderboardDatas)
        {
            if (OnLeaderbordForLevelUpdated != null)
            {
                OnLeaderbordForLevelUpdated.Invoke(userLeaderboardDatas);
            }
        }
        #endregion

        #region Public methods
        public LeaderboardController UpdateLevelsLeaderboard()
        {
            _playFabManager.GetPlayersLevelLeaderboard();
            return this;
        }

        public LeaderboardController GetLeaderboardForLevel(int level)
        {
            _playFabManager.GetLevelScoresLeaderboard(level);
            return this;
        }
        #endregion
    }
}