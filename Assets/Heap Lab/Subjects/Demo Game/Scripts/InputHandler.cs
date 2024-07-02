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
            delta = Vector3.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            delta = -Vector3.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            delta -= Vector3.right;
            rotation = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            delta += Vector3.right;
            rotation = 1f;
        }

        delta = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

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
            Player.Run(delta,Input.GetAxis("Mouse X"));
        }


        if (!Input.anyKey)
        {
            Player.Idle();
        }
    }
}
