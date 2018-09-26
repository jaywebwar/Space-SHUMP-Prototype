using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy {
    //Enemy_3 will move following a Bezier curve, which is a linear
    //interpolation of more than two points
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;

	// Use this for initialization
	void Start () {
        points = new Vector3[3];

        points[0] = pos;

        float xMin = Utils.camBounds.min.x + Main.S.enemySpawnPadding;
        float xMax = Utils.camBounds.max.x + Main.S.enemySpawnPadding;

        Vector3 v;
        //assigns a point in the lower half of the map
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = Random.Range(Utils.camBounds.min.y, 0);
        points[1] = v;

        //assigns a random point above the screen
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        birthTime = Time.time;
	}

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //interpolate
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);//This speeds up the middle portion of the curve
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
