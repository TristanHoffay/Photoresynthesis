using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    public Transform subject;
    [SerializeField]
    private float panSpeed = 5f;
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
        if (subject == null)
            subject = Player.Instance.transform;
        Vector3 newPos = Vector3.Lerp(transform.position, subject.position, panSpeed * Time.deltaTime);
        newPos.z = -10;
        transform.position = newPos;
    }
}
