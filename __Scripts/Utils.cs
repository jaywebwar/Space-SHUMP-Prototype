using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoundsTest
{
    center,     //is the center of the go on screen?
    onScreen,   //are the bounds entirely on screen?
    offScreen   //are the bounds entirely off screen?
}

public class Utils : MonoBehaviour {

    //=================================================== Bounds Functions ====================================================================

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

    //read-only property for camera bounds
    static public Bounds camBounds
    {
        get
        {
            //if _camBounds hasn't been set, set them using the default camera
            if(_camBounds.size == Vector3.zero)
            {
                SetCameraBounds();
            }
            return _camBounds;
        }
    }

    static private Bounds _camBounds;

    public static void SetCameraBounds(Camera cam = null)
    {
        //if no camera was passed in, use the main camera
        if (cam == null) cam = Camera.main;

        //Two assumptions are made for this method
        //1) Camera is Orthographic
        //2) Camera is at rotation R[0,0,0]

        //Make vector3's at the topLeft and BotRight of the screen coordinates
        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 botRight = new Vector3(Screen.width, Screen.height, 0);

        //covnert to worlds coords
        Vector3 boundsTLN = cam.ScreenToWorldPoint(topLeft);
        Vector3 boundsBRF = cam.ScreenToWorldPoint(botRight);
        //adjust z's to near and far
        boundsTLN.z += cam.nearClipPlane;
        boundsBRF.z += cam.farClipPlane;

        //find the center of the bounds
        Vector3 center = (boundsTLN + boundsBRF) / 2f;
        _camBounds = new Bounds(center, Vector3.zero);

        //expand bounds to encapsulate the entents
        _camBounds.Encapsulate(boundsTLN);
        _camBounds.Encapsulate(boundsBRF);
    }

    public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center)
    {
        return (BoundsInBoundsCheck(camBounds, bnd, test));
    }

    //checks to see if lilB are within bigB
    public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen)
    {
        //behavior depends on BoundsTest, selected onScreen by default

        Vector3 pos = lilB.center;
        Vector3 offset = Vector3.zero;

        switch (test)
        {
            //center test dertermine what offset is require to move lilB's center back into bigB
            case BoundsTest.center:
                if (bigB.Contains(pos))
                {
                    return Vector3.zero;
                }
                if(pos.x > bigB.max.x)
                {
                    offset.x = pos.x - bigB.max.x;
                }
                else if(pos.x < bigB.min.x)
                {
                    offset.x = pos.x - bigB.min.x;
                }
                if(pos.y > bigB.max.y)
                {
                    offset.y = pos.y - bigB.max.y;
                }
                else if(pos.y < bigB.min.y)
                {
                    offset.y = pos.y - bigB.min.y;
                }
                if(pos.z > bigB.max.z)
                {
                    offset.z = pos.z - bigB.max.z;
                }
                else if(pos.z < bigB.min.z)
                {
                    offset.z = pos.z - bigB.min.z;
                }
                return offset;

            //the onScreen test determines what offset would have to be to keep all of lilB inside of bigB
            case BoundsTest.onScreen:
                if(bigB.Contains(lilB.min) && bigB.Contains(lilB.max))
                {
                    return Vector3.zero;
                }

                if(lilB.max.x > bigB.max.x)
                {
                    offset.x = lilB.max.x - bigB.max.x;
                }
                else if(lilB.min.x < bigB.min.x)
                {
                    offset.x = lilB.min.x - bigB.min.x;
                }
                if (lilB.max.y > bigB.max.y)
                {
                    offset.y = lilB.max.y - bigB.max.y;
                }
                else if (lilB.min.y < bigB.min.y)
                {
                    offset.y = lilB.min.y - bigB.min.y;
                }
                if (lilB.max.z > bigB.max.z)
                {
                    offset.z = lilB.max.z - bigB.max.z;
                }
                else if (lilB.min.z < bigB.min.z)
                {
                    offset.z = lilB.min.z - bigB.min.z;
                }
                return offset;

            //the offScreen test determines what offset would need to be to get any of lilB into bigB
            case BoundsTest.offScreen:
                bool cMin = bigB.Contains(lilB.min);
                bool cMax = bigB.Contains(lilB.max);
                if(cMin || cMax)
                {
                    return Vector3.zero;
                }

                if(lilB.min.x > bigB.max.x)
                {
                    offset.x = lilB.min.x - bigB.max.x;
                }
                else if(lilB.max.x < bigB.min.x)
                {
                    offset.x = lilB.max.x - bigB.min.x;
                }
                if (lilB.min.y > bigB.max.y)
                {
                    offset.y = lilB.min.y - bigB.max.y;
                }
                else if (lilB.max.y < bigB.min.y)
                {
                    offset.y = lilB.max.y - bigB.min.y;
                }
                if (lilB.min.z > bigB.max.z)
                {
                    offset.z = lilB.min.z - bigB.max.z;
                }
                else if (lilB.max.z < bigB.min.z)
                {
                    offset.z = lilB.max.z - bigB.min.z;
                }
                return offset;
        }
        return Vector3.zero;
    }


    //===================================================== Transform Functions =========================================================

    //This function climbs up the parent tree until it finds parent "Untagged" or no parent
    public static GameObject FindTaggedParent(GameObject go)
    {
        //if this go has a tag...
        if(go.tag != "Untagged")
        {
            return go;
        }
        //if there is no parent of this transform...
        if(go.transform.parent == null)
        {
            //We've reached the top without an interesting tag
            return null;
        }
        //Otherwise recursively climb up the tree
        return FindTaggedParent(go.transform.parent.gameObject);
    }
    //Overload for transform
    public static GameObject FindTaggedParent(Transform t)
    {
        return FindTaggedParent(t.gameObject);
    }

    //===================================================== Materials Functions =========================================================
    static public Material[] GetAllMaterials(GameObject go)
    {
        List<Material> mats = new List<Material>();
        if(go.GetComponent<Renderer>() != null)
        {
            mats.Add(go.GetComponent<Renderer>().material);
        }
        foreach(Transform t in go.transform)
        {
            mats.AddRange(GetAllMaterials(t.gameObject));
        }
        return mats.ToArray();
    }
}
