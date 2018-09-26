using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {
    //Enemy_2 uses a Sin wave to modify a 2-point linear interpolation
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;
    public float sinEccentricity = 0.6f;//determines how much sin wave affects movement

	// Use this for initialization
	void Start () {
        points = new Vector3[2];

        Vector3 cbMin = Utils.camBounds.min;
        Vector3 cbMax = Utils.camBounds.max;

        //Pick a random point on the left side of the screen
        Vector3 v = Vector3.zero;
        v.x = cbMin.x - Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[0] = v;

        //Pick a random point on the right side of the screen
        v = Vector3.zero;
        v.x = cbMax.x + Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[1] = v;

        //Possibly swap sides
        if(Random.value < 0.5f)
        {
            points[0].x *= -1;
            points[1].x *= -1;
        }
        birthTime = Time.time;
	}

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        if(u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //Adjust u by adding an easing curve based on a sine wave
        u = u + sinEccentricity * Mathf.Sin(u * Mathf.PI * 2);

        //interpolate
        pos = (1 - u) * points[0] + u * points[1];
    }
}
