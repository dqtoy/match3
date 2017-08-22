using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class ShowFPS: MonoBehaviour 
{
	public  float updateInterval = 0.5F;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	public Text fpsLabel;

	void Start()
	{
		timeleft = updateInterval;
	}

	void LateUpdate()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;

		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			fps = Mathf.Floor (fps);
			//string format = System.String.Format("{0} FPS",fps);

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;

            fpsLabel.text = fps.ToString();
			//fpsLabel.text = GameManager.getTimePassedForRateMeInt().ToString();
			//fpsLabel.Commit ();
		}
	}


}