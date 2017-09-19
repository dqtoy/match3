using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.FacebookComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIFriendsList.ScoresLeaderboard
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScoresLeaderboardWindowController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _leaderboardItemGameObject;
        [SerializeField]
        private GameObject _inviteLeaderboardItemGameObject;
        [SerializeField]
        private Image _separatorImage;
        [SerializeField]
        private Transform _rootTransform;

        private List<GameObject> _items = new List<GameObject>(); 

        private CanvasGroup _canvasGroup;

        private int _separatorsCount = 0;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _leaderboardItemGameObject.SetActive(false);
            _inviteLeaderboardItemGameObject.SetActive(false);
            _separatorImage.gameObject.SetActive(false);
        }

        public void Show(List<UserLeaderboardData> dataList)
        {
            foreach (var userLeaderboardData in dataList)
            {
                CreateItem(userLeaderboardData);
                CreateSeparator();
            }

            CreateInviteItem();    

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            if (_items != null && _items.Any())
            {
                foreach (var item in _items)
                {
                    Destroy(item.gameObject);
                }

                _items.Clear();
            }

            _separatorsCount = 0;
        }

        private void CreateItem(UserLeaderboardData itemData)
        {
            var itemGameObject = Instantiate(_leaderboardItemGameObject);
            itemGameObject.transform.SetParent(_rootTransform);
            itemGameObject.transform.localScale = Vector3.one;
            itemGameObject.SetActive(true);
            var itemObject = itemGameObject.GetComponent<ScoresLeaderboardItem>();
            itemObject.SetItem(itemData);

            _items.Add(itemGameObject);
        }

        private void CreateInviteItem()
        {
            var itemGameObject = Instantiate(_inviteLeaderboardItemGameObject);
            itemGameObject.transform.SetParent(_rootTransform);
            itemGameObject.transform.localScale = Vector3.one;
            itemGameObject.SetActive(true);
            _items.Add(itemGameObject);
        }

        private void CreateSeparator()
        {
            if (_separatorsCount < 2)
            {
                var itemGameObject = Instantiate(_separatorImage.gameObject);
                itemGameObject.transform.SetParent(_rootTransform);
                itemGameObject.transform.localScale = Vector3.one;
                itemGameObject.SetActive(true);
                _items.Add(itemGameObject);
                _separatorsCount++;
            }
            
        }
    }
}