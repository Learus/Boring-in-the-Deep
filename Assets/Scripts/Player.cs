using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance { get { return _instance; }}

    #region Variables
    private Vector3 mousePosition;
    private Animator animator;

    private Rigidbody2D rb;
    private AudioSource EngineSound;

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
    public FollowPlayer followPLayerCamera;
    public float initialEnginePitch = 1f;
    public float breakEnginePitch = 0.6f;
    public float pitchOffset = 0.05f;
    private Task pitchCoroutine;
    private bool breaking;

    public Light2D Fire;

    [Header("Particle Settings")]
    public ParticleSystem Dust;
    public float DustNormalSpeed = 1f;
    public float DustBreakSpeed = 0.5f;
    public ParticleSystem Thruster;
    public float ThrusterNormalSpeed = 0.4f;
    public float ThrusterBreakSpeed = 0.15f;

    #endregion

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        StartCoroutine(CandleLight());
        sideSpeed = normalSideSpeed;
        rotationSpeed = normalRotationSpeed;
        EngineSound = this.GetComponent<AudioSource>();
        animator = this.GetComponent<Animator>();

        EngineSound.pitch = initialEnginePitch;
        pitchCoroutine = null;

        rb.velocity = Vector2.zero;

        Dust.Stop();
        Thruster.Stop();
    }


    public void Play()
    {
        EngineSound.Play();
        Dust.Play();
        Thruster.Play();
        animator.SetBool("Playing", true);
    }

    public void Pause()
    {
        EngineSound.Stop();
        Dust.Stop();
        Thruster.Stop();
        animator.SetBool("Playing", false);

        rb.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Game.Instance.playing || Game.Instance.pause) return;

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

        Dust.Play();

        if (Input.GetMouseButton(0))
        {
            Break();
        }
        else
        {
            Speed();
        }
    }

    public void Break()
    {
        animator.SetBool("Break", true);
        followPLayerCamera.offset = breakCameraOffset;
        sideSpeed = slowSideSpeed;
        rotationSpeed = slowRotationSpeed;
        Game.Instance.speed = Game.Instance.slowSpeed;

        if (pitchCoroutine!= null && !breaking) pitchCoroutine.Stop();
        
        pitchCoroutine = new Task(PitchEngine());
        breaking = true;

        var pm = Thruster.main;
        pm.startLifetime = ThrusterBreakSpeed;
        pm = Dust.main;
        pm.startLifetime = DustBreakSpeed;
    }

    public void Speed()
    {
        animator.SetBool("Break", false);
        followPLayerCamera.offset = Vector3.zero;
        sideSpeed = normalSideSpeed;
        rotationSpeed = normalRotationSpeed;
        Game.Instance.speed = Game.Instance.moveSpeed;

        if (pitchCoroutine!= null && breaking) pitchCoroutine.Stop();
        
        pitchCoroutine = new Task(PitchEngine(false));
        breaking = false;

        var pm = Thruster.main;
        pm.startLifetime = ThrusterNormalSpeed;
        pm = Dust.main;
        pm.startLifetime = DustNormalSpeed;
    }

    IEnumerator PitchEngine(bool down = true)
    {
        if (down)
        {
            for (float i = EngineSound.pitch; i >= breakEnginePitch; i -= pitchOffset)
            {
                EngineSound.pitch = i;
                yield return null;
            }
            EngineSound.pitch = breakEnginePitch;
        }
        else
        {
            for (float i = EngineSound.pitch; i <= initialEnginePitch; i += pitchOffset)
            {
                EngineSound.pitch = i;
                yield return null;
            }
            EngineSound.pitch = initialEnginePitch;
        }
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
