using System.Collections.Generic;
using System.Linq;
using ExaGames.Common;
using UnityEngine;

namespace Assets.Scripts.UIFriendsList
{
    public class InboxUIList : BaseUIList<UserRequestInfo>
    {
        protected override void ActionCall()
        {
            base.ActionCall();
            var livesManager = FindObjectOfType<LivesManager>();
            
            var selectedItems = new List<InboxListItem>();

            var selectedLifeRequests = new List<UserRequestInfo>();
            var selectedRequests = new List<UserRequestInfo>();

            var selectedToggles = GetSelectedToggles();
            foreach (var selectedToggle in selectedToggles)
            {
                var reqComponent = selectedToggle.GetComponentInParent<InboxListItem>();
                if (reqComponent.RequestData.Type.Equals(RequestType.RequestLife))
                {
                    selectedLifeRequests.Add(reqComponent.RequestData);
                }
                else if (reqComponent.RequestData.Type.Equals(RequestType.SendLife))
                {
                    selectedRequests.Add(reqComponent.RequestData);
                }
                selectedItems.Add(reqComponent);
            }
            if (selectedRequests.Count > 0)
            {
                if (livesManager != null)
                {
                    if (!livesManager.ifFullLives())
                    {
                        for (int i = 0; i < selectedRequests.Count; i++)
                        {
                            livesManager.GiveOneLife();
                        }
                        FacebookManager.Instance.ConfirmRequests(selectedRequests);
                    }
                }
            }

            if (selectedLifeRequests.Count > 0)
            {
                List<string> userIds = new List<string>();
                foreach (var selectedLifeRequest in selectedLifeRequests)
                {
                    userIds.Add(selectedLifeRequest.Id);
                }
                FacebookManager.Instance.SendLives(userIds);
            }

            foreach (var inboxListItem in selectedItems)
            {
                RemoveListItem(inboxListItem);
            }
        }
    }
}