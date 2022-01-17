
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LineDistTest : UdonSharpBehaviour
{
    public Vector3 origin;
    public TrailRenderer line;
    bool side;
    void Start()
    {
        side = true;
        for (int i = 0; i < 10; i++)
        {
            for (float j = 0; j < 1; j += .01f)
            {   
                if (side)
                {
                    line.AddPosition(new Vector3(origin.x + Mathf.Log10(i * .1f) , origin.y, origin.z + j));
                }
                else
                {
                    line.AddPosition(new Vector3(origin.x + Mathf.Log10(i * .1f), origin.y, origin.z - j + 1));
                }             
            }
            Debug.Log("Gap " + i + ": " + Mathf.Log10(i * .1f));
        }
    }
}
