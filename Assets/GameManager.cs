using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    public GameObject potCenter;
    [SerializeField]
    private GameObject indicator;
    [SerializeField]
    private GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameWin()
    {
        Debug.Log("You win!!");
    }
    public void CreateIndicator(Vector3 position, string text, int colorIndex, float speed, float duration)
    {
        if (indicator != null && canvas != null)
        {
            Indicator newInd = Instantiate(indicator, Camera.main.WorldToScreenPoint(position), Quaternion.identity, canvas.transform).GetComponent<Indicator>();
            newInd.SetText(text);
            newInd.SetColor(colorIndex);
            newInd.SetSpeed(speed);
            newInd.SetDuration(duration);
        }
    }
}
