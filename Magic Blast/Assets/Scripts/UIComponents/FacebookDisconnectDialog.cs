using UnityEngine;

public class FacebookDisconnectDialog : MonoBehaviour
{
    public void AcceptDisconnect()
    {
        var fbManager = FacebookManager.Instance;
        if (fbManager != null && fbManager.IsInitialized && fbManager.IsLoggedIn)
        {
            fbManager.LogOutFacebook();
        }
    }
}
