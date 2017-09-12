using System.Linq;
using Assets.Scripts.FacebookComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIFriendsList.LeaderboardItems
{
    public class LeaderboardListItem : MonoBehaviour, IListItem
    {
        [SerializeField]
        private Image _userIcon;
        [SerializeField]
        private Image _userIconFrame;
        [SerializeField]
        private Sprite _iconFrameDefault;
        [SerializeField]
        private Sprite _iconFramePlayer;

        [SerializeField]
        private TextMeshProUGUI _userName;
        [SerializeField]
        private TextMeshProUGUI _userPlace;
        [SerializeField]
        private TextMeshProUGUI _userScore;

        private UserLeaderboardData _data;

        public GameObject GameObject { get { return gameObject; } }

        public Toggle SelectionToggle { get { return null; } }

        private FacebookManager _facebookManager;

        public UserLeaderboardData Data
        {
            get { return _data; }
        }

        public void SetItem(object data, UIListType itemtype)
        {
            _data = data as UserLeaderboardData;

            if (_facebookManager == null)
            {
                _facebookManager = FacebookManager.Instance;
            }
            if (_data != null)
            {
                _userPlace.text = (_data.Position+1).ToString();
                _userName.text = _data.DisplayName;
                _userScore.text = _data.LeaderboardValue.ToString();
                if (_facebookManager != null)
                {
                    if (_facebookManager.CurrentUserFacebookUserInfo.id == _data.FacebookId)
                    {
                        _userName.text = "You";
                        _userIconFrame.sprite = _iconFramePlayer;
                        var fbUser = _facebookManager.CurrentUserFacebookUserInfo;
                        if (fbUser.ProfilePicture != null)
                        {
                            _userIcon.sprite = fbUser.ProfilePicture;
                        }
                        else
                        {
                            fbUser.OnImageLoaded += () =>
                            {
                                _userIcon.sprite = fbUser.ProfilePicture;
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(_data.FacebookId) && _facebookManager.FriendUserFacebookInfos != null && _facebookManager.FriendUserFacebookInfos.Exists(f => f.id == _data.FacebookId))
                    {
                        var fbUser = _facebookManager.FriendUserFacebookInfos.First(f => f.id == _data.FacebookId);
                        if (fbUser.ProfilePicture != null)
                        {
                            _userIcon.sprite = fbUser.ProfilePicture;
                        }
                        else
                        {
                            fbUser.OnImageLoaded += () =>
                            {
                                _userIcon.sprite = fbUser.ProfilePicture;
                            };
                        }
                    }
                }
            }
        }
    }
}