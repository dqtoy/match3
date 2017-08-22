using System.Collections.Generic;
using ExaGames.Common;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class InboxUIList : BaseUIList<UserRequestInfo>
    {
        protected override void ActionCall()
        {
            base.ActionCall();
            var selectedRequests = new List<UserRequestInfo>();
            var selectedToggles = GetSelectedToggles();
            foreach (var selectedToggle in selectedToggles)
            {
                var reqComponent = selectedToggle.GetComponentInParent<InboxListItem>();
                selectedRequests.Add(reqComponent.RequestData);
            }
            if (selectedRequests.Count > 0)
            {
                var livesManager = FindObjectOfType<LivesManager>();
                if (livesManager != null)
                {
                    for (int i = 0; i < selectedRequests.Count; i++)
                    {
                        livesManager.GiveOneLife();
                    }
                }
                FacebookManager.Instance.ConfirmRequests(selectedRequests);
            }
        }
    }
}