
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EraseLine : UdonSharpBehaviour
{
    [SerializeField] TrailRenderer line;
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Erase");
    }
    public void Erase()
    {
        line.Clear();
    }
}
