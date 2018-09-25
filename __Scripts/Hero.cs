using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

    static public Hero S;

    //inspector
    public float gameRestartDelay = 2f;
    //controls movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    private float _shieldLevel = 1;

    bool ___________________________________________;

    public Bounds bounds;
    public delegate void WeaponFireDelegate();//creates the delegate type
    public WeaponFireDelegate fireDelegate;//creates the delegate variable of that type

    private void Awake()
    {
        S = this;//set singleton
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        //get coordinate info from input (This works for wasd, arrows, and a joystick)
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //change position based on axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        //Keep ship constrained to the screen
        bounds.center = transform.position;
        Vector3 offset = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
        if(offset != Vector3.zero)
        {
            pos -= offset;
            transform.position = pos;
        }

        //rotate ship to make movement feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //Use the fireDelegate to fire weapons
        //Axis("Jump") is our trigger, and we want to make sure fireDelegate is set
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
	}

    //This variable holds reference to the last triggering object
    public GameObject lastTriggerGO = null;

    private void OnTriggerEnter(Collider other)
    {
        //Find tag of other
        GameObject go = Utils.FindTaggedParent(other.gameObject);
        if(go != null)
        {
            //make sure it's not the same as last time
            if (go == lastTriggerGO)
                return;
            lastTriggerGO = go;
            if(go.tag == "Enemy")
            {
                //decrease shield level and destroy the enemy
                shieldLevel--;
                Destroy(go);
            }
            else
            {
                print("Triggered: " + go.name);
            }
        }
        else
        {
            print("Triggered: " + other.gameObject.name);
        }
    }

    public float shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if(value < 0)
            {
                Destroy(this.gameObject);//destroy ship
                //tell Main to restart the game after delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
