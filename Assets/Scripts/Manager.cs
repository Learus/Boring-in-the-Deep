using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance { get { return _instance; }}

    public Cinemachine.CinemachineVirtualCamera GameCamera;
    public Light2D GameLight;
    public float GameLightIntensity = 0.24f;
    public Cinemachine.CinemachineVirtualCamera MenuCamera;
    public Canvas Menu;
    public GameObject Beginning;
    public Vector3 BeginningInitialPosition = new Vector3(0, 14, 0);

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
        GameCamera.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!Game.Instance.playing) Play();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!Game.Instance.pause)
            {
                Game.Instance.pause = true;
                Player.Instance.Pause();
            }
            else
            {
                Game.Instance.pause = false;
                Player.Instance.Play();
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) Reset();
    }

    public void Play()
    {
        GameCamera.gameObject.SetActive(true);
        MenuCamera.gameObject.SetActive(false);

        StartCoroutine(HideMenu());
        StartCoroutine(DimLight());

        Game.Instance.Play();
        Player.Instance.Play();
    }

    public void Reset()
    {
        GameCamera.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(true);
        Beginning.SetActive(true);
        Menu.gameObject.SetActive(true);
        Game.Instance.Reset();
        Player.Instance.Reset();

        Beginning.transform.position = BeginningInitialPosition;
        GameLight.intensity = 1;
    }

    public void Lose()
    {
        Player.Instance.Lose();
        Game.Instance.pause = true;
        StartCoroutine(WaitToReset());
    }

    IEnumerator DimLight()
    {
        yield return new WaitForSeconds(1f);
        for (float i = GameLight.intensity; i >= GameLightIntensity; i -= 0.01f)
        {
            GameLight.intensity = i;
            yield return null;
        }
    }

    IEnumerator HideMenu()
    {
        Menu.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator WaitToReset()
    {
        yield return new WaitForSeconds(3f);
        Reset();
    }
}
