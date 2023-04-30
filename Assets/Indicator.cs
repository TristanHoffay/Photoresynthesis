using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private Color energyColorMinus3;
    [SerializeField]
    private Color energyColorMinus1;
    [SerializeField]
    private Color energyColor0;
    [SerializeField]
    private Color energyColor1;
    [SerializeField]
    private Color energyColor2;
    [SerializeField]
    private Color energyColor3;
    [SerializeField]
    private Color energyColor4;


    private float startTime;
    private string text;
    private float speed = 10;
    private float duration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + duration)
        {
            Destroy(gameObject);
        }    
        else
        {
            transform.position += Vector3.up * Time.deltaTime * speed;
        }
    }
    public void SetText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
    public void SetColor(int index)
    {
        switch(index)
        {
            case -3: GetComponent<TextMeshProUGUI>().color = energyColorMinus3; break;
            case -1: GetComponent<TextMeshProUGUI>().color = energyColorMinus1; break;
            case 0: GetComponent<TextMeshProUGUI>().color = energyColor0; break;
            case 1: GetComponent<TextMeshProUGUI>().color = energyColor1; break;
            case 2: GetComponent<TextMeshProUGUI>().color = energyColor2; break;
            case 3: GetComponent<TextMeshProUGUI>().color = energyColor3; break;
            case 4: GetComponent<TextMeshProUGUI>().color = energyColor4; break;
        }
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetDuration(float duration)
    {
        this.duration = duration;
    }
}
