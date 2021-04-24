using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    private Vector3 mousePosition;
    public float sideSpeed = 1f;
    private Rigidbody2D rb;

    public Light2D Fire;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        StartCoroutine(CandleLight());
    }

    // Update is called once per frame
    void Update()
    {
        // Figure out new position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        Vector2 velocity = new Vector2(direction.x * sideSpeed, 0);
        Vector3 target = transform.position + new Vector3(velocity.x, -Game.Instance.moveSpeed, 0);
        
        // Rotate sprite towards new position
        Vector3 perpendicular = transform.position - target;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);

        // Move the player
        rb.velocity = velocity;
    }
    IEnumerator CandleLight()
    {
        while (true) {
            float random = Random.Range(1, 2);
            Fire.intensity = Mathf.Lerp(Fire.intensity, random, Time.deltaTime);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
