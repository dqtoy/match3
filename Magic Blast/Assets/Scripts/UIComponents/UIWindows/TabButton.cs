using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabButton : MonoBehaviour
{
    [SerializeField]
    private Image _defaultSprite;
    [SerializeField]
    private Image _selectedSprite;

    public Button ControlButton
    {
        get { return GetComponent<Button>(); }
    }

    public void SetCurrent(bool current)
    {
        if (current)
        {
            _defaultSprite.gameObject.SetActive(false);
            _selectedSprite.gameObject.SetActive(true);
        }
        else
        {
            _defaultSprite.gameObject.SetActive(true);
            _selectedSprite.gameObject.SetActive(false);
        }
    }
}