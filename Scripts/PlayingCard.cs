
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;

public class PlayingCard : UdonSharpBehaviour
{
    public DeckManager2 Deck;
    [UdonSynced] public bool drawn;
    void Start()
    {
        
    }

    public override void OnPickup()
    {
        if (drawn)
            return;
        drawn = true;
        Deck.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DeckManager2.DrawCard));
    }
    public void LaunchCard(Vector3 launchVector, float timer)
    {
        GetComponent<Rigidbody>().AddForce(launchVector.x, launchVector.y, launchVector.z, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddRelativeTorque(Vector3.back * Random.Range(-5, 5), ForceMode.Impulse);
        if (timer != 0)
        {
            SendCustomEventDelayedSeconds("ReturnCard", timer);
        }
    }
    public void ReturnCard()
    {
        Deck.AutoShuffleReturn(gameObject);
    }
    public void LetCardLoad1(float timerIn)
    {
        drawn=true;
        SendCustomEventDelayedSeconds("LetCardLoad2", timerIn);
    }
    public void LetCardLoad2()
    {
        Deck.AutoShuffleWait();

        SendCustomEventDelayedSeconds("LetCardLoad3", 1);
    }
    public void LetCardLoad3()
    {
        Deck.AutoShuffleSend(gameObject);
    }

    public void LetCardLoadLine1(float timerIn)
    {
        drawn=true;
        SendCustomEventDelayedSeconds("LetCardLoadLine2", timerIn);
    }
    public void LetCardLoadLine2()
    {
        Deck.AutoSetShapeToLineWait();
        SendCustomEventDelayedSeconds("LetCardLoadLine3", 1);
    }
    public void LetCardLoadLine3()
    {
        Deck.AutoSetShapeToLineSend(gameObject);
    }
}
