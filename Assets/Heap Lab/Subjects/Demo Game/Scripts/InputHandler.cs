using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class InputHandler : MonoBehaviour
{
    public Player Player;
    Vector3 delta = Vector3.zero;
    float rotation = 0f;
    void Update()
    {
        Transform movingTr = Player.transform;
        delta = Vector3.zero;
        rotation = 0f;
        // Check keyboard inputs
        if (Input.GetKey(KeyCode.W))
        {
            delta = movingTr.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            delta = -movingTr.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            delta -= movingTr.right;
            rotation = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            delta += movingTr.right;
            rotation = 1f;
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
            Player.Run(delta,rotation);
        }


        if (!Input.anyKey)
        {
            Player.Idle();
        }
    }
}
