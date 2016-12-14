using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipBehaviour : MonoBehaviour {

	[SerializeField] public List<Sprite> images;

	private int NextImage = 0;
	void FlipCallback() {
		Debug.Log(NextImage);
		if (NextImage >= images.Count || NextImage < 0)
			return;

		GetComponent<Image>().sprite = images[NextImage];
	}

	public void FlipImage(int nextImage) {
		Debug.Log(nextImage);
		NextImage = nextImage;
		GetComponent<Animator>().Play("Flip");
	}
}
