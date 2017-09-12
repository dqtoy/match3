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
        private string[] _askLivesFields;
        [SerializeField]
        private string[] _sendLivesFields;

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
            get { return gameObject != null ? gameObject : null; }
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
                if (_requestData.Type.Equals(RequestType.RequestLife))
                {
                    if (_askLivesFields != null && _askLivesFields.Any())
                    {
                        _actionText.text = _askLivesFields[0];
                        _informationText.text = "";
                        _informationText.text += string.Format("{0} {1}\n", _requestData.User.firstName, _askLivesFields[1]);
                        _informationText.text += _askLivesFields[2];

                    }
                }
                else if(_requestData.Type.Equals(RequestType.SendLife))
                {
                    if (_sendLivesFields != null && _sendLivesFields.Any())
                    {
                        _actionText.text = _sendLivesFields[0];
                        _informationText.text = "";
                        _informationText.text += string.Format("{0} {1}\n", _requestData.User.firstName, _sendLivesFields[1]);
                        _informationText.text += _sendLivesFields[2];

                    }
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