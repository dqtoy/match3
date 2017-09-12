using System;
using UnityEngine;
using System.Collections;
using TMPro;


public class GUIEvents : MonoBehaviour
{

    void Update()
    {
        if (name == "FaceBook" || name == "Share")
        {
            if (!LevelManager.THIS.FacebookEnable)
                gameObject.SetActive(false);
        }

        if (name == "FaceBook")
        {
            if (FacebookManager.Instance != null)
            {
                if (FacebookManager.Instance.IsLoggedIn)
                {
                    GetComponentInChildren<TextMeshProUGUI>().text = "LogOut";
                }
                else
                {
                    GetComponentInChildren<TextMeshProUGUI>().text = "LogIn";
                }
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

    public void ShowInboxDialog()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        GameObject.Find("CanvasGlobal").transform.Find("InboxWindow").gameObject.SetActive(true);
    }

    public void Settings(string name = "")
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.SetActive(true);

    }
    public void Play()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        transform.Find("Loading").gameObject.SetActive(true);
        Application.LoadLevel("game");
    }

    public void Pause()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

		if (LevelManager.THIS.gameStatus == GameState.Playing) {
			if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
				GameObject.Find ("CanvasGlobal").transform.Find ("PreQuitTreeClamb").gameObject.SetActive (true);
			} else {
				GameObject.Find ("CanvasGlobal").transform.Find ("MenuPause").gameObject.SetActive (true);
			}

		}

    }

    public void FaceBookLogin()
    {
#if FACEBOOK
        if (FacebookManager.Instance.IsLoggedIn)
        { 
            FacebookManager.Instance.LogOutFacebook();
        }
        else
        {
            FacebookManager.Instance.LogInFacebook();
        }
        //InitScript.Instance.CallFBLogin();
#endif
    }
    public void Share()
    {
#if FACEBOOK

        InitScript.Instance.Share();
#endif
    }

}
