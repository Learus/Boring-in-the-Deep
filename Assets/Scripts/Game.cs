using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;
    public static Game Instance { get { return _instance; }}

    public Transform Background;

    public List<GameObject> Templates;
    public List<GameObject> ActiveGame;

    public float templatesHeightDifference = 10f;
    public float moveSpeed = 5f;
    public float slowSpeed = 2.5f;
    public float speed = 5f;
    public int ActiveTemplates = 5;


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
        ClearGame();
        InitialGenerate();
        speed = moveSpeed;
    }

    void Update()
    {
        this.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    public void EnteredNewTemplate(string templateName)
    {
        try
        {
            int id = int.Parse(templateName);
            if (id - 8 >= 0)
            {
                Destroy(ActiveGame[0]);
                ActiveGame.RemoveAt(0);
            }
        }
        catch (System.Exception) {}
        
        Generate(ActiveGame.Count);
    }

    public void InitialGenerate()
    {
        ActiveGame = new List<GameObject>();

        for (int i = 0; i < ActiveTemplates; i++)
        {
            Generate(i);
        }
    }

    public void Generate(int index = 0)
    {
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
            randomIndex = Random.Range(0, Templates.FindAll(template => template.GetComponent<TilemapTemplate>().PrevDependencies.Count == 0).Count);
        }

        GameObject newTemplate = Instantiate(
            Templates[randomIndex],
            targetPosition,
            Quaternion.identity,
            this.transform
        );
        newTemplate.name = index.ToString();

        ActiveGame.Add(newTemplate);
    }

    public void ClearGame()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
