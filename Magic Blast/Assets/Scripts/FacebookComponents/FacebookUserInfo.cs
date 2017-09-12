using System;
using UnityEngine;

public class FacebookUserInfo
{
    public string id = null;
    public string firstName = "Unknown";
    public string lastName = "";
    public string pictureUrl;

    private Sprite _profilePicture;

    public Action OnImageLoaded;

    public Sprite ProfilePicture
    {
        get { return _profilePicture; }
        set
        {
            _profilePicture = value;
            if (OnImageLoaded != null)
            {
                OnImageLoaded.Invoke();
            }
        }
    }
}