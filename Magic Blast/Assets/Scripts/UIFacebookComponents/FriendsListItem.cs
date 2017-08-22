using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsListItem : MonoBehaviour, IListItem
{
    [SerializeField]
    private Image _userIcon;
    [SerializeField]
    private TextMeshProUGUI _userNameText;
    [SerializeField]
    private Toggle _selectionToggle;

    private Button _selectionButton;

    private FacebookUserInfo _userInfo;

    public Toggle SelectionToggle
    {
        get { return _selectionToggle; }
    }

    public FacebookUserInfo UserInfo
    {
        get { return _userInfo; }
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    public void Start()
    {
        _selectionButton = GetComponent<Button>();

        if (_selectionButton != null)
        {
            _selectionButton.onClick.AddListener(OnItemSelect);
        }
    }

    private void OnItemSelect()
    {
        _selectionToggle.isOn = !_selectionToggle.isOn;
    }

    public void SetItem(object userInfo, UIListType listItemType)
    {
        _userInfo = userInfo as FacebookUserInfo;
        if (userInfo != null)
        {
            _userNameText.text = _userInfo.firstName;

            if (_userInfo.ProfilePicture != null)
            {
                _userIcon.sprite = _userInfo.ProfilePicture;
            }
            else
            {
                _userInfo.OnImageLoaded += () =>
                {
                    _userIcon.sprite = _userInfo.ProfilePicture;
                };
            }
        }
    }
}
