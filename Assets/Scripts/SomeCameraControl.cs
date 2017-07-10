using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SomeCameraControl : MonoBehaviour
{
    [SerializeField]
    CharacterController2D[] player;

    [SerializeField]
    Image button;

    [SerializeField]
    Sprite[] buttonImage;

    int selectedCharacter;

    // Shake variables begin
    bool shake, smooth;
    Vector3 currentForcePower;
    [SerializeField]
    public Vector3 forcePower = new Vector3(0, 0.5f, 0);
    Vector3 lastCamPosition;
    Quaternion lastCamRotation;
    [SerializeField]
    float currentSpeed;
    private float smoothSpeed;
    // Shake variables end


    // Use this for initialization
    void Start()
    {
        selectedCharacter = 0;
        smoothSpeed = currentSpeed;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Shake(float latency)
    {
        Invoke("TimedShake", latency);
    }

    public void TimedShake()
    {
        currentForcePower = forcePower;
        lastCamPosition = transform.position;
        lastCamRotation = transform.rotation;
        shake = true;
        currentSpeed = smoothSpeed;
    }


    void Update()
    {

        var targetPosition = player[selectedCharacter].transform.position + Vector3.back * 10 + Vector3.up * 3 + Vector3.right * player[selectedCharacter].GetComponent<CharacterController2D>().direction * -5;

        if (shake)
        {
            currentForcePower = Vector3.Lerp(currentForcePower, Vector3.zero, Time.deltaTime * 5);
            transform.position = lastCamPosition + new Vector3(Mathf.Cos(Time.time * 80) * currentForcePower.x, Mathf.Cos(Time.time * 80) * currentForcePower.y, Mathf.Cos(Time.time * 80) * currentForcePower.z);

            if (currentForcePower.y < forcePower.y / 5)
            {
                shake = false;
                smooth = true;
            }

        }
        else if (smooth)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lastCamRotation, currentSpeed * Time.deltaTime);

            currentSpeed += 0.1f;

            transform.LookAt(player[selectedCharacter].transform.position);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                // {
                // if (transform.position == targetPosition)
                smooth = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);
            transform.LookAt(player[selectedCharacter].transform.position);
        }

        if (Input.GetButtonUp("Fire3"))
            ChangeCharacter();
    }

    public void ChangeCharacter()
    {
        player[0].enabled = !player[0].enabled;
        player[1].enabled = !player[1].enabled;
        button.sprite = buttonImage[selectedCharacter];
        selectedCharacter = (selectedCharacter + 1) % 2;
    }
}
