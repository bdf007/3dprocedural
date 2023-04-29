using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Key : MonoBehaviour
{

    // update is called once per frame
    void Update()
    {
        RotateKey();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().hasKey = true;
            // set the key to active
            UI.instance.ToggleKeyIcon(true);
            Destroy(gameObject);
        }
    }

    private void RotateKey()
    {
        transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    }
}
