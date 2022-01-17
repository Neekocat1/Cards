
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ChangeDeckAmount : UdonSharpBehaviour
{
    public int deckCount;
    public DeckManager2 deckManager;
    public ButtonManager buttonManager;
    [UdonSynced] public bool isOn = false;
    void Start()
    {
    }

    public override void Interact()
    {
        if (!buttonManager.masterLockout)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ChangeDeck");
        }
    }
    public void ChangeDeck()
    {
        deckManager.ChangeDeckCount(deckCount);
        buttonManager.ChangeButton(this);
    }
    public void SetRed()
    {
        gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOffMat;

    }
    public void SetGreen()
    {
        gameObject.GetComponent<Renderer>().material =  buttonManager.buttonOnMat;
    }
}
