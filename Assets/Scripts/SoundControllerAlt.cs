using UnityEngine;
using System.Collections;

public class SoundControllerAlt : MonoBehaviour
{

    [SerializeField]
    private static AudioClip[] clip;


    // Use this for initialization
    void Start()
    {
       // Inv
    }

    public static void fall(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[0], position);
    }

    public static void fatJump(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[1], position);
    }

    public static void fatRun(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[2], position);
    }

    public static void fiesta(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[3], position);
    }

    public static void fiestaStop(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[4], position);
    }

    public static void skinnyJump(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[5], position);
    }

    public static void skinnyDoubleJump(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[6], position);
    }

    public static void skinnyRunning(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[7], position);
    }

    public static void skinnyIdle(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[8], position);
    }
    public static void skinnyIdleStop(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip[9], position);
    }
}
