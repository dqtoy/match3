using System;
using System.Linq;
using UnityEngine;

public class SendLivesDialogController : MonoBehaviour
{
    [SerializeField]
    private SendLivesUIList _baseUiListComponent;

    private FacebookManager _facebookManager;

    private AnimationManager _animationManager;

    private void Start()
    {
        _animationManager = GetComponent<AnimationManager>();
        _facebookManager = FacebookManager.Instance;
        if (_facebookManager != null)
        {
            _facebookManager.OnSendLivesSuccess += OnSendLivesSuccess;
        }
    }

    public void OnEnable()
    {

        if (_facebookManager == null)
        {
            _facebookManager = FacebookManager.Instance;
            _facebookManager.OnSendLivesSuccess += OnSendLivesSuccess;
        }

        if (_facebookManager != null)
        {
            if (_facebookManager.FriendUserFacebookInfos != null && _facebookManager.FriendUserFacebookInfos.Any())
            {
                _baseUiListComponent.UpdateList(_facebookManager.FriendUserFacebookInfos, UIListType.Lives);
            }
        }
    }

    private void OnSendLivesSuccess()
    {
        _animationManager.CloseMenu();
    }

    public void OnDisable()
    {
        _baseUiListComponent.ClearList();
    }
}
