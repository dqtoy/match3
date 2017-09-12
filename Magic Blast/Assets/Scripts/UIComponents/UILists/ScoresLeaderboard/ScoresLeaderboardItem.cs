using System.Diagnostics;
using System.Linq;
using Assets.Scripts.FacebookComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.UIFriendsList.ScoresLeaderboard
{
    public class ScoresLeaderboardItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _positionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _scoresText;
        [SerializeField]
        private Image _userImage;
        [SerializeField]
        private Image _userIconFrame;
        [SerializeField]
        private Sprite _iconFramePlayer;

        private FacebookManager _facebookManager;
        


        public void SetItem(UserLeaderboardData data)
        {
            if (data != null)
            {
                if (_facebookManager == null)
                {
                    _facebookManager = FacebookManager.Instance;
                }

                _positionText.text = (data.Position + 1).ToString();
                _scoresText.text = data.LeaderboardValue.ToString();
                _nameText.text = data.DisplayName;

                if (_facebookManager != null)
                {
                    if (_facebookManager.CurrentUserFacebookUserInfo.id == data.FacebookId)
                    {
                        _nameText.text = "You";
                        _userIconFrame.sprite = _iconFramePlayer;
                        var fbUser = _facebookManager.CurrentUserFacebookUserInfo;
                        if (fbUser.ProfilePicture != null)
                        {
                            _userImage.sprite = fbUser.ProfilePicture;
                            Debug.Log("Switching image player");
                        }
                        else
                        {
                            fbUser.OnImageLoaded += () =>
                            {
                                _userImage.sprite = fbUser.ProfilePicture;
                                Debug.Log("Switching image player");
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(data.FacebookId) && _facebookManager.FriendUserFacebookInfos != null && _facebookManager.FriendUserFacebookInfos.Exists(f => f.id == data.FacebookId))
                    {
                        var fbUser = _facebookManager.FriendUserFacebookInfos.First(f => f.id == data.FacebookId);
                        if (fbUser.ProfilePicture != null)
                        {
                            _userImage.sprite = fbUser.ProfilePicture;
                            Debug.Log("Switching image");
                        }
                        else
                        {
                            fbUser.OnImageLoaded += () =>
                            {
                                _userImage.sprite = fbUser.ProfilePicture;
                                Debug.Log("Switching image");
                            };
                        }
                    }
                }
                else
                {
                    Debug.Log("facebook == null");
                }
            }
        }
    }
}