using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayFab.ClientModels;

public class TournamentLeaderboard : MonoBehaviour {

	// Use this for initialization
	public ScrollRect _scroll;
	public Transform _contentObj;
	public GameObject tournamentPlayerPrefab;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void clearLeaderbordObjects()
	{
		int childs = _contentObj.childCount;
		for (int i = childs - 1; i > 0; i--)
		{
			GameObject.Destroy(_contentObj.GetChild(i).gameObject);
		}
	}

	public void displayLeaderboard(PlayFab.ClientModels.GetSharedGroupDataResult _result)
	{
		clearLeaderbordObjects ();
		string[] keys = new string[_result.Data.Keys.Count];
		_result.Data.Keys.CopyTo(keys, 0);

		List<KeyValuePair<string,SharedGroupDataRecord >> myList = _result.Data.ToList();

		myList.Sort(
			delegate(KeyValuePair<string, SharedGroupDataRecord> pair1,
				KeyValuePair<string, SharedGroupDataRecord> pair2)
			{
				return pair2.Value.Value.CompareTo(pair1.Value.Value);
			}
		);

		_result.Data = myList.ToDictionary (x => x.Key, x => x.Value);

		int counter = 0;
		var enumerator = _result.Data.GetEnumerator();
		while( enumerator.MoveNext() )
		{
			string currentId = enumerator.Current.Key;

			GameObject go = (GameObject)Instantiate (tournamentPlayerPrefab, _contentObj);
			go.GetComponent<RectTransform>().localPosition = new Vector3 (0f,-73f - counter*122f,0);

			string currentScore = enumerator.Current.Value.Value;

			go.GetComponent <TournamentPlayer>().displayPlayer(counter+1,currentScore,currentId);
			// Access value with enumerator.Current.Value;
			counter++;
		}

		/*for (int i=0;i<_result.Data.Count;i++)
		{
			string currentId = _result.Data[keys[i]].ToString();

			GameObject go = (GameObject)Instantiate (tournamentPlayerPrefab, _contentObj);
			go.GetComponent<RectTransform>().localPosition = new Vector3 (0f,-73f - i*122f,0);

			string currentScore = _result.Data [currentId].Value;

			go.GetComponent <TournamentPlayer>().displayPlayer(i+1,currentScore,currentId);
		}*/
		//_scroll.Rebuild (CanvasUpdate.Layout);
		//_scroll.CalculateLayoutInputVertical ();
	}
}
