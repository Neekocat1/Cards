
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;

public class DeckManager2 : UdonSharpBehaviour
{
    public ButtonManager buttonManager;
    public int deckCount;
    VRCObjectPool[] pools;
    public VRCObjectPool[] jokerPools;
    public VRCObjectPool[] normalPools;

    public GameObject deckObject;
    //private PlayingCard[] cardList;
    [UdonSynced] public int[] cardsRemaining;
    public int[] cardCount;
    GameObject currentCard;
    int REMAINING_CARD_VALUE;
    public Transform[] sendPositions;
    public Transform[] returnPositions;
    public float SHUFFLE_TIMER;
    public CardsToLine line;
    

    void Start()
    {
        pools = normalPools;
        cardsRemaining = new int[deckCount];
        REMAINING_CARD_VALUE = 52;
        //cardList = new PlayingCard[deckCount*REMAINING_CARD_VALUE];
        cardCount = new int[deckCount];
        for (int i = 0; i < deckCount; i++)
        {
            //for(int j = REMAINING_CARD_VALUE * i; j < REMAINING_CARD_VALUE * (i + 1); j++) //remove?
            //{
            //    cardList[j] = pools[i].Pool[j / (i + 1)].GetComponent<PlayingCard>();
            //}
            cardsRemaining[i] = REMAINING_CARD_VALUE;
            cardCount[i] = 0;
            pools[i].Shuffle();
        }
        //DrawCard(); //load problem?
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }
    
