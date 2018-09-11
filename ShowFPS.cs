using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    TMP_Text labelText;
    float deltaTime = 0.0f;
    int cooldownTime = 0;

    void Start()
    {
        labelText = gameObject.GetComponent<TMP_Text>();
    }

    void Update()
    {
        cooldownTime++;
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        if (cooldownTime % 10 == 0)
        {
            cooldownTime = 0;
            string fpsText = string.Format("{0:0.} fps ({1:0.} ms)", fps, msec);
            labelText.text = fpsText;
        }
    }
}
