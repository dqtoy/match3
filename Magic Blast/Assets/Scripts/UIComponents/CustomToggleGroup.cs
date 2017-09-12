using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Assets.Scripts.UIFriendsList
{
    public class CustomToggleGroup
    {
        private List<Toggle> _toggles = new List<Toggle>();

        public bool AllTogglesSelected
        {
            get { return _toggles != null &&_toggles.All(t => t.isOn); }
        }

        public List<Toggle> GetSelectedToggles()
        {
            return _toggles.FindAll(t => t.isOn);
        }

        public List<Toggle> GetAllTogles()
        {
            return _toggles;
        }

        public Action AnyToggleChangedState;

        public void SelectAllToggles()
        {
            foreach (var toggle in _toggles)
            {
                toggle.isOn = true;
            }
        }

        public void DeselectAllToggles()
        {
            foreach (var toggle in _toggles)
            {
                toggle.isOn = false;
            }
        }

        public void AddToggle(Toggle toggle)
        {
            if (!_toggles.Contains(toggle))
            {
                _toggles.Add(toggle);
                toggle.onValueChanged.AddListener(ToggleStateChanged);
            }
        }

        private void ToggleStateChanged(bool arg0)
        {
            if (AnyToggleChangedState != null)
            {
                //AnyToggleChangedState.Invoke();
            }
        }

        public void RemoveToggle(Toggle toggle)
        {
            if (_toggles.Contains(toggle))
            {
                _toggles.Remove(toggle);
            }
        }

        public void ClearToggles()
        {
            if (_toggles != null)
            {
                _toggles.Clear();
            }
        }
    }
}