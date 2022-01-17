
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DeckReset : UdonSharpBehaviour
{
    public DeckManager2 deckReference;
    public ButtonManager buttonManager;
    void Start()
    {
        
    }

    public override void Interact()
    {
        if (!buttonManager.masterLockout)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ResetFunc");
        }
    }

    public void ResetFunc()
    {
        deckReference.ResetDeck();
    }
}
