using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonBehaviour : MonoBehaviour {

	[SerializeField]
	public Color normalColor = new Color(78, 109, 72);

	[SerializeField]
	public Color pressedColor = Color.white;

	public void OnButtonPress(Text text)
	{
		text.color = pressedColor;
	}

	public void OnButtonUp(Text text)
	{
		text.color = normalColor;
	}
}
