using TMPro;
using UnityEngine;

public class InboxCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _counterText;

    private void Start()
    {
        if (_counterText == null)
        {
            _counterText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void Show(int withValue = 0)
    {
        gameObject.SetActive(true);
        UpdateValue(withValue);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateValue(int value)
    {
        _counterText.text = value.ToString();
    }
}
