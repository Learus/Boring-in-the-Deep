using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

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
    public Image Fader;
    public float pauseAlpha = 0.7f;
    public bool resetting = false;

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
        GameCamera.m_Lens.OrthographicSize = 6.2f;
        GameCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(0, 0, 0);

        MenuCamera.gameObject.SetActive(true);
        
        StartCoroutine(FadeIn());
        resetting = false;
    }

    void Update()
    {
        if (Game.Instance.cinematic) return;
        
        if (Input.GetMouseButton(0))
        {
            if (!Game.Instance.playing) Play();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Game.Instance.playing && !Game.Instance.pause && !resetting && !Game.Instance.cinematic)
            {
                Game.Instance.pause = true;
                Player.Instance.Pause();
            }
            else if (Game.Instance.playing && Game.Instance.pause && !resetting && !Game.Instance.cinematic)
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
        Beginning.SetActive(false);

        StartCoroutine(HideMenu());
        StartCoroutine(DimLight());

        Game.Instance.Play();
        Player.Instance.Play();
    }

    public void Reset()
    {
        resetting = false;
        GameCamera.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(true);
        Beginning.SetActive(true);
        Menu.gameObject.SetActive(true);
        Game.Instance.Reset();
        Player.Instance.Reset();
        GameCamera.m_Lens.OrthographicSize = 6.2f;
        GameCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(0, 0, 0);

        Beginning.transform.position = BeginningInitialPosition;
    }

    public void Lose()
    {
        Player.Instance.Lose();
        Game.Instance.pause = true;
        StartCoroutine(WaitToReset());
    }

    public void EndGame()
    {
        this.GetComponent<Cinematic>().Begin();
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
        if (!resetting)
            resetting = true;
        else yield break;

        yield return new WaitForSeconds(3f);
        Reset();
    }

    IEnumerator FadeIn()
    {
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            Color c = Fader.color;
            c.a = i;
            Fader.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        for (float i = 0; i <= 1; i += 0.05f)
        {
            Color c = Fader.color;
            c.a = i;
            Fader.color = c;
            yield return null;
        }
    }
}
