using UnityEngine;
using UnityEngine.UI;

public interface IListItem
{
    GameObject GameObject { get; }
    Toggle SelectionToggle { get; }

    void SetItem(object data, UIListType itemtype);
}