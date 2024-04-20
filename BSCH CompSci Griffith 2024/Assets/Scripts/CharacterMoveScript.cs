using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterMoveScript : MonoBehaviour
{
    public float maxSpeed;
    public float acceleration;
    public Rigidbody2D myRb;
    public Animator anim;
    public bool isGrounded;
    public float jumpForce;
    public float secondaryJumpForce;
    public float secondaryJumpDelay;
    public bool secondaryJump;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        myRb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed",Mathf.Abs(myRb.velocity.x));
        anim.SetFloat("verticalSpeed",myRb.velocity.y);
        if(Input.GetAxis("Horizontal")<0)
        {
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        if(Input.GetAxis("Horizontal") >0)
        {
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        if(Mathf.Abs(myRb.velocity.magnitude) < maxSpeed&& Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            myRb.AddForce(new Vector2(acceleration * Input.GetAxis("Horizontal"), 0), ForceMode2D.Force);
        }
        if(Input.GetButtonDown("Jump")&& isGrounded)
        {
            myRb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            StartCoroutine(SecondaryJump());
        }
        if(Input.GetButton("Jump")&& secondaryJump == true && isGrounded)
        {
            myRb.AddForce(new Vector2(0, secondaryJumpForce), ForceMode2D.Force);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        isGrounded = true;
        anim.SetBool("grounded", true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isGrounded = false;
        anim.SetBool("grounded", false);
    }
    IEnumerator SecondaryJump()
    {
        secondaryJump = true;
        yield return new WaitForSeconds(secondaryJumpDelay);
        secondaryJump = false;
        yield return null;
    }
}
