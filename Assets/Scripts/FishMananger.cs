using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMananger : MonoBehaviour
{
    public FishAgent fishAgentPrefab;

    [Range(10, 100)]
    public int fishCount;

    [Range(0, 15)]
    public float fForceScale = 0.0f;

    [Range(1, 5)]
    public float neighborRadius;

    public Material maskMaterial;

    private List<FishAgent> agents;
    private FishAgent highLightedFish;
    private int currentIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        agents = new List<FishAgent>();
        for (int i = 0; i < fishCount; ++i)
        {
            Vector2 pos = Random.insideUnitCircle;
            FishAgent agent = Instantiate(fishAgentPrefab,
                new Vector3(pos.x, pos.y),
                Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            agent.transform.localScale = Vector3.one * 0.03f;
            agents.Add(agent);
        }

        HighLightFish(10);
    }

    void HighLightFish(int index)
    {
        if (highLightedFish != null)
        {
            highLightedFish.HighLight(false);
        }

        if (index >= 0 && index < fishCount)
        {
            highLightedFish = agents[index];
            highLightedFish.HighLight(true);
            currentIndex = index;
        }
        else
        {
            highLightedFish.HighLight(false);
            currentIndex = -1;
        }
    }

    void OnRenderObject()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach(FishAgent agent in agents){
            agent.UpdateArgs(fForceScale);
            agent.SenseNeighbors(agents);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (currentIndex >= 0)
            {
                HighLightFish(10);
            }else
            {
                HighLightFish(-1);
            }
        }
    }
}