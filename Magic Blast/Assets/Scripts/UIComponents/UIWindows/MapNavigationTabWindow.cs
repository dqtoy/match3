using System.Linq;
using Assets.Scripts.UIFriendsList.UI;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class MapNavigationTabWindow : TabWindow
    {
        [SerializeField]
        private MapNavigationItem[] _mapNavigationItems;

        private MapCamera _mapCamera;

        private void Start()
        {
            if (_mapNavigationItems != null && _mapNavigationItems.Any())
            {
                foreach (var mapNavigationItem in _mapNavigationItems)
                {
                    if (mapNavigationItem != null)
                    {
                        mapNavigationItem.LocationButton.onClick.AddListener(() => MoveCamera(mapNavigationItem.YTranslation));
                    }
                }
            }

            _mapCamera = GameObject.FindObjectOfType<MapCamera>();
        }

        private void MoveCamera(float yTranslation)
        {
            if (_mapCamera != null)
            {
                var movePosition = new Vector2(_mapCamera.transform.localPosition.x, yTranslation);
                _mapCamera.SetPosition(movePosition);
            }
        }
    }
}