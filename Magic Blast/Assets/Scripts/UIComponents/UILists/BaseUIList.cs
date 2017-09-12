using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIFriendsList;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUIList<T> : MonoBehaviour
{
    [SerializeField]
    protected GameObject _defaultListItem;
    [SerializeField]
    protected Transform _listRoot;
    [SerializeField]
    protected Toggle _selectAllToggle;
    [SerializeField]
    protected Button _actionButton;

    protected CustomToggleGroup _toggleGroup;

    protected List<IListItem> _listItems = new List<IListItem>();

    protected virtual void Awake()
    {
        if (_toggleGroup == null)
        {
            InitToggleGroup();
        }
    }

    protected virtual void Start()
    {
        if (_defaultListItem == null)
        {
            Debug.LogWarning("UIList: Default list item is not assigned!");
            return;
        }
        if (_selectAllToggle != null)
        {
            _selectAllToggle.onValueChanged.AddListener(SelectAllValueChanged);
            _selectAllToggle.isOn = _toggleGroup.AllTogglesSelected;
        }
        _defaultListItem.SetActive(false);

        if (_actionButton != null)
        {
            _actionButton.onClick.AddListener(ActionCall);
        }
    }

    protected virtual void ActionCall()
    {
    }

    protected virtual List<Toggle> GetSelectedToggles()
    {
        return _toggleGroup.GetSelectedToggles();
    }

    private void SelectAllValueChanged(bool selected)
    {
        if (_toggleGroup != null)
        {
            if (selected)
            {
                _toggleGroup.SelectAllToggles();
            }
            else
            {
                _toggleGroup.DeselectAllToggles();
            }
        }
    }
    private IListItem CreateListItem()
    {
        var listItem = Instantiate(_defaultListItem);
        listItem.transform.SetParent(_listRoot);
        listItem.transform.localScale = Vector3.one;
        listItem.SetActive(true);

        return listItem.GetComponent<IListItem>();
    }

    private void InitToggleGroup()
    {
        _toggleGroup = new CustomToggleGroup();
        _toggleGroup.AnyToggleChangedState += AnyToggleChangedState;
    }

    private void AnyToggleChangedState()
    {
        _selectAllToggle.isOn = false;
    }

    public virtual void ClearList()
    {
        if (_toggleGroup != null)
        {
            _toggleGroup.ClearToggles();
        }

        if (_listItems != null && _listItems.Any())
        {
            foreach (var friendsListItem in _listItems)
            {
                if (friendsListItem != null)
                {
                    if (friendsListItem.GameObject != null)
                    {
                        Destroy(friendsListItem.GameObject);
                    }
                }
            }
            _listItems.Clear();
        }
    }

    public virtual void UpdateList(List<T> itemsData, UIListType listType)
    {
        ClearList();
        if (itemsData != null && itemsData.Any())
        {
            foreach (var itemData in itemsData)
            {
                var newItem = CreateListItem();
                newItem.SetItem(itemData, listType);
                _listItems.Add(newItem);
                if (_toggleGroup == null)
                {
                    InitToggleGroup();
                }
                if (newItem.SelectionToggle)
                {
                    _toggleGroup.AddToggle(newItem.SelectionToggle);
                }
            }
        }
    }

    public virtual void RemoveListItem(IListItem item)
    {
        if (_listItems != null && _listItems.Contains(item))
        {
            var selectedItem = _listItems.First(li => li == item);
            Destroy(selectedItem.GameObject);
            _listItems.Remove(selectedItem);
        }
    }
}