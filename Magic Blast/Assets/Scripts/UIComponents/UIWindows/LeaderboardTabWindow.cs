using Assets.Scripts.FacebookComponents;
using Assets.Scripts.UIFriendsList.UI;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class LeaderboardTabWindow : TabWindow
    {
        [SerializeField]
        private LeaderboardUIList _list;

        public override void Show()
        {
            base.Show();
            if (LeaderboardController.Instance != null)
            {
                _list.UpdateList(LeaderboardController.Instance.LevelsLeaderboard, UIListType.Leaderboard);
            }
        }

        public override void Hide()
        {
            base.Hide();
            _list.ClearList();
        }
    }
}