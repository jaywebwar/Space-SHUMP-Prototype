using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;

    public bool _____________________________;

    public Bounds bounds;
    public Vector3 boundsCenterOffset;

    private void Awake()
    {
        InvokeRepeating("CheckOffscreen", 0f, 2f);
    }

    // Update is called once per frame
    void Update () {
        Move();
	}

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void CheckOffscreen()
    {
        //if bounds are still their default value...
        if(bounds.size == Vector3.zero)
        {
            //then set them
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);
            //find the diff between bounds.center and transform.position
            boundsCenterOffset = bounds.center - transform.position;
        }

        //Every time, update the bounds to the current position
        bounds.center = transform.position + boundsCenterOffset;
        //Check to see if bounds are completely offscreen
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        if(off != Vector3.zero)
        {
            //if the enemy has gone off the bottom edge...
            if(off.y < 0)
            {
                //destroy it
                Destroy(this.gameObject);
            }
        }
    }
}
