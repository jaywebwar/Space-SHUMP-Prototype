using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    //inspector
    public float rotationsPerSecond = 0.1f;

    public bool ____________________________________;

    //dynamic
    public int levelShown = 0;

	// Update is called once per frame
	void Update () {
        //read current shield level from Hero singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);

        //if different from level shown, adjust the texture to proper level
        if(levelShown != currLevel)
        {
            levelShown = currLevel;
            Material mat = this.GetComponent<Renderer>().material;
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        //rotate the shield about the ship
        float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
	}
}
