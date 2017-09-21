using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookFeaturesStartController : MonoBehaviour
{
    private void Start()
    {
        if (FacebookManager.Instance != null && FacebookManager.Instance.IsLoggedIn)
        {
            var rnd = UnityEngine.Random.Range(0, 1000);
            if (rnd > 500)
            {

                FacebookManager.Instance.OnInvintableFriendsInfoDownloadedEvent += InviteDialog;
            }
            else
            {
                FacebookManager.Instance.OnFriendsInfoDownloadedEvent += SendLivesDialog;
            }
        }
    }

    public void InviteDialog()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        GameObject.Find("CanvasGlobal").transform.Find("InviteFriendsDialogWindow").gameObject.SetActive(true);
    }

    public void SendLivesDialog()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        GameObject.Find("CanvasGlobal").transform.Find("SendLivesDialogWindow").gameObject.SetActive(true);
    }
}
