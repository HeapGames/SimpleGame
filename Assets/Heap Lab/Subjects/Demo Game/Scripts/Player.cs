using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody Rb;
    public Renderer Renderer;
    public Animator Animator;
   
    public float RunSpeed = 5f;
    public float JumpHeight = 1f;
    public float JumpSpeed = 1f;
    public float RotationSpeed = 1f;

    public float Health = 1f;

    public bool IsIdle;
    public bool IsRunning;
    public bool IsJumping;

    private Vector3 velocity;

    public void Idle()
    {
        Animator.SetTrigger("idle");
    }

    public void Rotate(Vector3 axis)
    {
        //RotateCharacterToMouse();
        RotateCharacterToPosition();
    }



    void RotateCharacterToMouse()
    {
        Transform movingTr = transform.parent;

        Vector3 mouseScreenPosition = Input.mousePosition;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 50f));

        Vector3 characterPosition = movingTr.position;

        mouseWorldPosition.y = characterPosition.y;

        Vector3 directionToMouse = mouseWorldPosition - characterPosition;

        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

        movingTr.rotation = Quaternion.Slerp(movingTr.rotation, targetRotation, Time.deltaTime * 5f);


    }

    void RotateCharacterToPosition(Vector3 pos)
    {
        Transform movingTr = transform.parent;


        Vector3 characterPosition = movingTr.position;

        pos.y = characterPosition.y;

        Vector3 directionToMouse = pos - characterPosition;

        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

        float angle_diff = Mathf.Abs(movingTr.eulerAngles.y - targetRotation.eulerAngles.y);

        movingTr.rotation = Quaternion.Slerp(movingTr.rotation, targetRotation, Time.deltaTime * 5f);

    }


    public void Run(Vector3 velocity)
    {
        Animator.SetFloat("run", velocity.magnitude);
        Move(velocity);
    }

    private void Move(Vector3 velocity)
    {
        this.velocity = velocity;
        Transform movingTr = transform.parent;
        Vector3 deltaPosition = velocity * Time.deltaTime;
        movingTr.position = movingTr.position + movingTr.forward * deltaPosition.z * RunSpeed;
    }

    public void Jump()
    {
        if (IsJumping)
        {
            return;
        }
        IsJumping = true;
        Animator.SetTrigger("jump");
        StartCoroutine(JumpRoutine());
    }

    public void Flip()
    {
        Animator.SetTrigger("flip");
    }

    public void Land()
    {
        Animator.SetTrigger("land");
    }

    IEnumerator JumpRoutine()
    {
        bool jumping_up = true;

        while (true)
        {
            if (jumping_up)
            {
                if (transform.localPosition.y >= JumpHeight)
                {
                    jumping_up = false;
                }

                transform.localPosition = transform.localPosition + Vector3.up * Time.deltaTime * JumpSpeed;
            }
            else
            {

                if (transform.localPosition.y <= 0f)
                {
                    transform.localPosition = Vector3.zero;
                    break;
                }

                transform.localPosition = transform.localPosition + Vector3.down * Time.deltaTime * JumpSpeed;

            }
            Move(velocity);

            yield return null;
        }

        IsJumping = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("enemy"))
        {
            Health -= Time.deltaTime * 0.1f;
        }
    }
}
