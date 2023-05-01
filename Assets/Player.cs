using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject stem;
    [SerializeField]
    private GameObject leaf;
    [SerializeField]
    private int leafCost = 3;
    [SerializeField]
    private int stemCost = 1;

    [SerializeField]
    private float perishDelay = 0.1f;
    private float nextPerish = 0;

    public static Player Instance;

    private Vector3 mousePos;
    private Vector3 objPos;
    private float mouseAngle;
    private bool perishing = false;
    private bool placingLeaf = false;
    private bool canPlaceOutline = true;
    private bool direction = false;

    private GameObject outline;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        outline = Instantiate(stem, transform.position, Quaternion.Euler(new Vector3(0, 0, mouseAngle)), transform);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            MazeSpawner.Instance.GenerateMaze();



        mousePos = Input.mousePosition;
        mousePos.z = 0;
        objPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objPos.x;
        mousePos.y -= objPos.y;
        mouseAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        //create steam or leaf outline pointing at cursor
        outline.transform.rotation = Quaternion.Euler(new Vector3(0, 0, mouseAngle));
        if (canPlaceOutline)
        {
            // interpolate opacity of outline between 0.2 and 0.8
            foreach (SpriteRenderer spr in outline.GetComponentsInChildren<SpriteRenderer>())
            {
                Color newColor = spr.color;
                newColor.a = Mathf.Lerp(0.2f, 0.6f, Mathf.PingPong(Time.time * 2, 1));
                spr.color = newColor;
            }
        }
        //maybe check to make sure direction is within certain degree of last stem
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !perishing)
        {
            if (placingLeaf)
            {
                if (!CheckWall(outline.GetComponent<Collider2D>()) && Energy.Instance.UseEnergy(leafCost, transform.position))
                {
                    GameObject newLeaf = Instantiate(leaf, transform.position, Quaternion.Euler(new Vector3(0, 0, mouseAngle + (direction ? 90f : -90f))), transform.parent);
                    transform.parent = newLeaf.transform;
                    // set level of nearby stems
                    for (int i = 0; i < 4; i++)
                    {
                        Transform dad = transform.parent;
                        for (int j = 0; j < i; j++)
                        {
                            dad = dad.parent.parent;
                        }
                        if (dad.CompareTag("Stem"))
                        {
                            dad.GetComponent<Stem>().IncreaseLevel(4 - i);
                        }
                        if (dad.CompareTag("Pot"))
                        {
                            break;
                        }
                    }
                    outline.transform.position = transform.position;
                    direction = Random.Range(0, 2) == 0;
                    outline.transform.GetChild(2).rotation = Quaternion.Euler(new Vector3(0, 0, mouseAngle + (direction ? 90f : -90f)));
                    GameObject newStem = Instantiate(stem, transform.position, Quaternion.Euler(new Vector3(0, 0, mouseAngle)), transform.parent);
                    Transform stemEnd = newStem.transform.GetChild(1).transform;
                    transform.position = stemEnd.position;
                    transform.parent = stemEnd;

                    CheckOutline();
                }
            }
            else if (transform.parent.parent.CompareTag("Stem") && transform.parent.parent.GetComponent<Stem>().level < 1)
            {
                Debug.Log("Cannot grow from dead stem");
            }
            else 
            {
                if (!CheckWall(outline.GetComponent<Collider2D>()) && Energy.Instance.UseEnergy(stemCost, transform.position))
                {
                    GameObject newStem = Instantiate(stem, transform.position, Quaternion.Euler(new Vector3(0, 0, mouseAngle)), transform.parent);
                    Transform stemEnd = newStem.transform.GetChild(1).transform;
                    transform.position = stemEnd.position;
                    transform.parent = stemEnd;

                    CheckOutline();
                }
            }
        }
        if (Input.GetMouseButtonDown(1) && !perishing)
        {
            placingLeaf = !placingLeaf;
            if (placingLeaf)
            {
                direction = Random.Range(0, 2) == 0;
                Instantiate(leaf, outline.transform.position, Quaternion.Euler(0, 0, mouseAngle + (direction ? 90f : -90f)), outline.transform);
            }
            else
            {
                Destroy(outline.transform.GetChild(2).gameObject);
            }
            CheckOutline();
        }

        // When perishing
        if (perishing && nextPerish < Time.time)
        {
            if (transform.parent.CompareTag("Pot"))
            {
                perishing = false;
                // make outline visible
                CheckOutline();
            }
            else if (transform.parent.CompareTag("Leaf"))
            {
                Transform leaf = transform.parent;
                transform.parent = transform.parent.parent;
                if (transform.parent.CompareTag("Pot"))
                {
                    Destroy(leaf.gameObject);
                }
                else
                {
                    // Move to lower stem
                    Stem oldStem = transform.parent.parent.GetComponent<Stem>();
                    transform.position = transform.parent.parent.parent.position;
                    transform.parent = transform.parent.parent.parent;
                    // Change above stem to falling
                    oldStem.falling = true;
                    oldStem.transform.parent = null;
                    oldStem.GetComponent<Rigidbody2D>().velocity = transform.up * 10f + transform.right * Random.Range(-10f, 10f);
                    oldStem.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-300, 300);
                    // Pause for a moment before next
                    nextPerish = Time.time + perishDelay;
                }
            }
            else
            {
                // Move to lower stem
                Stem oldStem = transform.parent.parent.GetComponent<Stem>();
                transform.position = transform.parent.parent.parent.position;
                transform.parent = transform.parent.parent.parent;
                // Change above stem to falling
                oldStem.falling = true;
                oldStem.transform.parent = null;
                oldStem.GetComponent<Rigidbody2D>().velocity = transform.up * 10f + transform.right * Random.Range(-10f, 10f);
                oldStem.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-300, 300);
                // Pause for a moment before next
                nextPerish = Time.time + perishDelay;
            }
        }
    }
    public void Perish()
    {
        Instance.perishing = true;
        // make outline invisible
        foreach (SpriteRenderer spr in outline.GetComponentsInChildren<SpriteRenderer>())
        {
            Color newColor = spr.color;
            newColor.a = 0f;
            spr.color = newColor;
            canPlaceOutline = false;
        }
    }
    public void CheckOutline()
    {
        if (placingLeaf)
        {
            if (Energy.Instance.CheckEnergyUse(leafCost))
            {
                // if can place leaf, make it fade in and out
                canPlaceOutline = true;
            }
            else
            {
                foreach (SpriteRenderer spr in outline.GetComponentsInChildren<SpriteRenderer>())
                {
                    Color newColor = spr.color;
                    newColor.a = 0.2f;
                    spr.color = newColor;
                    canPlaceOutline = false;
                }
            }
        }
        else
        {
            if (Energy.Instance.CheckEnergyUse(stemCost))
            {
                // if can place leaf, make it fade in and out
                canPlaceOutline = true;
            }
            else
            {
                Color newColor = outline.GetComponentInChildren<SpriteRenderer>().color;
                newColor.a = 0.2f;
                outline.GetComponentInChildren<SpriteRenderer>().color = newColor;
                canPlaceOutline = false;
            }
        }
    }
    public bool CheckWall(Collider2D ourCollider)
    {
        Collider2D[] cols = new Collider2D[10];
        ContactFilter2D cf = new ContactFilter2D();
        cf.NoFilter();
        int found = ourCollider.OverlapCollider(cf, cols);
        for (int i = 0; i < found; i++)
        {
            if (cols[i].gameObject.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
}
