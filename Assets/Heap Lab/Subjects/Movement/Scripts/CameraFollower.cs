using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollower : MonoBehaviour
{
    public float Speed = 1f; 
    public Transform FollowTarget;
    public Transform LookTarget;
    public Vector3 Offset;
    private Transform CameraTr;
    // Start is called before the first frame update
    void Start()
    {
        CameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CameraTr.position = Vector3.Lerp(CameraTr.position, FollowTarget.position + FollowTarget.rotation * Offset, Time.deltaTime * Speed);
        
        if(LookTarget != null)
        {
            CameraTr.LookAt(LookTarget);
        }
    }
}
