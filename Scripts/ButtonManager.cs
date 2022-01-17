
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonManager : UdonSharpBehaviour
{
    public ChangeDeckAmount[] deckCountButtons;
    [UdonSynced] public bool[] deckCountEnabled;
    public GameObject jokerButton;
    public Material buttonOnMat;
    public Material buttonOffMat;
    [UdonSynced] public int currentButton;
    [UdonSynced] public bool masterLockout;
    public MasterLockoutButton lockoutButton;
    public float timerLength;
    public float timerCount;
    public bool isDone;

    void Start()
    {
        isDone = true;
        timerLength = 0;
        timerCount = 0;
        masterLockout = false;
        deckCountEnabled = new bool[4] {true, false, false, false};
        currentButton = 0;
        ToggleButtons();
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }
    void Update()
    {
        if (!isDone)
        {
            if (timerCount < timerLength)
            {
                timerCount += Time.deltaTime;
            }
            else
            {
                Debug.Log("Timer Done");
                isDone = true;
                timerLength = 0;
                masterLockout = false;
                RequestSerialization();
            }
        }
        
    }
    public void AddTime(float time)
    {
        timerLength += time;
    }

    public void StartTimer()
    {
        Debug.Log("Starting Timer");
        masterLockout = true;
        RequestSerialization();
        Debug.Log("Should be locked out: " + masterLockout);
        timerCount = 0;
        isDone = false;
    }
    public void ChangeButton(ChangeDeckAmount button)
    {
        if(Networking.IsOwner(gameObject))
        {
            //Networking.SetOwner(Networking.LocalPlayer, gameObject);
            deckCountEnabled[currentButton] = false;
            for (int i = 0; i < 4; i++)
            {
                if (button == deckCountButtons[i])
                {
                    deckCountEnabled[i] = true;
                    currentButton = i;
                }
            }
            RequestSerialization();
            ToggleButtons();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ChangeButton");
        }
        
    }
    public override void OnDeserialization()
    {
        ToggleButtons();
        lockoutButton.ToggleMats();
    }
    public void ToggleButtons()
    {
        if (deckCountEnabled[0])
        {
            deckCountButtons[0].SetGreen();
        }
        else
        {
            deckCountButtons[0].SetRed();
        }
        if (deckCountEnabled[1])
        {
            deckCountButtons[1].SetGreen();
        }
        else
        {
            deckCountButtons[1].SetRed();
        }
        if (deckCountEnabled[2])
        {
            deckCountButtons[2].SetGreen();
        }
        else
        {
            deckCountButtons[2].SetRed();
        }
        if (deckCountEnabled[3])
        {
            deckCountButtons[3].SetGreen();
        }
        else
        {
            deckCountButtons[3].SetRed();
        }
    }
    public void ToggleLockout()
    {
        if (Networking.IsOwner(gameObject))
        {
            masterLockout = !masterLockout;
            RequestSerialization();
            lockoutButton.ToggleMats();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ToggleLockout");
        }
    }
}
