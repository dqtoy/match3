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
        _selectAllToggle.onValueChanged.AddListener(SelectAllValueChanged);
        _selectAllToggle.isOn = _toggleGroup.AllTogglesSelected;
        _defaultListItem.SetActive(false);

        _actionButton.onClick.AddListener(ActionCall);
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
        if (_listItems != null && _listItems.Any())
        {
            foreach (var friendsListItem in _listItems)
            {
                Destroy(friendsListItem.GameObject);
            }
            _listItems.Clear();
        }
        if (_toggleGroup != null)
        {
            _toggleGroup.ClearToggles();
        }
    }

    public virtual void UpdateList(List<T> itemsData, UIListType listType)
    {
        ClearList();

        foreach (var itemData in itemsData)
        {
            var newItem = CreateListItem();
            newItem.SetItem(itemData, listType);
            _listItems.Add(newItem);
            if (_toggleGroup == null)
            {
                InitToggleGroup();
            }
            _toggleGroup.AddToggle(newItem.SelectionToggle);
        }
    }

}