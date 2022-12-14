using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (20f,20f);
    Vector2 moveInput; 
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Gun myGun;
    float gravityScaleAtStart;

    bool isTurnRight;
    bool isAlive = true;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();        
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myGun = GetComponentInChildren<Gun>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if(!isAlive) { return ;}
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void OnFire(InputValue value)
    {
        if(!isAlive) { return; }
        myGun.Fire();
    }

    private void OnMove(InputValue value)
    {
        if(!isAlive) { return ;}
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if(!isAlive) { return ;}
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }


        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    // void JumpAnimationCheck()
    // {
    //     if(!isAlive) { return ;}
    //     if(myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies" , "Hazards")))
    //     {
    //         myAnimator.SetBool("isJumping",false);
    //     }
    //     else if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
    //     {
    //         myAnimator.SetBool("isJumping",true);
    //     }
    // }
    
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x *runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning",playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing" , false);

            return; 
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x , moveInput.y *climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing" , playerHasVerticalSpeed);
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies" , "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;

            StartCoroutine(DieDelay());
        }
    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSecondsRealtime(3);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
