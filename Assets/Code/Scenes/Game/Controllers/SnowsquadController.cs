using System.Collections;
using UnityEngine;


public class SnowsquadController : MonoBehaviour {
    Transform player;
    float toggleDistance = 750f;
    bool isActive = false;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject[] snowmen = GetChildren();

        foreach (GameObject snowman in snowmen) {
            snowman.SetActive(false);
        }

        isActive = false;
    }
    
    void Update () {
        // For performance reasons, we hide snowmen that are too far from the player
        // to be seen or interacted with. The activation of snowmen is also staggered
        // over several frames with a coroutine for performance reasons.
        if (Vector3.Distance(transform.position, player.position) < toggleDistance) {
            if (!isActive) {
                GameObject[] snowmen = GetChildren();
                StartCoroutine(ActivateGameobjects(snowmen));
                isActive = true;
            }
        }
        else {
            if (isActive) {
                GameObject[] snowmen = GetChildren();
                StartCoroutine(DeactivateGameobjects(snowmen));
                isActive = false;
            }
        }
    }

    GameObject[] GetChildren() {
        GameObject[] children = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            children[i] = transform.GetChild(i).gameObject;
        }

        return children;
    }

    IEnumerator ActivateGameobjects(GameObject[] gameobjects) {
        foreach (GameObject gameobject in gameobjects) {
            gameobject.SetActive(true);
            yield return null;
        }
    }

    IEnumerator DeactivateGameobjects(GameObject[] gameobjects) {
        foreach (GameObject gameobject in gameobjects) {
            gameobject.SetActive(false);
            yield return null;
        }
    }
}
