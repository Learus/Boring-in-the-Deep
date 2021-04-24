using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    private Vector3 mousePosition;
    public FollowPlayer followPLayerCamera;

    [Header("Rotation Speed")]
    public float normalRotationSpeed = 5f;
    public float slowRotationSpeed = 2.5f;
    public float rotationSpeed = 5f;

    [Header("Side Movement Speed")]
    public float normalSideSpeed = 10f;
    public float slowSideSpeed = 5f;
    public float sideSpeed = 10f;

    [Header("Break Stuff")]
    public Vector3 breakCameraOffset;
    private Rigidbody2D rb;

    public Light2D Fire;
    public ParticleSystem Dust;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        StartCoroutine(CandleLight());
        sideSpeed = normalSideSpeed;
        rotationSpeed = normalRotationSpeed;
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
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
       
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Move the player
        rb.velocity = velocity;

        Speed();
        CreateDust();

        if (Input.GetMouseButton(0))
        {
            Break();
        }
    }

    public void Break()
    {
        followPLayerCamera.offset = breakCameraOffset;
        sideSpeed = slowSideSpeed;
        rotationSpeed = slowRotationSpeed;
        Game.Instance.speed = Game.Instance.slowSpeed;
    }

    public void Speed()
    {
        followPLayerCamera.offset = Vector3.zero;
        sideSpeed = normalSideSpeed;
        rotationSpeed = normalRotationSpeed;
        Game.Instance.speed = Game.Instance.moveSpeed;
    }

    public void CreateDust()
    {
        Dust.Play();
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
