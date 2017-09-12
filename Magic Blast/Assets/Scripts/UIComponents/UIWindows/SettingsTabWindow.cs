using System;
using Assets.Scripts.UIFriendsList.UI;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class SettingsTabWindow : TabWindow
    {
        [SerializeField]
        private GameObject _facebookItems;

        private FacebookManager _facebookManager;

        private void Start()
        {
            _facebookManager = FacebookManager.Instance;

            if (_facebookManager == null || !_facebookManager.IsInitialized || !_facebookManager.IsLoggedIn)
            {
                _facebookItems.SetActive(false);
            }

            if (_facebookManager != null)
            {
                _facebookManager.OnFbLoggedIn += OnFbLoggedIn;
                _facebookManager.OnFbLoggedOut += OnFbLoggedOut;
            }
        }

        private void OnFbLoggedOut()
        {
            _facebookItems.SetActive(true);
        }

        private void OnFbLoggedIn()
        {
            _facebookItems.SetActive(true);
        }
    }
}