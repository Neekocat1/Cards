
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class DrawLine : UdonSharpBehaviour
{
    [SerializeField] private TrailRenderer drawingLine;
    [SerializeField] private TrailRenderer displayLine;
    
    
    void Start()
    {
        //drawingLine.emitting = false;
        drawingLine.enabled = false;
        drawingLine.Clear();
    }
    
    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "StartDrawing");
    }
    
    public override void OnPickupUseUp()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "StopDrawing");        
    }
    public void StartDrawing()
    {
        drawingLine.Clear();
        drawingLine.enabled = true;
    }

    public void StopDrawing()
    {
        Vector3[] positions = new Vector3[drawingLine.positionCount];
        drawingLine.GetPositions(positions);
        displayLine.AddPositions(positions);
        drawingLine.enabled = false;
        drawingLine.Clear();
    }
    /*
    public override void OnPickupUseDown()
    {
        drawingLine.emitting = true;
    }
    
    public override void OnPickupUseUp()
    {
        drawingLine.emitting = false;

    }
    */
}
