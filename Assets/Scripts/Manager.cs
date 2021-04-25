using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Camera GameCamera;
    public GameObject GameLight;
    public Camera MenuCamera;
    public Canvas Menu;
    // Start is called before the first frame update
    void Start()
    {
        GameCamera.gameObject.SetActive(false);
        GameLight.SetActive(false);


        MenuCamera.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {

    }

    IEnumerator HideMenu()
    {
        yield break;
    }
}
