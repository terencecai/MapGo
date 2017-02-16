using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {

	public Action OnZoomStarted;
	public Action OnZoomEnded;

	void Start () {
		
	}

	void StartCallback()
	{
		if (OnZoomStarted != null)
		{
			OnZoomStarted();
		}
	}

	void EndCallback()
	{
		if (OnZoomEnded != null)
		{
			OnZoomEnded();
		}
	}
}
