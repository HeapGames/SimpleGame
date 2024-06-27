using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class InputHandler : MonoBehaviour
{
    public Player Player;
    Vector3 deltaX = Vector3.zero;
    Vector3 rotAxis = Vector3.zero;
    void Update()
    {
        Transform movingTr = Player.transform.parent;
        deltaX = Vector3.zero;
        rotAxis = Vector3.zero;
        // Check keyboard inputs
        if (Input.GetKey(KeyCode.W))
        {
            deltaX = transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            deltaX = -transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotAxis = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotAxis = Vector3.up;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Player.Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Code for shooting or another action goes here
        }

        if (!Player.IsJumping)
        {
            Player.Run(deltaX);
            //Player.Rotate(rotAxis);
        }


        if (!Input.anyKey)
        {
            Player.Idle();
        }
    }
}
