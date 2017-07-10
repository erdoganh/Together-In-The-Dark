using UnityEngine;
using System.Collections;

public class ObjectListener : MonoBehaviour
{

    public enum PuzzleMode
    {
        Door, OneTimeDoor, ColorSequence, ColorSequenceListen, PuzzleSequence, HamsterWheel
    };

    public enum CharOperator
    {
        Skinny, Fat, Both
    };

    public PuzzleMode puzzleMode;
    public CharOperator charOperator;

    public GameObject DoorObject;
    public float DoorDesiredHeight;
    public float DoorLiftSpeed;
    private float DoorOriginalHeight;

    public GameObject[] ColorObjects;
    public int[] ColorCounts;
    private int[] originalColorCount;

    public GameObject[] ListeningColorObjects;
    public GameObject ListeningColorObjectsPanel;
    public int[] ListeningColorCounts;
    public Transform ListeningColorObjectsPanelOriginalPos;
    private int[] originalListeningColorCount;

    public AudioClip[] crystalSound;

    public GameObject[] PuzzleSequenceObjects;
    public bool[] PuzzleSequenceIsDown;
    public float PuzzleSequenceUpPosition;
    public float PuzzleSequenceDownPosition;

    public GameObject HamsterWheelObject;
    public GameObject HamsterWheelAttachedObject;
    public float HamsterWheelAttachedObjectDesiredPos;
    private float HamsterWheelAttachedObjectOriginalPos;
    private bool HamsterWheelActive;

    public GameObject AssignedParticleSystem;

    public bool PlayAudio;
    public bool AlternateAudio;
    public AudioClip alternateAudio;

    private Coroutine operation;
    private Coroutine secondaryOperation;

    bool ColorSequenceListenExit;
    bool LiftDoorExit;
    bool ColorSequenceExit;
    bool OneTimeLiftWorking;

