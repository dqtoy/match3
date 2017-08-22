using System;
using UnityEngine;

public class CharacterPhotoSetter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _characterImageComponent;

    private FacebookUserInfo _currentUser;

    private void Start()
    {
        FacebookManager.Instance.OnUserInfoDownloadedEvent += OnUserInfoDownloadedEvent;
    }

    private void OnUserInfoDownloadedEvent()
    {
        _currentUser = FacebookManager.Instance.CurrentUserFacebookUserInfo;

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
            if (FacebookManager.Instance.CurrentUserFacebookUserInfo.ProfilePicture != null)
            {
                _characterImageComponent.sprite = FacebookManager.Instance.CurrentUserFacebookUserInfo.ProfilePicture;
            }
        }
    }
}
