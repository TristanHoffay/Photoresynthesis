using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    public Transform subject;
    [SerializeField]
    private float panSpeed = 5f;
    [SerializeField]
    private float leftBound = -9f;
    [SerializeField]
    private float rightBound = 9f;
    [SerializeField]
    private float bottomBound = 0f;
    [SerializeField]
    private float upperBound = 70f;
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
        if (Player.Instance.perishing && GameManager.Instance.hasWon)
        {
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, 10f, panSpeed * Time.deltaTime);

            Vector3 newPos = Vector3.Lerp(transform.position, subject.position, panSpeed * Time.deltaTime);
            newPos.z = -10;
            newPos.x = 0;
            transform.position = newPos;
        }
        else
        {
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, 5f, panSpeed * Time.deltaTime);
            if (subject == null)
                subject = Player.Instance.transform;
            Vector3 newPos = Vector3.Lerp(transform.position, subject.position, panSpeed * Time.deltaTime);
            newPos.z = -10;
            if (newPos.x < leftBound)
                newPos.x = leftBound;
            if (newPos.x > rightBound)
                newPos.x = rightBound;
            if (newPos.y < bottomBound)
                newPos.y = bottomBound;
            if (newPos.y > upperBound)
                newPos.y = upperBound;
            transform.position = newPos;
        }
    }
}
