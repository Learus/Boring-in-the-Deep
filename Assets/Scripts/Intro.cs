using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Intro : MonoBehaviour
{

    public AudioSource Music;
    public AudioSource Voice;
    public GameObject PhileasLight;
    public GameObject PassepartoutLight;
    public Text text;

    public Image Fader;

    public List<AudioClip> VoiceSequence;
    public List<string> TextSequence;

    Cinematic.Sync sync;
    // Start is called before the first frame update
    void Start()
    {
        PhileasLight.SetActive(false);
        PassepartoutLight.SetActive(false);

        List<Cinematic.Sync.SyncTuple> actions = new List<Cinematic.Sync.SyncTuple>();
        int curVoice = 0;

        UnityAction PhileasVoice = () => {
            text.text = TextSequence[curVoice];
            text.alignment = TextAnchor.MiddleLeft;
            Voice.Stop();
            PassepartoutLight.SetActive(false);
            PhileasLight.SetActive(true);
            Voice.clip = VoiceSequence[curVoice];
            Voice.Play();
            curVoice++;
        };

        UnityAction PassepartoutVoice = () => {
            text.text = TextSequence[curVoice];
            text.alignment = TextAnchor.MiddleRight;
            Voice.Stop();
            PhileasLight.SetActive(false);
            PassepartoutLight.SetActive(true);
            Voice.clip = VoiceSequence[curVoice];
            Voice.Play();
            curVoice++;
        };

        actions.Add(new Cinematic.Sync.SyncTuple(3f, PhileasVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(8.631f, PassepartoutVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(14.076f, PhileasVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(29.242f, PassepartoutVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(33.143f, PhileasVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(43.818f, PassepartoutVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(48.063f, PhileasVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(54.361f, PassepartoutVoice));
        actions.Add(new Cinematic.Sync.SyncTuple(57.075f, () => {
            StartCoroutine(FadeToPlay());
        }));

        sync = new Cinematic.Sync(actions);

        StartCoroutine(Synchronizer());
    }

    public void Play()
    {

        SceneManager.LoadScene(1);
    }

    IEnumerator Synchronizer()
    {
        Cinematic.Sync.SyncTuple action = sync.next();

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

    IEnumerator MusicFade()
    {
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            Music.volume = i;
            yield return null;
        }
    }

    IEnumerator FadeToPlay()
    {
        for (float i = 0; i <= 1; i++)
        {
            Color c = Fader.color;
            c.a = i;
            Fader.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Play();
    }
}
