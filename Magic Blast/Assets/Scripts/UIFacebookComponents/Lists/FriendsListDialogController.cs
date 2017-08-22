using System;
using System.Linq;
using Assets.Scripts.UIFriendsList;
using UnityEngine;

public class FriendsListDialogController : MonoBehaviour
{
    [SerializeField]
    private InviteUIList _baseUiListComponent;

    private FacebookManager _facebookManager;

    private AnimationManager _animationManager;

    private void Start()
    {
        _animationManager = GetComponent<AnimationManager>();
        _facebookManager = FacebookManager.Instance;
        if (_facebookManager != null)
        {
            _facebookManager.OnSendInviteSuccess += OnSendInviteSuccess;
        }
    }

    public void OnEnable()
    {

        if (_facebookManager == null)
        {
            _facebookManager = FacebookManager.Instance;
            _facebookManager.OnSendInviteSuccess += OnSendInviteSuccess;
        }
        
        if (_facebookManager != null)
        {
            if (_facebookManager.InventableFriendsList != null && _facebookManager.InventableFriendsList.Any())
            {
                _baseUiListComponent.UpdateList(_facebookManager.InventableFriendsList, UIListType.Invite);
            }
        }
    }

    private void OnSendInviteSuccess()
    {
        _animationManager.CloseMenu();
    }

    public void OnDisable()
    {
        _baseUiListComponent.ClearList();
    }
}
