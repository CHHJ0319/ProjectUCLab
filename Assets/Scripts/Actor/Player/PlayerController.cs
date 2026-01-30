using UnityEngine;

namespace Actor.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]

    public class PlayerController : MonoBehaviour
    {

        public float animSpeed = 1.5f;
        public float lookSmoother = 3.0f;
        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;

        private CapsuleCollider col;
        private Rigidbody rb;

        private Vector3 velocity;
        private float orgColHight;
        private Vector3 orgVectColCenter;

        private Animator anim;
        private AnimatorStateInfo currentBaseState;

        private PlayerInputHandler playerInputHandler;

        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        void Start()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();

            playerInputHandler = GetComponent<PlayerInputHandler>();

            orgColHight = col.height;
            orgVectColCenter = col.center;
        }

        void FixedUpdate()
        {
            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            SetGravity(true);
            UpdateMovementAnimation(h, v);
            UpdateAnimationState();
            CalculateVelocity(v);
            Jump();
            ApplyMovement(h);
            UpdateStateBehavior();
        }

        void SetGravity(bool active)
        {
            rb.useGravity = active;
        }

        void UpdateMovementAnimation(float horizontal, float vertical)
        {
            anim.SetFloat("Speed", vertical);
            anim.SetFloat("Direction", horizontal);

            anim.speed = animSpeed;
        }

        void UpdateAnimationState()
        {
            currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        }

        void CalculateVelocity(float vertical)
        {
            velocity = new Vector3(0, 0, vertical);
            velocity = transform.TransformDirection(velocity);

            if (vertical > 0.1f)
            {
                velocity *= forwardSpeed;
            }
            else if (vertical < -0.1f)
            {
                velocity *= backwardSpeed;
            }
        }

        void Jump()
        {
            if (playerInputHandler.JumpTriggered)
            {
                if (currentBaseState.fullPathHash == locoState)
                {
                    if (!anim.IsInTransition(0))
                    {
                        rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                        anim.SetBool("Jump", true);
                    }
                }
            }
        }

        void ApplyMovement(float horizontal)
        {
            transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, horizontal * rotateSpeed, 0);
        }

        void UpdateStateBehavior()
        {
            if (currentBaseState.fullPathHash == locoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }
            else if (currentBaseState.fullPathHash == jumpState)
            {
                if (!anim.IsInTransition(0))
                {
                    if (useCurves)
                    {
                        float jumpHeight = anim.GetFloat("JumpHeight");
                        float gravityControl = anim.GetFloat("GravityControl");
                        if (gravityControl > 0)
                            rb.useGravity = false;

                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                col.height = orgColHight - jumpHeight;
                                float adjCenterY = orgVectColCenter.y + jumpHeight;
                                col.center = new Vector3(0, adjCenterY, 0);
                            }
                            else
                            {
                                ResetCollider();
                            }
                        }
                    }
                    anim.SetBool("Jump", false);
                }
            }
            else if (currentBaseState.fullPathHash == idleState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
                if (playerInputHandler.JumpTriggered)
                {
                    anim.SetBool("Rest", true);
                }
            }
            else if (currentBaseState.fullPathHash == restState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Rest", false);
                }
            }
        }

        void ResetCollider()
        {
            col.height = orgColHight;
            col.center = orgVectColCenter;
        }
    }
}


