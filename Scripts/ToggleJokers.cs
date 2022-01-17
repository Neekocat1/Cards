
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleJokers : UdonSharpBehaviour
{
    public DeckManager2 deckManager;
    public ButtonManager buttonManager;
    [UdonSynced] bool isJokers;
    void Start()
    {
        isJokers = false;
    }

    public override void Interact()
    {
        if (!buttonManager.masterLockout)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ToggleJoker));
        }
    }
    public void ToggleJoker()
    {
        if(isJokers)
        {
            deckManager.DisableJokers();
            gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOffMat;
            isJokers = false;
        }
        else
        {
            deckManager.EnableJokers();
            gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOnMat;
            isJokers = true;
        }
    }
}