    public void ResetDeck()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    //Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
                pools[i].Shuffle();
                cardCount[i] = 0;
                cardsRemaining[i] = REMAINING_CARD_VALUE;
            }
            DrawCard();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "ResetDeck");
        }
        
    }
    public GameObject DrawCard() 
    {
        GameObject drawnCard = null;
        int emptyDecks = 0;
        for (int i = 0; i < deckCount; i++)
        {
            if (cardsRemaining[i] <= 0)
            {
                emptyDecks++;
            }
        }
        if (emptyDecks == deckCount)
        {
        }
        else
        {
            bool drawn = false;
            do
            {
                int drawnPool = Random.Range(0, deckCount);
                if (cardsRemaining[drawnPool] != 0)
                {
                    cardCount[drawnPool]++;
                    cardsRemaining[drawnPool]--;
                    RequestSerialization();
                    drawn = true;
                    if (!Networking.IsOwner(pools[drawnPool].gameObject))
                    {
                        Networking.SetOwner(Networking.LocalPlayer, pools[drawnPool].gameObject);
                    }
                    drawnCard = pools[drawnPool].TryToSpawn();
                    //Networking.SetOwner(Networking.LocalPlayer, drawnCard);

                    //cardList[cardCount[drawnPool] - 1] = drawnCard.GetComponent<PlayingCard>();

                    drawnCard.transform.position = deckObject.transform.position;
                    VRCObjectSync sync = (VRCObjectSync) drawnCard.GetComponent(typeof(VRCObjectSync));
                    if (sync) sync.FlagDiscontinuity();                   
                }
            } while (!drawn);
            
        }
        currentCard = drawnCard;
        return drawnCard;
    }
    public GameObject DrawCardNoSync() //Dont know if still needed or not. Will test more later.
    {
        GameObject drawnCard = null;
        int emptyDecks = 0;
        for (int i = 0; i < deckCount; i++)
        {
            if (cardsRemaining[i] <= 0)
            {
                emptyDecks++;
            }
        }
        if (emptyDecks == deckCount)
        {
        }
        else
        {
            bool drawn = false;
            do
            {
                int drawnPool = Random.Range(0, deckCount);
                if (cardsRemaining[drawnPool] != 0)
                {
                    cardCount[drawnPool]++;
                    cardsRemaining[drawnPool]--;
                    //RequestSerialization();
                    drawn = true;
                    if (!Networking.IsOwner(pools[drawnPool].gameObject))
                    {
                        Networking.SetOwner(Networking.LocalPlayer, pools[drawnPool].gameObject);
                    }
                    drawnCard = pools[drawnPool].TryToSpawn();
                    //Networking.SetOwner(Networking.LocalPlayer, drawnCard);

                    //cardList[cardCount[drawnPool] - 1] = drawnCard.GetComponent<PlayingCard>();

                    drawnCard.transform.position = deckObject.transform.position;
                    VRCObjectSync sync = (VRCObjectSync) drawnCard.GetComponent(typeof(VRCObjectSync));
                    if (sync) sync.FlagDiscontinuity();                   
                }
            } while (!drawn);
            
        }
        currentCard = drawnCard;
        return drawnCard;
    }
    void RefreshCards() //Reset deck but when card count is changed it gets fucky so second half is needed.
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    //Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
                pools[i].Shuffle();
                cardCount[i] = 0;
                cardsRemaining[i] = REMAINING_CARD_VALUE;
            }

            cardsRemaining = new int[deckCount];
            //cardList = new PlayingCard[deckCount*REMAINING_CARD_VALUE];
            cardCount = new int[deckCount];
            for (int i = 0; i < deckCount; i++)
            {
                //for(int j = REMAINING_CARD_VALUE * i; j < REMAINING_CARD_VALUE * (i + 1); j++)
                //{
                //    cardList[j] = pools[i].Pool[j / (i + 1)].GetComponent<PlayingCard>();
                //}
                cardsRemaining[i] = REMAINING_CARD_VALUE;
                cardCount[i] = 0;
                pools[i].Shuffle();
            }
            DrawCard();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "RefreshCards");
        }
    }
    public void ChangeDeckCount(int count)
    {
        if(Networking.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    //Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
                pools[i].Shuffle();
                cardCount[i] = 0;
                cardsRemaining[i] = REMAINING_CARD_VALUE;
            }
            deckCount = count;
            cardCount = new int[deckCount];
            cardsRemaining = new int[deckCount];
            RefreshCards();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "ChangeDeckCount");
        }
    }
    public void EnableJokers()
    {
        if (Networking.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    //Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
            }
            REMAINING_CARD_VALUE = 54;
            pools = jokerPools;
            RefreshCards();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "EnableJokers");
        }        
    }
    public void DisableJokers()
    {
        if (Networking.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                   //Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
            }
            REMAINING_CARD_VALUE = 52;
            pools = normalPools;
            RefreshCards();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "DisableJokers");
        }
    }
    public void AutoShuffle()
    {
        if (Networking.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
                pools[i].Shuffle();
                cardCount[i] = 0;
                cardsRemaining[i] = REMAINING_CARD_VALUE;
            }
            buttonManager.AddTime(2);
            Debug.Log("Adding time in Shuffle: " + 2);
            buttonManager.StartTimer();
            //SendCustomNetworkEvent(NetworkEventTarget.Owner, "AutoShuffleWait");
            AutoShuffleWait();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "AutoShuffle");
        }
        
    }
    public void AutoShuffleWait()
    {
        GameObject drawnCard = DrawCardNoSync(); //TODO: Clean up wait code so ownership doesn't break the cards.
        if (drawnCard != null)
        {
            drawnCard.GetComponent<PlayingCard>().LetCardLoad1((SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)));
            buttonManager.AddTime((SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)) * 2);
            //Debug.Log("Adding time in wait: " + (SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)));
        }         
    }
    public void AutoShuffleSend(GameObject drawnCard)
    {
        //GameObject drawnCard = DrawCardNoSync();   
        if (drawnCard != null)
        {
            Vector3 sendTarget = new Vector3(Random.Range(sendPositions[0].position.x, sendPositions[1].position.x), sendPositions[0].position.y, Random.Range(sendPositions[0].position.z, sendPositions[1].position.z));
            float angleOfLaunch = Random.Range(30, 46);
            Vector3 vectorToTarget = sendTarget - drawnCard.transform.position;
            float launchPower = Mathf.Sqrt((9.8f*vectorToTarget.magnitude)/(Mathf.Sin(2*(Mathf.Deg2Rad * angleOfLaunch))));
            float launchPowerHorizontal = launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch);       
            Vector3 launchVector = new Vector3(launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch) * Mathf.Sign(vectorToTarget.x), 0, 0); 
            Vector3 rotateTarget = new Vector3(vectorToTarget.x, 0, vectorToTarget.z);
            launchVector = Vector3.RotateTowards(launchVector, rotateTarget, Mathf.Deg2Rad * 360, 0);
            launchVector.y = launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch);
            float timeOfFlight = 2 * (launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch))/9.8f;
            float launchTimer = SHUFFLE_TIMER + timeOfFlight + 1;
            buttonManager.AddTime((SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)) / 3 / timeOfFlight);
            Debug.Log("Adding time in send: " + (SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)) / 3 / timeOfFlight);
            drawnCard.GetComponent<PlayingCard>().LaunchCard(launchVector, launchTimer);
        }           
    }
    public void AutoShuffleReturn(GameObject card)
    {
        Vector3 sendTarget = new Vector3(Random.Range(returnPositions[0].position.x, returnPositions[1].position.x), returnPositions[0].position.y, Random.Range(returnPositions[0].position.z, returnPositions[1].position.z));
        float angleOfLaunch = Random.Range(30, 46);
        Vector3 vectorToTarget = sendTarget - card.transform.position;
        float launchPower = Mathf.Sqrt((9.8f*vectorToTarget.magnitude)/(Mathf.Sin(2*(Mathf.Deg2Rad * angleOfLaunch))));
        float launchPowerHorizontal = launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch);       
        Vector3 launchVector = new Vector3(launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch) * Mathf.Sign(vectorToTarget.x), 0, 0); 
        Vector3 rotateTarget = new Vector3(vectorToTarget.x, 0, vectorToTarget.z);
        launchVector = Vector3.RotateTowards(launchVector, rotateTarget, Mathf.Deg2Rad * 360, 0);
        launchVector.y = launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch);
        float timeOfFlight = 2 * (launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch))/9.8f;
        //buttonManager.AddTime(timeOfFlight);
        //Debug.Log("Adding time in return: " + timeOfFlight);
        card.GetComponent<PlayingCard>().LaunchCard(launchVector, 0);
    }
    public void AutoSetShapeToLine()
    {
        if(Networking.IsOwner(gameObject))
        {
            for(int i = 0; i < deckCount; i++)
            {
                Networking.SetOwner(Networking.LocalPlayer, pools[i].gameObject);
                foreach(GameObject card in pools[i].Pool)
                {
                    Networking.SetOwner(Networking.LocalPlayer, card);
                    PlayingCard curCard = card.GetComponent<PlayingCard>();
                    curCard.drawn = false;
                    //curCard.RequestSerialization();
                    pools[i].Return(card);
                }
                pools[i].Shuffle();
                cardCount[i] = 0;
                cardsRemaining[i] = REMAINING_CARD_VALUE;
            }
            //Debug.Log("Starting Shape");
            line._SetSpacing(REMAINING_CARD_VALUE * deckCount);
            buttonManager.AddTime(1);
            Debug.Log("Adding time in Shape: " + 2);
            buttonManager.StartTimer();
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "AutoSetShapeToLineWait");
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "AutoSetShapeToLine");
        }
    }
    public void AutoSetShapeToLineWait()
    {
        GameObject drawnCard = DrawCardNoSync();

        if (drawnCard != null)
        {
            //Debug.Log("Waiting: " + drawnCard);
            buttonManager.AddTime((SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)));

            drawnCard.GetComponent<PlayingCard>().LetCardLoadLine1((SHUFFLE_TIMER / (REMAINING_CARD_VALUE * deckCount)));
        }         
    }
    public void AutoSetShapeToLineSend(GameObject drawnCard)
    {
        //GameObject drawnCard = DrawCardNoSync();   
        if (drawnCard != null)
        {
            //Debug.Log("Sending: " + drawnCard);
            line._LaunchCube(drawnCard);
        }           
    }

}
