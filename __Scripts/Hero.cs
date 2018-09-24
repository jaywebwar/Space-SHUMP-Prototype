using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

    static public Hero S;

    //inspector
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float shieldLevel = 1;

    bool ___________________________________________;

    private void Awake()
    {
        S = this;//set singleton
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

        //rotate ship to make movement feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
	}
}
