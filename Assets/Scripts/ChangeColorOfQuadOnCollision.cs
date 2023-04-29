using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOfQuadOnCollision : MonoBehaviour
{
    public Color newColor;

    private Color originalColor;
    private bool isColliding;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isColliding = true;
            //Renderer renderer = GetComponent<Renderer>();
            //// deactivate the renderer
            //renderer.enabled = true;
            //Color targetColor = new Color(newColor.r, newColor.g, newColor.b, originalColor.a);
            //renderer.material.color = targetColor;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isColliding = false;
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.enabled = false;
            //renderer.material.color = originalColor;
        }
    }

    private void Update()
    {
            Renderer renderer = GetComponent<Renderer>();
        if (isColliding)
        {
            renderer.enabled = true;
            //Color currentColor = renderer.material.color;
            //Color targetColor = new Color(newColor.r, newColor.g, newColor.b, originalColor.a);
            //renderer.material.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 5f);
        }
        else
        {
            renderer.enabled=false;
        }
    }
}
