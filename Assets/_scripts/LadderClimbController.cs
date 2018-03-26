using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class LadderClimbController : MonoBehaviour
{
    [SerializeField]
    private float climbingSpeed;

    private bool isClimbingLadder = false;
    private bool isCollidingWithLadder = false;
    private CharacterController characterController;
    private FirstPersonController firstPersonController;
    private Transform ladderTransform;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (isClimbingLadder)
        {
            if (!isCollidingWithLadder || CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                ToggleLadderClimbing(false);
            }
        }
        else if (isCollidingWithLadder)
        {
            if (Vector3.Dot(ladderTransform.forward, transform.forward) >= 0.9f &&
                (CrossPlatformInputManager.GetAxis("Vertical") > .0f || !characterController.isGrounded))
            {
                ToggleLadderClimbing(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isClimbingLadder)
        {
            transform.Translate(Vector3.up * CrossPlatformInputManager.GetAxis("Vertical") *
                climbingSpeed * Time.deltaTime);
        }
    }

    private void ToggleLadderClimbing(bool isEnabled)
    {
        isClimbingLadder = isEnabled;
        firstPersonController.ToggleMovement(!isEnabled);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.tag == "Ladder")
        {
            Debug.Log("Collided with ladder");
            isCollidingWithLadder = true;
            ladderTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ladder")
        {
            isCollidingWithLadder = false;
        }
    }
}
