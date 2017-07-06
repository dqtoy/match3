using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTargetIcon : MonoBehaviour
{
    private int num;
    public Sprite[] targetSprite;
    private Target tar;
	private Target tar2;
	private Target tar3;

	private List<Target> targets = new List<Target> ();

    private LIMIT limitType;
    void OnEnable()
    {
        StartCoroutine(loadTarget());
    }

    IEnumerator loadTarget()
    {
        num = int.Parse(transform.parent.name.Replace("Level", ""));
        LoadLevel(num);
        yield return new WaitForSeconds(0.1f);
       // if (limitType == LIMIT.TIME)
            //GetComponent<SpriteRenderer>().sprite = targetSprite[4];
        //else
           GetComponent<SpriteRenderer>().sprite = targetSprite[(int)tar];

    }

    void LoadLevel(int n)
    {
		targets.Clear ();
		targets.TrimExcess ();
        TextAsset map = Resources.Load("Levels/" + n) as TextAsset;
        if (map != null)
        {
            string mapText = map.text;
            string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int mapLine = 0;
            foreach (string line in lines)
            {
                //check if line is game mode line
				if (line.Contains("MODE "))
                {
                    string modeString = line.Replace("MODE", string.Empty).Trim();
                    tar = (Target)int.Parse(modeString);
					targets.Add (tar);
                }
				else if (line.Contains("MODE2 "))
				{
					string modeString = line.Replace("MODE2", string.Empty).Trim();
					tar2 = (Target)int.Parse(modeString);
					targets.Add (tar2);
				}
				else if (line.Contains("MODE3 "))
				{
					string modeString = line.Replace("MODE3", string.Empty).Trim();
					tar3 = (Target)int.Parse(modeString);
					targets.Add (tar3);
				}
				else if (line.Contains("LIMIT"))
                {
                    string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                    string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
					limitType = LIMIT.MOVES;
                }

            }
        }

    }

    void Update()
    {

    }
}
