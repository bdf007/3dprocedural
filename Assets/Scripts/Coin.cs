using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Coin : MonoBehaviour
{

    public int coinValue = 5;
    private GameManager gameManager;
    private AudioSource coinPickupSound;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        coinPickupSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        RotateCoin();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(PlaysSoundAndDestroy());
            other.GetComponent<Player>().Addcoin(coinValue);
        }
    }

    IEnumerator PlaysSoundAndDestroy()
    {
        coinPickupSound.Play();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void RotateCoin()
    {
        transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    }
}
