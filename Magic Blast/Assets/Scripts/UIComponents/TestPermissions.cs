using System;
using TMPro;
using UnityEngine;

public class TestPermissions : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _statusText;

    private FacebookManager _facebookManager;

    private void Start()
    {
        if (_facebookManager == null)
        {
            _facebookManager = FacebookManager.Instance;
        }

        _facebookManager.OnFbLoggedIn += OnFbLoggedIn;
    }

    private void OnFbLoggedIn()
    {
        _statusText.text = "";
        foreach (var facebookManagerUserFacebookPermission in _facebookManager.UserFacebookPermissions)
        {
            _statusText.text = _statusText.text + facebookManagerUserFacebookPermission.Key + "\n";
        }
    }
}
