using System;
using UnityEngine;

public class CharacterPhotoSetter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _characterImageComponent;

    private FacebookUserInfo _currentUser;

    private FacebookManager _fbManager;

    private void Start()
    {
        _fbManager = FacebookManager.Instance;
        if (_fbManager != null)
        {
            _fbManager.OnUserInfoDownloadedEvent += OnUserInfoDownloadedEvent;
        }
    }

    private void OnUserInfoDownloadedEvent()
    {
        _currentUser = _fbManager.CurrentUserFacebookUserInfo;

        if (_currentUser.ProfilePicture == null)
        {
            _currentUser.OnImageLoaded += OnImageLoaded;
        }
        else
        {
            _characterImageComponent.sprite = _currentUser.ProfilePicture;
        }
    }

    private void OnImageLoaded()
    {
        if (_characterImageComponent != null)
        {
            if (_fbManager.CurrentUserFacebookUserInfo.ProfilePicture != null)
            {
                _characterImageComponent.sprite = _fbManager.CurrentUserFacebookUserInfo.ProfilePicture;
            }
        }
    }
}
