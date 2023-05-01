using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private float[] parallaxFactors = new float[10];

    private SpriteRenderer[] backgrounds;
    
    // Start is called before the first frame update
    void Start()
    {
        backgrounds = transform.GetComponentsInChildren<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.z = 0;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.localPosition = cameraPos / parallaxFactors[i];  // the larger the factor, closer to camera
        }
    }
}
