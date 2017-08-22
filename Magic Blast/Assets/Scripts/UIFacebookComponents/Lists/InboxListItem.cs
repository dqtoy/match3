using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIFriendsList
{
    public class InboxListItem : MonoBehaviour, IListItem
    {
        [SerializeField]
        private Image _userIcon;
        [SerializeField]
        private TextMeshProUGUI _actionText;
        [SerializeField]
        private TextMeshProUGUI _informationText;
        [SerializeField]
        private Toggle _selectionToggle;
        [SerializeField]
        private string[] _fields;

        private Button _selectionButton;

        private UserRequestInfo _requestData;

        public Toggle SelectionToggle
        {
            get { return _selectionToggle; }
        }

        public UserRequestInfo RequestData
        {
            get { return _requestData; }
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

        public void SetItem(object data, UIListType itemtype)
        {
            _requestData = data as UserRequestInfo;
            if (_requestData != null)
            {
                if (_fields != null && _fields.Any())
                {
                    _actionText.text = _fields[0];
                    _informationText.text += string.Format("{0} {1}\n", _requestData.User.firstName, _fields[1]);
                    _informationText.text += _fields[2];

                }

                if (_requestData.User.ProfilePicture != null)
                {
                    _userIcon.sprite = _requestData.User.ProfilePicture;
                }
                else
                {
                    _requestData.User.OnImageLoaded += () =>
                    {
                        _userIcon.sprite = _requestData.User.ProfilePicture;
                    };
                }
            }
        }
    }
}