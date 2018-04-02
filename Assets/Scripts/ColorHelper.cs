using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHelper : MonoBehaviour {

	public Material blackMaterial;
	public static Material black;

	public Material greenMaterial;
	public static Material green;

	// I added a red material here
	public Material redMaterial;
	public static Material red;

	void Start () {
		ColorHelper.black = blackMaterial;
		ColorHelper.green = greenMaterial;
		ColorHelper.red = redMaterial;
	}
}
