using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cinematic : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera cam;
    public Cinemachine.CinemachineVirtualCamera zoomedOutCamera;

    public AudioClip StopEngine;
    public GameObject PlayerLights;

    public GameObject god;
    public float initialGodY = -60f;
    public float openEyesY = -32.7f;
    public float openMouthY = -15f;
    public GameObject player;

    Sync sync;
    public AudioSource Music;
    public AudioSource Boom;

    public Image Fader;

    public class Sync
    {
        public struct SyncTuple
        {
            public float timeStamp;
            public UnityAction action;

            public SyncTuple(float _timeStamp, UnityAction _action)
            {
                timeStamp = _timeStamp;
                action = _action;
            }
        }

        public List<SyncTuple> actions;

        private int currentAction;

        public SyncTuple next()
        {
            return actions[currentAction++];
        }

        public bool Done()
        {
            return currentAction == actions.Count;
        }

        public Sync(List<SyncTuple> _actions)
        {
            currentAction = 0;
            actions = _actions;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        zoomedOutCamera.gameObject.SetActive(false);
    }

    public void Begin()
    {   
        Game.Instance.cinematic = true;
        god.transform.position = new Vector3(god.transform.position.x, initialGodY, god.transform.position.z);

        Player.Instance.rb.velocity = Vector2.zero;
        Player.Instance.transform.position = new Vector3(0, Player.Instance.transform.position.y, Player.Instance.transform.position.z);
        Player.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);

        List<Sync.SyncTuple> actions = new List<Sync.SyncTuple>();

        // Sequence;
        actions.Add(new Sync.SyncTuple(0f, () => {
            Player.Instance.animator.SetBool("VoidIdle", true);
            Player.Instance.Dust.Stop();
        }));
        actions.Add(new Sync.SyncTuple(10f, () => {
            Player.Instance.EngineSound.clip = StopEngine;
            Player.Instance.EngineSound.loop = false;
            Player.Instance.EngineSound.Play();
        }));
        // Stop Engine
        actions.Add(new Sync.SyncTuple(16f, () => {
            Player.Instance.animator.SetBool("StopEngine", true);
        }));
        // Flicker Lights
        actions.Add(new Sync.SyncTuple(18f, () => {
            PlayerLights.SetActive(false);
            Player.Instance.Thruster.Stop();
        }));
        actions.Add(new Sync.SyncTuple(18.3f, () => {
            PlayerLights.SetActive(true);
            Player.Instance.Thruster.Play();
        }));
        actions.Add(new Sync.SyncTuple(19f, () => {
            PlayerLights.SetActive(false);
            Player.Instance.Thruster.Stop();
        }));
        actions.Add(new Sync.SyncTuple(19.6f, () => {
            PlayerLights.SetActive(true);
            Player.Instance.Thruster.Play();
        }));
        actions.Add(new Sync.SyncTuple(20f, () => {
            PlayerLights.SetActive(false);
            Player.Instance.Thruster.Stop();
        }));
        actions.Add(new Sync.SyncTuple(21f, () => {
            god.GetComponent<AudioSource>().Play();
        }));
        // Zoom out
        actions.Add(new Sync.SyncTuple(23f, () => {
            zoomedOutCamera.gameObject.SetActive(true);
        }));
        // Move God
        actions.Add(new Sync.SyncTuple(33f, () => {
            StartCoroutine(MoveGod(initialGodY, openEyesY, 10f));
        }));
        actions.Add(new Sync.SyncTuple(50.25f, () => {
            god.GetComponent<Animator>().SetBool("Wake", true);
        }));
        actions.Add(new Sync.SyncTuple(60f, () => {
            StartCoroutine(MoveGod(openEyesY, openMouthY, 8f));
        }));
        actions.Add(new Sync.SyncTuple(70f, () => {
            god.GetComponent<Animator>().SetBool("Smile", true);
        }));
        actions.Add(new Sync.SyncTuple(73f, () => {
            Color c = Fader.color;
            c.a = 1;
            Fader.color = c;

            // Play Boom sound
            Boom.Play();
        }));
        actions.Add(new Sync.SyncTuple(88, () => {
            Application.Quit();
        }));

        sync = new Sync(actions);
        
        // Add actions

        Music.Play();
        StartCoroutine(Synchronizer());
    }

    IEnumerator MoveGod(float start, float end, float duration)
    {
        float offset = 0.1f;
        float secondsToWait = duration / ((end - start) / offset);

        for (float i = start; i <= end; i += offset)
        {
            god.transform.position = new Vector3(god.transform.position.x, i, god.transform.position.z);
            yield return new WaitForSeconds(secondsToWait);
        }
    }


    IEnumerator Synchronizer()
    {
        Sync.SyncTuple action = sync.next();

        while (true)
        {
            if (Music.time >= action.timeStamp)
            {
                action.action();

                if (sync.Done()) yield break;

                action = sync.next();
            }

            yield return null;
        }
    }
}
