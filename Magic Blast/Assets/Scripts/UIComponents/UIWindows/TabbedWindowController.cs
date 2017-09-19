using System;
using System.Linq;
using Assets.Scripts.UIFriendsList.UI;
using UnityEngine;
using UnityEngine.UI;

public class TabbedWindowController : MonoBehaviour
{
    [SerializeField]
    private RectTransform _tabsRoot;
    [SerializeField]
    private float _openSpeed = 1f;
    [SerializeField]
    private Image _backgroundVeil;
    [SerializeField]
    private TabWindow[] _tabWindows;
    [SerializeField]
    private TabButtonSettings[] _tabButtons;

    private TabWindow _currentTabWindow;
    private TabButton _currentTabButton;

    private CanvasGroup _tabsRootCanvasGroup;
    private CanvasGroup _backgroundVeilCanvasGroup;

    private FacebookManager _facebookManager;

    private bool _isOpened = false;

	private void Start ()
	{
        _facebookManager = FacebookManager.Instance;

        _tabsRootCanvasGroup = _tabsRoot.GetComponent<CanvasGroup>();
	    _backgroundVeilCanvasGroup = _backgroundVeil.GetComponent<CanvasGroup>();

        _backgroundVeil.GetComponent<Button>().onClick.AddListener(CloseWindow);

        foreach (var tabWindow in _tabWindows)
        {
            tabWindow.Hide();
        }

        foreach (var tabButton in _tabButtons)
        {
            var button = tabButton;
            button.Button.ControlButton.onClick.AddListener(()=>OnTabButtonClicked(button));
        }

	    if (_facebookManager == null || !_facebookManager.IsInitialized || !_facebookManager.IsLoggedIn)
	    {
	        _tabButtons[2].Button.ControlButton.gameObject.SetActive(false);
	    }

	    if (_facebookManager != null)
	    {
            _facebookManager.OnFbLoggedIn += OnFbLoggedIn;
            _facebookManager.OnFbLoggedOut += OnFbLoggedOut;
        }
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWindow();
        }
    }

    private void OnFbLoggedOut()
    {
        _tabButtons[2].Button.ControlButton.gameObject.SetActive(false);
    }

    private void OnFbLoggedIn()
    {
        _tabButtons[2].Button.ControlButton.gameObject.SetActive(true);
    }

    private void OnTabButtonClicked(TabButtonSettings button)
    {
        if (button != null)
        {
            if (!_isOpened)
            {
                OpenWindow(button.TabId);
            }
            else
            {
                if (_currentTabWindow.WindowId == button.TabId)
                {
                    CloseWindow();
                    if (_currentTabButton != null)
                    {
                        _currentTabButton.SetCurrent(false);
                    }
                }
                else
                {
                    SwitchWindow(button.TabId);
                }
            }
        }
    }

    private void OpenWindow(string withTab)
    {
        LevelsMap.SetClickEnabled(false);
        var newPosition = new Vector2(-_tabsRoot.rect.width, _tabsRoot.localPosition.y);
        LeanTween.move(_tabsRoot, newPosition, _openSpeed);
        LeanTween.alphaCanvas(_backgroundVeilCanvasGroup, 1, _openSpeed);
        _backgroundVeilCanvasGroup.blocksRaycasts = true;
        _backgroundVeilCanvasGroup.interactable = true;
        _isOpened = true;
        SwitchWindow(withTab);
    }

    private void CloseWindow()
    {
        LevelsMap.SetClickEnabled(true);
        var newPosition = new Vector2(0, _tabsRoot.localPosition.y);
        LeanTween.move(_tabsRoot, newPosition, _openSpeed).setOnComplete(OnCloseComplete);
        LeanTween.alphaCanvas(_backgroundVeilCanvasGroup, 0, _openSpeed);
        _backgroundVeilCanvasGroup.blocksRaycasts = false;
        _backgroundVeilCanvasGroup.interactable = false;
        if (_currentTabButton != null)
        {
            _currentTabButton.SetCurrent(false);
        }
        _isOpened = false;
    }

    private void OnCloseComplete()
    {
        if (_currentTabWindow != null)
        {
            _currentTabWindow.Hide();
        }
    }

    private void SwitchWindow(string windowId)
    {
        if (!string.IsNullOrEmpty(windowId) && _tabWindows.Any(tw => tw.WindowId == windowId))
        {
            if (_currentTabButton != null)
            {
                _currentTabButton.SetCurrent(false);
            }

            _currentTabButton = _tabButtons.First(tb => tb.TabId == windowId).Button;

            _currentTabButton.SetCurrent(true);

            if (_currentTabWindow != null)
            {
                _currentTabWindow.Hide();
            }

            _currentTabWindow = _tabWindows.First(tw => tw.WindowId == windowId);

            _currentTabWindow.Show();
        }
    }

    public void HideTabs()
    {
        if (_tabsRootCanvasGroup != null)
        {
            _tabsRootCanvasGroup.alpha = 0;
            _tabsRootCanvasGroup.blocksRaycasts = false;
            _tabsRootCanvasGroup.interactable = false;
        }
        else
        {
            _tabsRoot.gameObject.SetActive(false);
        }
    }

    public void ShowTabs()
    {
        if (_tabsRootCanvasGroup != null)
        {
            _tabsRootCanvasGroup.alpha = 1;
            _tabsRootCanvasGroup.blocksRaycasts = true;
            _tabsRootCanvasGroup.interactable = true;
        }
        else
        {
            _tabsRoot.gameObject.SetActive(true);
        }
    }
}