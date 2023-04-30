using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stem : MonoBehaviour
{
    [SerializeField]
    private GameObject indicator;
    [SerializeField]
    private Color aliveColor;
    [SerializeField]
    private Color deadColor;

    public bool falling = false, kill = false;
    public int level = 0;
    private Transform potCenter;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        potCenter = GameManager.Instance.potCenter.transform;
        rb = GetComponent<Rigidbody2D>();

        GetComponentInChildren<SpriteRenderer>().color = deadColor;
        //check for lower stems
        for (int i = 0; i < 4; i++)
        {
            Transform dad = transform.parent;
            for (int j = 0; j < i; j++)
            {
                dad = dad.parent.parent;
            }
            if (dad.CompareTag("Leaf") || dad.CompareTag("Pot"))
            {
                SetLevel(4 - i);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            Vector3 distance = (potCenter.position - transform.position);
            rb.velocity += new Vector2(distance.x, distance.y)  * Time.deltaTime;
            rb.velocity = Vector3.Lerp(rb.velocity, distance.normalized * 5, Time.deltaTime * 5);
        }
        else if (kill)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 10);
            transform.localScale *= .99f * (1 - Time.deltaTime);
            if (transform.localScale.magnitude < .01)
                Destroy(gameObject);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (falling)
        {
            if (col.gameObject.CompareTag("Pot"))
            {
                Energy.Instance.AddEnergy(level);
                // Create some effect; shrink into nothing, spawn text saying +4 or whatever energy level
                GameManager.Instance.CreateIndicator(transform.position, "+" + level + " Energy", level, 100f, 1f);
                falling = false;
                kill = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Sun"))
        {
            GameManager.Instance.GameWin();
        }
    }
    public void SetLevel(int level)
    {
        this.level = level;
        GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(deadColor, aliveColor, Mathf.Pow((float)level / 4f, 2));
    }
    public void IncreaseLevel(int level)
    {
        if (level > this.level)
            SetLevel(level);
    }
}
