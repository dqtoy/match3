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
            _facebookItems.SetActive(false);
        }

        private void OnFbLoggedIn()
        {
            _facebookItems.SetActive(true);
        }

        public void SoundOff(GameObject Off)
        {
            if (!Off.activeSelf)
            {
                SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
                InitScript.sound = false;

                Off.SetActive(true);
            }
            else
            {
                SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
                InitScript.sound = true;

                Off.SetActive(false);

            }
            PlayerPrefs.SetInt("Sound", (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
            PlayerPrefs.Save();

        }
        public void MusicOff(GameObject Off)
        {
            if (!Off.activeSelf)
            {
                GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
                InitScript.music = false;

                Off.SetActive(true);
            }
            else
            {
                GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
                InitScript.music = true;

                Off.SetActive(false);

            }
            PlayerPrefs.SetInt("Music", (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
            PlayerPrefs.Save();

        }
    }
}