using UnityEngine;
using System.Collections;

public class DontDie : MonoBehaviour
{
    [SerializeField]
    string playerTag;

    Transform collidedPlayer;
    Vector3 newPosition;

    // Update is called once per frame
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag(playerTag))
        {
            collidedPlayer = coll.transform;
            newPosition = coll.GetComponent<CharacterController2D>().lastGroundedPosition;
            Invoke("Respawn", 0.2f);
        }

        else if (coll.CompareTag("PullableObject") || coll.CompareTag("InteractableObject"))
            Destroy(coll.gameObject);
    }

    void Respawn()
    {
        collidedPlayer.position = newPosition;
    }
}
