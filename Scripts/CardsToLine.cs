
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CardsToLine : UdonSharpBehaviour
{
    float pointDistance;
    Vector3 point;
    float distance = 0;
    float remainingDistance = 0;
    [SerializeField] TrailRenderer lineRenderer;
    [SerializeField] Transform groundLevel;
    [SerializeField] int angleOfLaunch;
    int i;
    Vector3 start;
    Vector3 next;
    float totalDistance;
    [SerializeField] DeckManager2 deck;
    [SerializeField] ButtonManager buttonManager;
    public override void Interact()
    {
        if (!buttonManager.masterLockout)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "LineSetup");
        }
    }
    public void LineSetup()
    {
        if (lineRenderer.positionCount <= 1)
        {
            return;
        }
        start = lineRenderer.GetPosition(0);
        next = lineRenderer.GetPosition(1);
        int size = lineRenderer.positionCount;
        i = 1;
        deck.AutoSetShapeToLine();
    }
    public void _LaunchCube(GameObject launchObject)
    {
        if(GetPoint())
        {
            Vector3 target = point;
            target.y = groundLevel.position.y;
            Vector3 vectorToTarget = target - launchObject.transform.position;
            float launchPower = Mathf.Sqrt((9.8f*vectorToTarget.magnitude)/(Mathf.Sin(2*(Mathf.Deg2Rad * angleOfLaunch))));
            float launchPowerHorizontal = launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch);       
            Vector3 launchVector = new Vector3(launchPower * Mathf.Cos(Mathf.Deg2Rad * angleOfLaunch) * Mathf.Sign(vectorToTarget.x), 0, 0); 
            Vector3 rotateTarget = new Vector3(vectorToTarget.x, 0, vectorToTarget.z);
            launchVector = Vector3.RotateTowards(launchVector, rotateTarget, Mathf.Deg2Rad * 360, 0);
            Vector3 rotateNext = new Vector3(next.x, 0, next.z);
            Vector3 rotateStart = new Vector3(start.x, 0, start.z);

            launchObject.transform.rotation = Quaternion.LookRotation((rotateNext - rotateStart).normalized) * Quaternion.FromToRotation(Vector3.up, Vector3.back);
            launchObject.GetComponent<Rigidbody>().AddForce(launchVector.x, launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch), launchVector.z, ForceMode.Impulse);
            float timeOfFlight = 2 * (launchPower * Mathf.Sin(Mathf.Deg2Rad * angleOfLaunch))/9.8f;
        }
        else
        {
            _LaunchCube(launchObject);
        }
        
    }
    bool GetPoint()
    {
        if (Vector3.Distance(start, next) > .1)
        {
            start = next;
            next = lineRenderer.GetPosition(i);
            return false;
        }
        int size = lineRenderer.positionCount;
        if (Step2Points(start, next))
        {
            start = point;
            return true;
        }
        else
        {
            i++;
            if (i == size)
            {
                i = 0;      
            }
            start = next;
            next = lineRenderer.GetPosition(i);
            return false;
        }
    }
    public bool Step2Points(Vector3 start, Vector3 next)
    {
        Vector3 direction = (next - start).normalized;
        float contextDistance = Vector3.Distance(start, next);
        
        distance += contextDistance;
        if (distance > pointDistance)
        {
            point = start + (direction * (pointDistance - remainingDistance));
            remainingDistance = distance = 0;
            return true;
        }        
        remainingDistance = distance;
        return false;
    }
    void GetTotalDistance()
    {
        totalDistance = 0;

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            if ((lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i + 1)).magnitude > .1)
            {
                i++;
            }
            totalDistance += (lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i + 1)).magnitude;
        }
        
    }
    public void _SetSpacing(int cards)
    {
        GetTotalDistance();
        Debug.Log(totalDistance);
        pointDistance = totalDistance / cards;
    }
}
