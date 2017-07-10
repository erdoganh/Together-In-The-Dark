using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishControl : MonoBehaviour {


    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            SceneManager.LoadScene("Menu Cave");
        }
    }

}
