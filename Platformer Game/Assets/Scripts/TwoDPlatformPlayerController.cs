/*Copyright Jeremy Blair 2021
License (Creative Commons Zero, CC0)
http://creativecommons.org/publicdomain/zero/1.0/

You may use these scripts in personal and commercial projects.
Credit would be nice but is not mandatory.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderRotationMonitor : MonoBehaviour
{

    public bool IsRotating = false;
    System.DateTime lastCheckTime;
    float lastRotation;
    // Start is called before the first frame update
    void Start()
    {
        lastCheckTime = System.DateTime.Now;
        lastRotation = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        if ((System.DateTime.Now - lastCheckTime).TotalSeconds >= 1)
        {
            if (lastRotation != transform.rotation.z)
                IsRotating = true;
            lastCheckTime = System.DateTime.Now;
            lastRotation = transform.rotation.z;
        }
    }
}

public class TwoDPlatformPlayerController : MonoBehaviour
{

    public bool isOnGround = false;
    Rigidbody2D rb;
    private Animator anim;
    LayerMask groundMaskLayer;
    LayerMask ladderMaskLayer;
    LayerMask wallMaskLayer;
    bool hasMovingAnimation = false;
    bool hasJumpingAnimation = false;
    bool hasClimbingAnimation = false;

    public GameObject feetPosition;
    public float fastClimbFactor = 2f;
    public float fastJumpFactor = 1.5f;
    public float walkSpeed = 3f;//Replace with your max speed
    public float runSpeed = 6f;//Replace with your max speed
    public float climbSpeed = 3f;
    public float jumpSpeed = 5f;

    public string GroundMaskLayerName = "Ground";
    public string LadderMaskLayerName = "Ladder";
    public string wallMaskLayerName = "Walls";
    public string AttackAnimationTrigger = "Attack";
    public string JumpingAnimationParam = "IsJumping";
    public string MovingAnimationParam = "IsMoving";
    public string ClimbingAnimationParam = "IsClimbing";
    public string ClimbingIdleAnimationParam = "IsClimbingIdle";
    public string FastModeKey = "f";
    public string AttackAxis = "Fire1";
    public float airMoveFactor = 0.2f;//this controls how much the player moves while in the air.
    GameObject ladderPlatform = null;
    Collider2D closestClimableLader = null;
    bool isAttacking = true;
    bool isOnLadder = false;
    enum speed { fast, slow }
    speed movespeed = TwoDPlatformPlayerController.speed.slow;
    bool isClimbing = false;
    float distanceFromLadder = 0f;//used to help keep the player moving straight up and down the ladder when it is moving.
    Collider2D[] playerColliders;
    // Start is called before the first frame update

    //check to see if the user is touching a ladder collider
    Collider2D[] colliders;
    void Start()
    {
        
        playerColliders = GetComponentsInChildren<Collider2D>();

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Please add a rigidbody to your player to use this platformer script.");
            return;
        }
        anim = GetComponentInChildren<Animator>();
        if (rb == null)
        {
            Debug.LogError("Please add an animation to your player to use this platformer script.");
            return;
        }
        groundMaskLayer = LayerMask.GetMask(GroundMaskLayerName);
        ladderMaskLayer = LayerMask.GetMask(LadderMaskLayerName);
        wallMaskLayer = LayerMask.GetMask(wallMaskLayerName);
        //check to see if there is an "IsMoving" animation.
        try
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == MovingAnimationParam)
                    hasMovingAnimation = true;
            }
        }
        finally { }
        if (!hasMovingAnimation)
        {
            Debug.LogWarning("Please add a boolean " + MovingAnimationParam + " parameter to your player animation.");
        }

        //check to see if there is an "IsJumping" animation.
        try
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == JumpingAnimationParam)
                    hasJumpingAnimation = true;
            }

        }
        finally { }
        if (!hasJumpingAnimation)
        {
            Debug.LogWarning("Please add a boolean " + JumpingAnimationParam + " parameter to your player animation.");
        }

        //check to see if there is an "IsClimbing" animation.
        try
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == ClimbingAnimationParam)
                    hasClimbingAnimation = true;
            }

        }
        finally { }

        if (!hasClimbingAnimation)
            Debug.LogWarning("If you would like your player to climb, please add a " + ClimbingAnimationParam + " boolean parameter to your player animation.");

        //attach a rotation monitor script to all of the ladder colliders to track if they are rotating or not.
        foreach (Collider2D coll in colliders)
        {
            //check to see if the collider is in the ground layer and if we are touching it.
            if (coll.gameObject != this.gameObject && 1 << coll.gameObject.layer == ladderMaskLayer.value)
            {
                coll.gameObject.AddComponent<LadderRotationMonitor>();
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        colliders = GameObject.FindObjectsOfType<Collider2D>();
        float currentJumpSpeed = this.jumpSpeed;
        float currentClimbSpeed = climbSpeed;
        if (this.movespeed == speed.fast)
        {
            currentJumpSpeed *= fastJumpFactor;
            currentClimbSpeed *= fastClimbFactor;
        }



        //if the user is pushing up or down and they are touching a ladder, then disable the gravity and move the user
        if (Input.GetAxisRaw("Vertical") != 0 || isOnLadder)
        {
            //The player is allowed to jump if they are touching ground layers, they are giving a jump command, and their up velocity is less than the max jump speed.
            //find the ladder we are touching and put a platform on it.
            Collider2D climableLadder = isNearClimbableLadder(colliders);
            closestClimableLader = climableLadder;
            if (ladderPlatform == null)
            {

                if (climableLadder != null)
                {
                    isOnLadder = true;
                }
                else
                    isOnLadder = false;

                if (isOnLadder)
                {
                    if (Input.GetAxisRaw("Vertical") != 0)
                    {
                        //stop the horizontal velocity
                        rb.velocity = new Vector2(0, 0);
                    }
                    float verticalVelocity = rb.velocity.y;


                    //add the platform to stand on
                    ladderPlatform = new GameObject("LadderCollider");
                    float ladderYPos = feetPosition.transform.position.y;
                    if (verticalVelocity <= -20)
                        ladderYPos -= 30;
                    ladderPlatform.transform.position = new Vector2(feetPosition.transform.position.x, ladderYPos);
                    BoxCollider2D playerLadderCollider = (BoxCollider2D)ladderPlatform.AddComponent(typeof(BoxCollider2D));
                    //apply a friction physics material so the player cannot fly off if they are on a swinging ladder.
                    PhysicsMaterial2D friction = new PhysicsMaterial2D();
                    friction.friction = 1;
                    friction.bounciness = 0;
                    playerLadderCollider.sharedMaterial = friction;
                    //change this later to be the max width of the player's collider
                    playerLadderCollider.size = new Vector2(climableLadder.bounds.extents.x * 2, 0.05f);
                    ladderPlatform.transform.parent = climableLadder.transform;
                    //this is important to maintain the same velocity as the ladder.
                    gameObject.transform.parent = climableLadder.transform;
                }
            }
            else
            {
                //are we still touching the ladder?
                if (climableLadder != null)
                {
                    if (Input.GetAxisRaw("Vertical") != 0)
                    {
                        //stop the horizontal velocity
                        rb.velocity = new Vector2(0, 0);
                    }

                    isOnLadder = true;
                    //turn the ladder platform so it is facing left/right at all times.
                    Vector3 eulers = ladderPlatform.transform.eulerAngles;
                    eulers.z = 0;
                    ladderPlatform.transform.eulerAngles = eulers;
                    transform.eulerAngles = eulers;
                }
                else
                {
                    isOnLadder = false;
                    gameObject.transform.parent = null;
                    if (ladderPlatform != null)
                    {
                        Destroy(ladderPlatform);
                        ladderPlatform = null;
                    }


                }
            }

            if (ladderPlatform != null && isOnLadder)
            {
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
            if (gameObject.transform.parent != null)
                gameObject.transform.parent = null;
        }

        //if they are not on the ladder and the ladder platform exists, remove it immediately.
        if (!isOnLadder && ladderPlatform != null)
        {
            Destroy(ladderPlatform);
            isOnLadder = false;
            isClimbing = false;
            ladderPlatform = null;
        }

        if (isOnLadder && Input.GetAxisRaw("Vertical") != 0)
        {
            float newX = feetPosition.transform.position.x;
            float newPlatformY = ladderPlatform.transform.position.y + Input.GetAxisRaw("Vertical") * 0.005f * currentClimbSpeed;

            //put the platform centered on the swinging ladder at the current y position of the platform.
            //cast a ray towards the ladder.  If we are not at least the minimum distance then move closer to it.
            RaycastHit2D? hit = FindRayCastHistToClosestClimbableLadder();
            //if we found the collider of the ladder near by 
            if (hit.HasValue)
            {

                Collider2D collider = hit.Value.collider;
                float top = collider.offset.y + (collider.bounds.size.y / 2f);
                float btm = collider.offset.y - (collider.bounds.size.y / 2f);
                float left = collider.offset.x - (collider.bounds.size.x / 2f);
                float right = collider.offset.x + (collider.bounds.size.x / 2f);

                Vector3 topLeft = collider.gameObject.transform.TransformPoint(new Vector3(left, top, 0f));
                Vector3 topRight = collider.gameObject.transform.TransformPoint(new Vector3(right, top, 0f));
                Vector3 btmLeft = collider.gameObject.transform.TransformPoint(new Vector3(left, btm, 0f));
                Vector3 btmRight = collider.gameObject.transform.TransformPoint(new Vector3(right, btm, 0f));
                Debug.DrawLine(topLeft, topRight);
                Debug.DrawLine(topLeft, btmLeft);
                Debug.DrawLine(btmLeft, btmRight);
                Debug.DrawLine(btmRight, topRight);

                //uncomment this line to support swinging/rotating ladders

                LadderRotationMonitor monitor = collider.gameObject.GetComponent<LadderRotationMonitor>();
                if (monitor != null && monitor.IsRotating)
                    newX = hit.Value.point.x;

                //The player must maintain the distance from theladder platform
                float currentDistance = Vector2.Distance(gameObject.transform.position, ladderPlatform.transform.position);
                //this is what makes the swinging ladders work. (Keeping us centered as we climb up and down)
                gameObject.transform.position = new Vector2(newX, newPlatformY);

            }
            ladderPlatform.transform.position = new Vector2(newX, newPlatformY);


        }


        //Attack animation (Don't set the attack trigger if we are already in the attack state.
        if (!isAttacking && Input.GetAxisRaw(AttackAxis) > 0 && anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationTrigger))
        {
            isAttacking = true;
            anim.SetTrigger(AttackAnimationTrigger);
        }
        else if (Input.GetAxisRaw(AttackAxis) == 0)
        {
            isAttacking = false;
        }




        bool grounded = isTouchingGround(colliders);
        bool allowAjump = (grounded || isOnLadder) && IsJumping();//&& rb.velocity.y < currentClimbSpeed;

        //proper rotation of the game object
        if (Input.GetAxis("Horizontal") < 0 && gameObject.transform.rotation.y != 0)
            gameObject.transform.rotation = Quaternion.identity;

        if (Input.GetAxis("Horizontal") > 0 && gameObject.transform.rotation.y == 0)
            gameObject.transform.Rotate(0, 180, 0);

        //run the move animation or idle animation if necessary.
        if ((hasMovingAnimation && Input.GetAxisRaw("Horizontal") == 0 && anim.GetBool(MovingAnimationParam)))
            anim.SetBool(MovingAnimationParam, false);
        else if (hasMovingAnimation && Input.GetAxisRaw("Horizontal") != 0 && !anim.GetBool(MovingAnimationParam))
            anim.SetBool(MovingAnimationParam, true);

        //run the jumping animation if necessary
        if (hasJumpingAnimation && (Input.GetAxisRaw("Jump") == 0 && anim.GetBool(JumpingAnimationParam) || grounded || isOnLadder))
            anim.SetBool(JumpingAnimationParam, false);
        else if (hasJumpingAnimation && (Input.GetAxisRaw("Jump") != 0 && !anim.GetBool(JumpingAnimationParam) || (!grounded && !isOnLadder)))
            anim.SetBool(JumpingAnimationParam, true);

        //run the climbing idle animation if necessary
        if (hasClimbingAnimation && ((Input.GetAxisRaw("Vertical") != 0 && anim.GetBool(ClimbingIdleAnimationParam)) || !isOnLadder))
            anim.SetBool(ClimbingIdleAnimationParam, false);
        else if (hasClimbingAnimation && isOnLadder && Input.GetAxisRaw("Vertical") == 0 && !anim.GetBool(ClimbingIdleAnimationParam))
            anim.SetBool(ClimbingIdleAnimationParam, true);

        //run the climbing animation if necessary
        if (hasClimbingAnimation && ((Input.GetAxisRaw("Vertical") == 0 && anim.GetBool(ClimbingAnimationParam)) || !isOnLadder))
            anim.SetBool(ClimbingAnimationParam, false);
        else if (hasClimbingAnimation && isOnLadder && Input.GetAxisRaw("Vertical") != 0 && !anim.GetBool(ClimbingAnimationParam))
            anim.SetBool(ClimbingAnimationParam, true);

        try
        {
            if (Input.GetKeyDown(FastModeKey))
            {
                if (this.movespeed == speed.fast)
                    this.movespeed = speed.slow;
                else
                    this.movespeed = speed.fast;
            }
        }
        catch (System.Exception exc)
        {
            Debug.LogError(exc.Message);
        }

        float goSpeed = walkSpeed;

        if (this.movespeed == speed.fast)
            goSpeed = runSpeed;

        if (!rb.IsTouchingLayers(groundMaskLayer.value))
            isOnGround = false;


        //move forward if we are not touching the ground.
        if (rb.IsTouchingLayers(groundMaskLayer.value))
        {
            isOnGround = true;
            rb.AddForce(Vector2.right * Input.GetAxis("Horizontal") * goSpeed, ForceMode2D.Impulse);
            
        }
        if (!rb.IsTouchingLayers(wallMaskLayer.value))
        {
            //allow the player to turn but only add a smaller force amount.
            rb.AddForce(Vector2.right * Input.GetAxis("Horizontal") * airMoveFactor * goSpeed, ForceMode2D.Impulse);
            //print("air turning....");

        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            print("Not able to turn.");
        }
        else
        {
            print(rb.IsTouchingLayers(wallMaskLayer.value));
        }

        //jump code (only allow jump if we are touching the ground and the collider is active.
        if (allowAjump)
        {

            rb.AddForce(Vector2.up * currentJumpSpeed, ForceMode2D.Impulse);
            //remove the ladder platform now that they have jumped.
            if (ladderPlatform != null)
            {
                Destroy(ladderPlatform);
                ladderPlatform = null;
                isOnLadder = false;
            }

        }

        //max speed
        //if (rb.velocity.magnitude > maxRunSpeed)
        //    rb.velocity = rb.velocity.normalized * maxRunSpeed;


        if (this.movespeed == speed.slow && Mathf.Abs(rb.velocity.x) > goSpeed)
        {
            // and finally asign the new vel
            rb.velocity = new Vector2(goSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
        else if (this.movespeed == speed.fast && Mathf.Abs(rb.velocity.x) > runSpeed)
        {
            // and finally asign the new vel
            rb.velocity = new Vector2(runSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }




        if (isClimbing && Mathf.Abs(rb.velocity.y) > currentClimbSpeed)
        {
            // and finally asign the new vel
            rb.velocity = new Vector2(rb.velocity.x, currentClimbSpeed * Mathf.Sign(rb.velocity.y));
        }
        else if (IsJumping() && Mathf.Abs(rb.velocity.y) > currentJumpSpeed)
        {
            // and finally asign the new vel
            rb.velocity = new Vector2(rb.velocity.x, currentJumpSpeed * Mathf.Sign(rb.velocity.y));
        }


        //turn off any ground colliders which are not below the FootLocation of the player.
        GameObject myFeetPosition = gameObject;
        if (feetPosition != null)
            myFeetPosition = feetPosition;
        foreach (Collider2D coll in colliders)
        {
            //check to see if the collider is in the ground layer and if we are touching it.
            if (coll.gameObject != this.gameObject && 1 << coll.gameObject.layer == groundMaskLayer.value)
            {
                //Debug.Log(coll.gameObject.name + ":" + (coll.bounds.extents.y + coll.bounds.center.y).ToString());
                //are we above it, and are we over it
                bool isOver = myFeetPosition.transform.position.x < (coll.bounds.extents.x + coll.bounds.center.x) && myFeetPosition.transform.position.x > (-coll.bounds.extents.x + coll.bounds.center.x);
                bool isAbove = (-coll.bounds.extents.y + coll.bounds.center.y) > myFeetPosition.transform.position.y;
                if (isAbove && !coll.isTrigger && !rb.IsTouching(coll))
                {
                    coll.isTrigger = true;
                }
                else if ((coll.bounds.extents.y + coll.bounds.center.y) <= myFeetPosition.transform.position.y)
                {
                    coll.isTrigger = false;
                }
            }
        }




        //do this at the end of update so as not to break code above it
        if (canJump && Input.GetAxisRaw("Jump") > 0)
            canJump = false;
        else if (!canJump && Input.GetAxisRaw("Jump") == 0)
            canJump = true;
    }

    private RaycastHit2D? FindRayCastHistToClosestClimbableLadder()
    {
        Collider2D coll_var = closestClimableLader.GetComponent<Collider2D>();
        Vector2 direction = Vector2.left;
        RaycastHit2D? hit = findLadderRayCastHit(direction, coll_var);
        if (!hit.HasValue)
        {
            direction = Vector2.right;
            hit = findLadderRayCastHit(direction, coll_var);
        }
        if (!hit.HasValue)
        {
            direction = Vector2.up;
            hit = findLadderRayCastHit(direction, coll_var);
        }
        if (!hit.HasValue)
        {
            direction = Vector2.down;
            hit = findLadderRayCastHit(direction, coll_var);
        }

        return hit;
    }

    RaycastHit2D? findLadderRayCastHit(Vector2 direction, Collider2D match)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(feetPosition.transform.position, direction);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetInstanceID() == match.GetInstanceID())
            {
                return hit;
            }
        }
        return null;
    }

    /// <summary>
    /// This method prevents the player from holding down the jump key and "skipping" up ground.  
    /// Once the jump key is pressed they must let go of it before pressing it again.
    /// </summary>
    bool canJump = true;
    private bool IsJumping()
    {
        return canJump && Input.GetAxisRaw("Jump") > 0;
    }

    /// <summary>
    /// Checks if the player is touching the ground
    /// </summary>
    /// <returns></returns>
    private bool isTouchingGround(Collider2D[] colliders)
    {
        GameObject myFeetPosition = gameObject;
        if (feetPosition != null)
            myFeetPosition = feetPosition;
        foreach (Collider2D coll in colliders)
        {
            //check toi see if the collider is in the ground layer and if we are touching it.
            if (coll.gameObject != this.gameObject && 1 << coll.gameObject.layer == groundMaskLayer.value)
            {
                foreach (Collider2D playerCollider in playerColliders)
                    if (coll.IsTouching(playerCollider) && coll.isTrigger == false)
                    {
                        return true;
                    }
            }
        }
        return false;
    }

    Collider2D isNearClimbableLadder(Collider2D[] colliders)
    {
        if (rb.IsTouchingLayers(ladderMaskLayer.value))
        {
            //we don't just want to be touching a ladder, we need to be within the bounds of its left and right collider
            foreach (Collider2D coll in colliders)
            {
                if (coll.gameObject != this.gameObject && 1 << coll.gameObject.layer == ladderMaskLayer.value && gameObject.transform.position.x > (-coll.bounds.extents.x + coll.bounds.center.x) && gameObject.transform.position.x < (coll.bounds.extents.x + coll.bounds.center.x) && gameObject.transform.position.y > (-coll.bounds.extents.y + coll.bounds.center.y) && gameObject.transform.position.y < (coll.bounds.extents.y + coll.bounds.center.y))
                {
                    //we are climbing
                    //the ladder feature has a box collider under the player so they can move left and right
                    isOnLadder = true;
                    return coll;
                }
            }
        }
        return null;

    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }



}






