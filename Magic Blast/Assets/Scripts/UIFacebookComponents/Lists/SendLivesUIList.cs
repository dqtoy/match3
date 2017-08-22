using System.Collections.Generic;
using System.Linq;

public class SendLivesUIList : BaseUIList<FacebookUserInfo>
{
    protected override void ActionCall()
    {
        base.ActionCall();
        var ids = new List<string>();
        var selectedToggles = GetSelectedToggles();
        foreach (var selectedToggle in selectedToggles)
        {
            var item = selectedToggle.GetComponentInParent<FriendsListItem>();
            ids.Add(item.UserInfo.id);
        }
        if (ids.Any())
        {
            FacebookManager.Instance.SendLives(ids);
        }
    }
}
