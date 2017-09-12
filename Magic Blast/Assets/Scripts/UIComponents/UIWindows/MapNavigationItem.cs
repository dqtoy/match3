using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIFriendsList
{
    [System.Serializable]
    public class MapNavigationItem
    {
        [SerializeField]
        private Button _locationButton;
        [SerializeField]
        private float _yTranslation;

        public Button LocationButton
        {
            get { return _locationButton; }
        }

        public float YTranslation
        {
            get { return _yTranslation; }
        }
    }
}