    void Start()
    {
        ColorSequenceListenExit = false;
        LiftDoorExit = false;
        ColorSequenceExit = false;
        OneTimeLiftWorking = false;
        HamsterWheelActive = false;

        if (DoorObject != null)
            DoorOriginalHeight = DoorObject.transform.position.y;

        if (HamsterWheelAttachedObject)
            HamsterWheelAttachedObjectOriginalPos = HamsterWheelAttachedObject.transform.position.z;

        if (puzzleMode == PuzzleMode.ColorSequenceListen)
            originalListeningColorCount = new int[3] { ListeningColorCounts[0], ListeningColorCounts[1], ListeningColorCounts[2] };

        if (puzzleMode == PuzzleMode.ColorSequence)
            originalColorCount = new int[3] { ColorCounts[0], ColorCounts[1], ColorCounts[2] };

    }

#pragma warning disable 0168

    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Player")
        {

            try
            {
                if (!OneTimeLiftWorking)
                    StopCoroutine(operation);
            }
            catch (System.Exception e)
            {

            }

            try
            {
                StopCoroutine(secondaryOperation);
            }
            catch (System.Exception e)
            {

            }


            switch (puzzleMode)
            {
                case PuzzleMode.ColorSequence:
                    {
                        ColorSequenceExit = false;
                        break;
                    }
                case PuzzleMode.Door:
                    {
                        LiftDoorExit = false;
                        break;
                    }
                case PuzzleMode.ColorSequenceListen:
                    {
                        ColorSequenceListenExit = false;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            CallFunction();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            switch (puzzleMode)
            {
                case PuzzleMode.ColorSequence:
                    {
                        ColorCounts = new int[3] { originalColorCount[0], originalColorCount[1], originalColorCount[2] };
                        ColorSequenceExit = true;
                        break;
                    }
                case PuzzleMode.Door:
                    {
                        LiftDoorExit = true;
                        break;
                    }
                case PuzzleMode.ColorSequenceListen:
                    {

                        try
                        {
                            StopCoroutine(operation);
                        }
                        catch (System.Exception e)
                        {

                        }

                        try
                        {
                            StopCoroutine(secondaryOperation);
                        }
                        catch (System.Exception e)
                        {

                        }

                        secondaryOperation = StartCoroutine(ColorSequencePanelMoveBack());
                        ListeningColorCounts = new int[3] { originalListeningColorCount[0], originalListeningColorCount[1], originalListeningColorCount[2] };
                        ColorSequenceListenExit = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }

    public void CallFunction()
    {
        switch (puzzleMode)
        {
            case PuzzleMode.ColorSequence:
                {
                    operation = StartCoroutine(ColorSequence());
                    break;
                }
            case PuzzleMode.Door:
                {
                    operation = StartCoroutine(LiftDoor());
                    break;
                }
            case PuzzleMode.ColorSequenceListen:
                {
                    secondaryOperation = StartCoroutine(ColorSequencePanelMoveIn());
                    operation = StartCoroutine(ColorSequenceListen());
                    break;
                }
            case PuzzleMode.OneTimeDoor:
                {
                    if (!OneTimeLiftWorking)
                    {
                        OneTimeLiftWorking = true;
                        operation = StartCoroutine(LiftDoorOneTime());
                    }
                    break;
                }
            case PuzzleMode.PuzzleSequence:
                {
                    operation = StartCoroutine(PuzzleSequence());
                    break;
                }
            case PuzzleMode.HamsterWheel:
                {
                    HamsterWheelActive = true;
                    operation = StartCoroutine(HamsterWheel());
                    break;
                }
            default:
                {
                    break;
                }
        }

        if (PlayAudio)
        {
            transform.parent.GetComponent<AudioSource>().Play();
        }
    }

    private IEnumerator LiftDoor()
    {
        if (alternateAudio)
        {
            // transform.parent.GetComponent<AudioSource>().volume = 0.5f;
            // transform.parent.GetComponent<AudioSource>().PlayOneShot(alternateAudio);

            AudioSource.PlayClipAtPoint(alternateAudio, Camera.main.transform.position);
        }

        float p_step = (DoorDesiredHeight - transform.position.y) / (1 / DoorLiftSpeed);

        if (AssignedParticleSystem != null)
        {
            AssignedParticleSystem.GetComponent<ParticleSystem>().Play();
        }

        while (!LiftDoorExit)
        {
            while (DoorObject.transform.position.y < DoorDesiredHeight)
            {
                DoorObject.transform.Translate(Vector3.up * p_step, Space.World);

                yield return null;
            }

            yield return null;
        }

        while (DoorObject.transform.position.y > DoorOriginalHeight)
        {
            DoorObject.transform.Translate(Vector3.up * p_step * -1, Space.World);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator LiftDoorOneTime()
    {

        float p_step = (DoorDesiredHeight - transform.position.y) / (1 / DoorLiftSpeed);

        if (AssignedParticleSystem != null)
        {
            AssignedParticleSystem.GetComponent<ParticleSystem>().Play();
        }

        while (DoorObject.transform.position.y < DoorDesiredHeight)
        {
            DoorObject.transform.Translate(Vector3.up * p_step, Space.World);

            yield return null;
        }


        if (PlayAudio)
        {
            transform.parent.GetComponent<AudioSource>().Stop();
            PlayAudio = false;
        }

        if (alternateAudio)
        {
            // transform.parent.GetComponent<AudioSource>().volume = 0.5f;
            // transform.parent.GetComponent<AudioSource>().PlayOneShot(alternateAudio);

            AudioSource.PlayClipAtPoint(alternateAudio, DoorObject.transform.position);
        }

        yield return null;
    }

    private IEnumerator ColorSequence()
    {

        int p_index = 0;
        Color[] p_colors = new Color[3] { Color.red, Color.green, Color.blue };


        while (!ColorSequenceExit)
        {
            ColorObjects[p_index].GetComponent<MeshRenderer>().material.color = p_colors[p_index];

            ColorCounts[p_index]--;

            yield return new WaitForSeconds(0.5f);

            ColorObjects[p_index].GetComponent<MeshRenderer>().material.color = Color.white;

            AudioSource.PlayClipAtPoint(crystalSound[p_index], Camera.main.transform.position);

            yield return new WaitForSeconds(0.5f);

            if (ColorCounts[p_index] <= 0)
            {
                p_index++;
                if (p_index == 3)
                {
                    p_index = 0;
                    ColorCounts = new int[3] { originalColorCount[0], originalColorCount[1], originalColorCount[2] };
                }
            }
        }

        yield return null;
    }

    private IEnumerator ColorSequenceListen()
    {

        int p_index = 0;

        while (!ColorSequenceListenExit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.collider.gameObject == ListeningColorObjects[p_index])
                    {
                        hit.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().loop = false;
                        hit.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();

                        AudioSource.PlayClipAtPoint(crystalSound[p_index], Camera.main.transform.position,2);

                        ListeningColorCounts[p_index]--;
                        if (ListeningColorCounts[p_index] == 0)
                        {
                            if (p_index == 2)
                            {
                                if (AlternateAudio)
                                {
                                    GetComponent<AudioSource>().PlayOneShot(alternateAudio);
                                    AlternateAudio = false;
                                }
                                StopAllCoroutines();
                                operation = StartCoroutine(LiftDoor());
                                secondaryOperation = StartCoroutine(ColorSequencePanelMoveBack());
                            }
                            else
                            {
                                p_index++;
                            }
                        }
                    }
                    else
                    {
                        ListeningColorCounts = new int[3] { originalListeningColorCount[0], originalListeningColorCount[1], originalListeningColorCount[2] };
                        p_index = 0;
                    }
                }
            }
            yield return null;
        }

        yield return null;
    }

    private IEnumerator ColorSequencePanelMoveIn()
    {

        while (Vector3.Distance(ListeningColorObjectsPanel.transform.position, Camera.main.transform.position) > 6f)
        {
            ListeningColorObjectsPanel.transform.position = Vector3.MoveTowards(ListeningColorObjectsPanel.transform.position
                                                                                , Camera.main.transform.position + Vector3.forward * 5 + Vector3.down
                                                                                , Time.deltaTime * 5);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator ColorSequencePanelMoveBack()
    {

        while (Vector3.Distance(ListeningColorObjectsPanel.transform.position, ListeningColorObjectsPanelOriginalPos.position) > 0.1f)
        {
            ListeningColorObjectsPanel.transform.position = Vector3.MoveTowards(ListeningColorObjectsPanel.transform.position
                                                                                , ListeningColorObjectsPanelOriginalPos.position
                                                                                , Time.deltaTime * 5);

            yield return null;
        }

        ListeningColorObjectsPanel.transform.position = ListeningColorObjectsPanelOriginalPos.position;


        if (secondaryOperation != null)
            StopCoroutine(secondaryOperation);

        secondaryOperation = null;

        yield return null;
    }

    private IEnumerator PuzzleSequence()
    {

        float p_step = (PuzzleSequenceUpPosition - transform.position.y) / (1 / 0.05f);

        PuzzleSequenceIsDown[0] = PuzzleSequenceObjects[0].transform.position.y <= PuzzleSequenceDownPosition ? true : false;
        PuzzleSequenceIsDown[1] = PuzzleSequenceObjects[1].transform.position.y <= PuzzleSequenceDownPosition ? true : false;

        while (PuzzleSequenceIsDown[0] == true ? PuzzleSequenceObjects[0].transform.position.y < PuzzleSequenceUpPosition
              : PuzzleSequenceObjects[0].transform.position.y > PuzzleSequenceDownPosition)
        {

            for (int i = 0; i < PuzzleSequenceObjects.Length; i++)
            {
                PuzzleSequenceObjects[i].transform.Translate(Vector3.up * p_step * (PuzzleSequenceIsDown[i] == true ? 1 : -1), Space.World);
            }

            yield return null;
        }

        for (int i = 0; i < PuzzleSequenceObjects.Length; i++)
        {
            PuzzleSequenceObjects[i].transform.position = new Vector3(PuzzleSequenceObjects[i].transform.position.x
                                                                      , (PuzzleSequenceIsDown[i] == true ? PuzzleSequenceUpPosition : PuzzleSequenceDownPosition)
                                                                      , PuzzleSequenceObjects[i].transform.position.z);
        }

        operation = null;
        StopAllCoroutines();
        yield return null;
    }

    private IEnumerator HamsterWheel()
    {

        float p_wheelRotationSpeed = 1f;

        float p_step = (HamsterWheelAttachedObjectDesiredPos - HamsterWheelAttachedObject.transform.localPosition.z) / (1 / 0.01f);

        if (AssignedParticleSystem != null)
        {
            AssignedParticleSystem.GetComponent<ParticleSystem>().Play();
        }

        while (p_wheelRotationSpeed < 1.5f)
        {
            HamsterWheelObject.transform.RotateAround(HamsterWheelObject.transform.position, Vector3.back, p_wheelRotationSpeed * Time.deltaTime * 100);
            GameObject.Find("GearParent").transform.RotateAround(GameObject.Find("GearParent").transform.position, Vector3.back, p_wheelRotationSpeed * Time.deltaTime * 100);

            p_wheelRotationSpeed += Time.deltaTime;

            yield return null;
        }

        while (HamsterWheelActive && (HamsterWheelAttachedObject.transform.localPosition.z < HamsterWheelAttachedObjectDesiredPos))
        {
            HamsterWheelObject.transform.RotateAround(HamsterWheelObject.transform.position, Vector3.back, p_wheelRotationSpeed * Time.deltaTime * 100);
            GameObject.Find("GearParent").transform.RotateAround(GameObject.Find("GearParent").transform.position, Vector3.back, p_wheelRotationSpeed * Time.deltaTime * 100);

            HamsterWheelAttachedObject.transform.Translate(Vector3.forward * p_step, Space.World);

            yield return null;
        }

        yield return null;
    }
}
