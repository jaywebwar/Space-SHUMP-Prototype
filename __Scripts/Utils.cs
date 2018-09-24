using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

	//creates bounds that encapsulate the two bounds passed in
    public static Bounds BoundsUnion(Bounds b0, Bounds b1)
    {
        //if the size of one of the bounds is Vector3.zero, ignore it
        if(b0.size == Vector3.zero && b1.size != Vector3.zero)
        {
            return b1;
        }
        else if(b0.size != Vector3.zero && b1.size == Vector3.zero)
        {
            return b0;
        }
        else if(b0.size == Vector3.zero && b1.size == Vector3.zero)
        {
            return b0;
        }
        //stretch b0 to include the b1.min and b1.max
        b0.Encapsulate(b1.min);
        b0.Encapsulate(b1.max);
        return b0;
    }

    public static Bounds CombineBoundsOfChildren(GameObject go)
    {
        //create an empty bounds b
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);

        //if this go has a renderer component, expand b to contain the renderer's bounds
        if(go.GetComponent<Renderer>() != null)
        {
            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
        }

        //if this go has a collider component, expand b to contain the collider's bounds
        if(go.GetComponent<Collider>() != null)
        {
            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
        }

        //recursively iterate through each child of this gameobject.transform
        foreach(Transform t in go.transform)
        {
            b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
        }
        return b;
    }
}
