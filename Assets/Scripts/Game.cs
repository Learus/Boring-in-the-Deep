using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    private static Game _instance;
    public static Game Instance { get { return _instance; }}

    #region Variables

    public List<GameObject> ActiveGame;

    public Vector3 InitialPosition = new Vector3(0, 0, -1);

    public List<GameObject> InitialTemplates;

    [Header("Dirt")]
    public List<GameObject> DirtTemplates;
    [Header("Rock")]
    public List<GameObject> RockTemplates;
    [Header("Lava")]
    public List<GameObject> LavaTemplates;
    [Header("Void")]
    public List<GameObject> VoidTemplates;

    [Header("Game")]

    public float moveSpeed = 5f;
    public float slowSpeed = 2.5f;
    public float speed = 5f;

    [Header("Generation - Optimization")]
    public int ActiveTemplates = 5;
    public float templatesHeightDifference = 10f;
    public int activeLayer;
    public int startLayer = -1;
    public List<List<GameObject>> Templates;

    public int levelsPerLayer = 20;
    public int currentGeneratedLevel;
    public int playerIsInLevel = 0;

    public bool playing = false;
    public bool pause = false;

    public bool winning = false;
    public bool cinematic = false;

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

    void Start()
    {

        Templates = new List<List<GameObject>>();
        Templates.Add(InitialTemplates);
        Templates.Add(DirtTemplates);
        Templates.Add(RockTemplates);
        Templates.Add(LavaTemplates);
        Templates.Add(VoidTemplates);

        playing = false;
        pause = false;
        winning = false;
        cinematic = false;

        ClearGame();
        InitialGenerate();
    }

    void Update()
    {
        if (pause) return;

        this.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
        Manager.Instance.Beginning.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    public void Play()
    {
        playing = true;

        activeLayer = 0;

        winning = true;
        currentGeneratedLevel = 0;
        playerIsInLevel = -1;

        ClearGame();
        InitialGenerate();
        speed = moveSpeed;
    }

    public void Reset()
    {
        activeLayer = 0;
        this.transform.position = InitialPosition;

        playing = false;
        pause = false;
        winning = false;
        cinematic = false;

        currentGeneratedLevel = 0;
        playerIsInLevel = -1;
        
        ActiveGame = new List<GameObject>();
        ClearGame();
        InitialGenerate();
        speed = moveSpeed;
    }

    public void EnteredNewTemplate(TilemapTemplate template)
    {
        playerIsInLevel++;

        if (playerIsInLevel > 5)
        {
            Destroy(ActiveGame[0]);
            ActiveGame.RemoveAt(0);
        }

        if (playing && currentGeneratedLevel >= levelsPerLayer)
        {
            winning = true;
            currentGeneratedLevel = 0;
        }
        currentGeneratedLevel++;

        Generate(ActiveGame.Count);
    }

    public void InitialGenerate()
    {
        for (int i = 0; i < ActiveTemplates; i++)
        {
            Generate(i);
            currentGeneratedLevel++;
        }
    }

    public void Generate(int index = 0)
    {
        List<GameObject> TemplateList = Templates[activeLayer];

        TilemapTemplate last = ActiveGame.Count != 0 ? ActiveGame[ActiveGame.Count - 1].GetComponent<TilemapTemplate>() : null;

        Vector3 targetPosition = Vector3.zero;
        int randomIndex = 0;

        // If we can, generate a template that comes next from our tile.
        if (last != null)
        {
            targetPosition = new Vector3(last.transform.position.x, last.transform.position.y - templatesHeightDifference, last.transform.position.z);

            // THIS IS THE END
            if (winning && last.WinIndex == -1)
            {
                // We're doing this to test specific layers
                if (startLayer != -1)
                {
                    activeLayer = startLayer;
                    startLayer = -1;
                }
                else activeLayer++;

                winning = false;
                currentGeneratedLevel = 0;
                TemplateList = Templates[activeLayer];
                StartCoroutine(Player.Instance.TransitionColors(Player.Instance.LayerColors[activeLayer]));
            }

            if (winning)
            {
                randomIndex = last.WinIndex;
            }
            else if (last.NextDependencies.Count != 0)
            {
                randomIndex = last.NextDependencies[Random.Range(0, last.NextDependencies.Count)];
            }
        }
        // If not, generate a template that can stand alone.
        else
        {
            randomIndex = Random.Range(0, TemplateList.FindAll(template => template.GetComponent<TilemapTemplate>().PrevDependencies.Count == 0).Count);
        }

        GameObject toSpawn = TemplateList[randomIndex];

        GameObject newTemplate = Instantiate(
            toSpawn,
            targetPosition,
            Quaternion.identity,
            this.transform
        );
        newTemplate.name = TemplateList[randomIndex].name;

        ActiveGame.Add(newTemplate);
    }

    public void ClearGame()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        ActiveGame.Clear();
    }
}
