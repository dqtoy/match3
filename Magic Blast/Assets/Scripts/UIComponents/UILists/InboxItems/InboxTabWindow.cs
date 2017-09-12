using System;
using System.Linq;
using Assets.Scripts.UIFriendsList.UI;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class InboxTabWindow : TabWindow
    {
        [SerializeField]
        private InboxUIList _baseUiListComponent;
        [SerializeField]
        private InboxCounter _inboxCounter;

        private FacebookManager _facebookManager;

        private void Start()
        {
            if (_facebookManager == null)
            {
                _facebookManager = FacebookManager.Instance;
            }

            if (_inboxCounter != null)
            {
                _inboxCounter.Hide();
            }

            if (_facebookManager != null)
            {
                _facebookManager.OnGetRequestsEvent += OnGetRequestsEvent;
               //CheckForInbox();
            }
        }

        private void OnGetRequestsEvent()
        {
            CheckForInbox();
        }

        private void CheckForInbox()
        {
            if (_facebookManager != null)
            {
                if (_facebookManager.UserRequests != null && _facebookManager.UserRequests.Any())
                {
                    if (_inboxCounter != null)
                    {
                        _inboxCounter.Show(_facebookManager.UserRequests.Count);
                    }
                    _baseUiListComponent.UpdateList(_facebookManager.UserRequests, UIListType.Lives);
                }
                else
                {
                    if (_inboxCounter != null)
                    {
                        _inboxCounter.Hide();
                    }
                }
            }
        }

        public override void Hide()
        {
            base.Hide();
            //_baseUiListComponent.ClearList();
        }
    }
}