using CnControls;
using UnityEngine;

public class IvyScript : MonoBehaviour
{
    private GameObject parent;

    private CharacterController2D character;
    private Rigidbody characterRigidbody, targetRigidbody;
    private Transform parentTransform, targetTransform, mainIvy;
    private CapsuleCollider capsuleCollider;

    private Vector3 modifiedVelocity, momentum;

    private IvySetup currentIvy;

    private SpringJoint fixedJoint;

    private Animator animController;

    // Use this for initialization
    void Awake()
    {
        parent = transform.parent.gameObject;
        character = parent.GetComponent<CharacterController2D>();
        capsuleCollider = parent.GetComponent<CapsuleCollider>();
        characterRigidbody = parent.GetComponent<Rigidbody>();
        parentTransform = parent.GetComponent<Transform>();

        // fixedJoint = parent.GetComponent<SpringJoint>();

        modifiedVelocity = new Vector3();
        animController = parent.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!character.holdingIvy && !character.onGround && coll.CompareTag("Ivy"))
        {
            // animController.SetBool("Hang", true);
            animController.SetTrigger("Hang");

            character.ivyReleased = false;
            character.holdingIvy = true;
            capsuleCollider.isTrigger = true;

            targetTransform = coll.transform;
            mainIvy = targetTransform.parent;

            modifiedVelocity.Set(character.direction, 0, 0);

            momentum = characterRigidbody.mass * modifiedVelocity * 10f;

            currentIvy = mainIvy.GetComponent<IvySetup>();
            currentIvy.ApplyVelocity(momentum);

            targetRigidbody = coll.GetComponent<Rigidbody>();
            // targetRigidbody.mass += parentRigidbody.mass;

            characterRigidbody.isKinematic = true;
            parentTransform.parent = targetTransform.GetChild(0);
            parentTransform.localPosition = new Vector3(0, 0, 0);
            parentTransform.localRotation = Quaternion.Euler(0, 90 * character.direction, 0);
        }
    }

    public void Jump(bool jump)
    {
        if (jump && character.holdingIvy)
        {
            characterRigidbody.isKinematic = false;
            character.onGround = false;
            character.jump = false;
            
            characterRigidbody.velocity = new Vector3(character.direction * 4f, 3f, 0);
            parentTransform.parent = null;
            parentTransform.rotation = Quaternion.Euler(0, 90 * character.direction, 0);

            // animController.SetBool("Hang", false);

            character.ivyReleased = true;
            Invoke("ReleaseIvy", 0.6f);
        }
    }

    public void AddVelocityTransfer(float x)
    {
        if (character.holdingIvy)
            currentIvy.AddVelocity(new Vector3(x, 0, 0));
    }

    void ReleaseIvy()
    {
        character.holdingIvy = false;
        capsuleCollider.isTrigger = false;
    }
}