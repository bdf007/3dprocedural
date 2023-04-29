using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public AudioSource healthSound;

    private void Start()
    {
        healthSound = GetComponent<AudioSource>();
    }
    void Update()
    {
        RotateHealth();
    }

    public int healthToAdd;
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {   
        if(other.CompareTag("Player"))
        { 
            if (other.GetComponent<Player>().AddHealth(healthToAdd))
            {
                StartCoroutine(WaitAndPlay());
              
            }
        }
    }
// coroutine to wait 2 seconds after playing the sound
IEnumerator WaitAndPlay()
    {
        healthSound.Play();
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    private void RotateHealth()
    {
        transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    }
}
