using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class InboxDialogController : MonoBehaviour
    {
        [SerializeField]
        private InboxUIList _baseUiListComponent;

        private FacebookManager _facebookManager;

        private void Start()
        {
            _facebookManager = FacebookManager.Instance;
        }

        public void OnEnable()
        {

            if (_facebookManager == null)
            {
                _facebookManager = FacebookManager.Instance;
            }

            if (_facebookManager != null)
            {
                if (_facebookManager.FriendUserFacebookInfos != null && _facebookManager.UserRequests.Any())
                {
                    _baseUiListComponent.UpdateList(_facebookManager.UserRequests, UIListType.Lives);
                }
            }
        }

        public void OnDisable()
        {
            _baseUiListComponent.ClearList();
        }
    }
}