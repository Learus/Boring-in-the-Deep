using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    private static Game _instance;
    public static Game Instance { get { return _instance; }}

    public List<GameObject> ActiveGame;

    [Header("Dirt")]
    public List<GameObject> DirtTemplates;
    public List<GameObject> DirtTransition;
    public List<GameObject> DirtBoss;
    [Header("Rock")]
    public List<GameObject> RockTemplates;
    public List<GameObject> RockTransition;
    public List<GameObject> RockBoss;
    [Header("Lava")]
    public List<GameObject> LavaTemplates;
    public List<GameObject> LavaTransition;
    public List<GameObject> LavaBoss;

    [Header("Game")]

    public float moveSpeed = 5f;
    public float slowSpeed = 2.5f;
    public float speed = 5f;

    [Header("Generation - Optimization")]
    public int ActiveTemplates = 5;
    public float templatesHeightDifference = 10f;
    public int activeLayer;
    public List<Dictionary<string, List<GameObject>>> Templates;

    public int levelsPerLayer = 20;
    public int currentGeneratedLevel;

    public bool playing = false;
    public bool pause = false;


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
        Dictionary<string, List<GameObject>> dirt = new Dictionary<string, List<GameObject>>();
        dirt["Templates"] = DirtTemplates;
        dirt["Transition"] = DirtTransition;
        dirt["Boss"] = DirtBoss;

        Dictionary<string, List<GameObject>> rock = new Dictionary<string, List<GameObject>>();
        rock["Templates"] = RockTemplates;
        rock["Transition"] = RockTransition;
        rock["Boss"] = RockBoss;

        Dictionary<string, List<GameObject>> lava = new Dictionary<string, List<GameObject>>();
        lava["Templates"] = LavaTemplates;
        lava["Transition"] = LavaTransition;
        lava["Boss"] = LavaBoss;

        Templates = new List<Dictionary<string, List<GameObject>>>();
        Templates.Add(dirt);
        Templates.Add(rock);
        Templates.Add(lava);

        playing = false;
        pause = false;
    }

    void Update()
    {
        if (!playing || pause) return;

        this.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
        Manager.Instance.Beginning.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    public void Play()
    {
        playing = true;

        activeLayer = 0;
        currentGeneratedLevel = 0;

        ClearGame();
        InitialGenerate();
        speed = moveSpeed;
    }

    public void EnteredNewTemplate(TilemapTemplate template)
    {
        try
        {
            int id = int.Parse(template.name);
            if (id - 8 >= 0)
            {
                Destroy(ActiveGame[0]);
                ActiveGame.RemoveAt(0);
                Manager.Instance.Beginning.SetActive(false);
            }
        }
        catch (System.Exception) {}

        if (currentGeneratedLevel >= levelsPerLayer)
        {
            GenerateBoss();
            currentGeneratedLevel = 0;
            activeLayer++;
            return;
        }

        if (template.type != TilemapTemplate.TemplateType.Boss && template.type != TilemapTemplate.TemplateType.Transition)
        {
            currentGeneratedLevel++;
        }

        Generate(ActiveGame.Count);
    }

    public void InitialGenerate()
    {
        ActiveGame = new List<GameObject>();

        for (int i = 0; i < ActiveTemplates; i++)
        {
            Generate(i);
            currentGeneratedLevel++;
        }
    }

    public void Generate(int index = 0)
    {
        List<GameObject> TemplateList = Templates[activeLayer]["Templates"];

        TilemapTemplate last = ActiveGame.Count != 0 ? ActiveGame[ActiveGame.Count - 1].GetComponent<TilemapTemplate>() : null;

        Vector3 targetPosition = Vector3.zero;
        int randomIndex = 0;
        // If we can, generate a template that comes next from our tile.
        if (last != null)
        {
            targetPosition = new Vector3(last.transform.position.x, last.transform.position.y - templatesHeightDifference, last.transform.position.z);

            if (last.NextDependencies.Count != 0)
            {
                randomIndex = last.NextDependencies[Random.Range(0, last.NextDependencies.Count)];
            }
        }
        // If not, generate a template that can stand alone.
        else
        {
            randomIndex = Random.Range(0, TemplateList.FindAll(template => template.GetComponent<TilemapTemplate>().PrevDependencies.Count == 0).Count);
        }

        GameObject newTemplate = Instantiate(
            TemplateList[randomIndex],
            targetPosition,
            Quaternion.identity,
            this.transform
        );
        newTemplate.name = index.ToString();

        ActiveGame.Add(newTemplate);
    }

    public void GenerateTransition()
    {

    }

    public void GenerateBoss()
    {
        // Among other things
        GenerateTransition();
    }

    public void ClearGame()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
