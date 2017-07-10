using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundController : MonoBehaviour
{

    public CharacterController2D skinny, fatty;

    public AudioClip[] clips;

    public static SoundController instance;

    public bool fiesta, sinav;

    void Awake()
    {
        instance = this;

        fiesta = false;
        sinav = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    // Use this for initialization
    void Start()
    {
        //skinny.gameObject.AddComponent<AudioSource>();
        //fatty.gameObject.AddComponent<AudioSource>();
    }

    public static void FiestaSound()
    {
        if (!instance.fiesta)
        {
            instance.fatty.GetComponent<AudioSource>().loop = true;
            instance.fatty.GetComponent<AudioSource>().clip = instance.clips[4];
            instance.fatty.GetComponent<AudioSource>().Play();
        }

        instance.fiesta = true;
    }

    public static void CancelFiesta()
    {
        if (instance.fiesta)
        {
            instance.fiesta = false;
            instance.fatty.GetComponent<AudioSource>().loop = false;
            instance.fatty.GetComponent<AudioSource>().Stop();
        }
    }

    public static void SkinnyJumpOne()
    {
        instance.skinny.GetComponent<AudioSource>().PlayOneShot(instance.clips[5]);
    }

    public static void FatRoll()
    {
        instance.skinny.GetComponent<AudioSource>().PlayOneShot(instance.clips[9]);
    }



    public static void SkinnyJumpTwo()
    {
        instance.skinny.GetComponent<AudioSource>().PlayOneShot(instance.clips[6]);
    }

    public static void Sinav()
    {
        if (!instance.sinav)
        {
            instance.skinny.GetComponent<AudioSource>().loop = true;
            instance.skinny.GetComponent<AudioSource>().clip = instance.clips[8];
            instance.skinny.GetComponent<AudioSource>().Play();
        }

        instance.sinav = true;
    }

    public static void CancelSinav()
    {
        if (instance.sinav)
        {
            instance.sinav = false;
            instance.skinny.GetComponent<AudioSource>().loop = false;
            instance.skinny.GetComponent<AudioSource>().Stop();
        }
    }

    public static void FatJump()
    {
        instance.fatty.GetComponent<AudioSource>().PlayOneShot(instance.clips[2]);
    }

    public static void Diyalog()
    {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(instance.clips[0]);
    }

    public static void Dusme()
    {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(instance.clips[1]);
    }

}
