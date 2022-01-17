
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AutoShuffleButton : UdonSharpBehaviour
{
    public DeckManager2 deckManager;
    public ButtonManager buttonManager;
    void Start()
    {
        
    }

    public override void Interact()
    {
        if (!buttonManager.masterLockout)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "AutoShuffle");
        }

    }
    public void AutoShuffle()
    {
        deckManager.AutoShuffle();
    }
}
