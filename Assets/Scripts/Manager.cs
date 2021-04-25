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
        Play();
    }

    public void Play()
    {
        GameCamera.gameObject.SetActive(true);
        MenuCamera.gameObject.SetActive(false);

        StartCoroutine(HideMenu());
        StartCoroutine(DimLight());
        
    }

    IEnumerator DimLight()
    {
        yield return new WaitForSeconds(1f);
        for (float i = 1; i >= GameLightIntensity; i -= 0.01f)
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
}
