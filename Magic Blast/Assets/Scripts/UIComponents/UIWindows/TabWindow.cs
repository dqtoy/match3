using UnityEngine;

namespace Assets.Scripts.UIFriendsList.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class TabWindow : MonoBehaviour
    {
        [SerializeField]
        private string _windowId = "";

        private CanvasGroup _canvasGroup;

        public string WindowId
        {
            get { return _windowId; }
        }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}