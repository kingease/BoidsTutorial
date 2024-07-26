using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAgent : MonoBehaviour
{
    // dynamic system related
    Vector2 direction;
    float speed;
    Vector2 force = Vector2.left;

    [Range(0, 2)]
    public float fSpeedScale = 1.0f;

    public float fForceScale = 0.0f;

    [HideInInspector]
    public float perceptionRadius = 1f;

    // screen related
    private Camera mainCamera;
    private Vector2 screenBottomLeft;
    private Vector2 screenTopRight;

    // Highlight related for demostration
    public bool isHighlighted;
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.red;
    public Color perceptedColor = Color.yellow;

    private GameObject perceptionCircle;

    private Renderer rend;

    private LineRenderer lineRenderer;

    private void updatePerceptionCircle()
    {
        perceptionCircle.transform.position = transform.position;
        perceptionCircle.transform.rotation = transform.rotation;
        perceptionCircle.transform.localScale = Vector3.one * perceptionRadius * 2;
    }
    private void Awake()
    {
        // get direction from rotation
        direction = transform.up;
        // screen bounds
        mainCamera = Camera.main;
        speed = 1.0f;
        perceptionRadius = 1f;



        //demostration
        isHighlighted = false;

        rend = GetComponent<Renderer>();
        rend.material.color = defaultColor;

        lineRenderer = GetComponent<LineRenderer>();

        perceptionCircle = new GameObject("PerceptionCircle");
        SpriteRenderer circleRender = perceptionCircle.AddComponent<SpriteRenderer>();
        circleRender.sprite = Resources.Load<Sprite>("Sprites/RangeHint");
        circleRender.color = new Color(1, 1, 1, 0.2f);
        perceptionCircle.SetActive(false);

        updatePerceptionCircle();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void UpdateArgs(float forceScale){
        fForceScale = forceScale;
    }

    public void SenseNeighbors(List<FishAgent> fishAgents)
    {
        List<FishAgent> neighbors = new List<FishAgent>();
        List<FishAgent> noNeighbors = new List<FishAgent>();
        foreach (FishAgent agent in fishAgents)
        {
            if (agent != this)
            {
                float dist = Vector3.Distance(agent.transform.position, transform.position);
                if (dist < perceptionRadius && agent != this)
                {
                    neighbors.Add(agent);
                }
                else
                {
                    noNeighbors.Add(agent);
                }
            }
            
        }

        // for display 
        if (isHighlighted)
        {
            lineRenderer.positionCount = neighbors.Count * 2;

            for(int i = 0; i < neighbors.Count; i++)
            {
                FishAgent agent = neighbors[i];
                agent.rend.material.color = perceptedColor;
                lineRenderer.SetPosition(2 * i, this.transform.position);
                lineRenderer.SetPosition(2*i+1, agent.transform.position);
            }

            foreach (FishAgent agent in noNeighbors)
            {
                agent.rend.material.color = defaultColor;
            }
        }
    }

    public void HighLight(bool isHighLight)
    {
        isHighlighted = isHighLight;
        rend.material.color = isHighlighted ? highlightColor : defaultColor;
        perceptionCircle.SetActive(isHighLight);
    }

    private void wrapPosition()
    {
        screenBottomLeft = mainCamera.ScreenToWorldPoint(
            new Vector3(0, 0, mainCamera.nearClipPlane)
            );
        screenTopRight = mainCamera.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane)
            );

        Vector3 pos = transform.position;
        if (transform.position.x < screenBottomLeft.x)
        {
            pos.x = screenTopRight.x;
        } else if (transform.position.x > screenTopRight.x)
        {
            pos.x = screenBottomLeft.x;
        }

        if (transform.position.y < screenBottomLeft.y)
        {
            pos.y = screenTopRight.y;
        } else if (transform.position.y > screenTopRight.y)
        {
            pos.y = screenBottomLeft.y;
        }
        transform.position = pos;
    }

    private void CalculateNextState() 
    {
        // caculate according to force dynamic
        Vector2 velocity = force * fForceScale * Time.deltaTime + speed * direction;

        // steer to velocity direction
        transform.up = velocity.normalized;

        // update direction
        direction = transform.up;
        // update speed
        speed = Mathf.Min(velocity.magnitude, 3f);


        // update new position
        transform.position += fSpeedScale * speed * Time.deltaTime * (Vector3)direction;
        wrapPosition();

        // demostration
        updatePerceptionCircle();
    }

    // Update is called once per frame
    void Update()
    {
        // formulation 
        CalculateNextState();
    }

}
