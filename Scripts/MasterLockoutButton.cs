
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MasterLockoutButton : UdonSharpBehaviour
{

    public ButtonManager buttonManager;

    void Start()
    {
        
    }

    public override void Interact()
    {
        if (Networking.IsMaster)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ToggleButton");
        }
    }
    public void ToggleButton()
    {
        buttonManager.ToggleLockout();
    }
    public void ToggleMats()
    {
        if(buttonManager.masterLockout)
        {
            gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOnMat;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOffMat;
        }
    }
}
