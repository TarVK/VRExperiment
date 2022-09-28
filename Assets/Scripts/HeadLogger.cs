using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class HeadLogger : MonoBehaviour
{
    public GameObject focus;
    public double timesPerSec = 5;

    private Boolean active = false;
    private DateTime last = System.DateTime.Now;
    private string fileName = "";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Transform headTransform = gameObject.transform;

        RaycastHit hit;
        if (active && Physics.Raycast(headTransform.position, headTransform.TransformDirection(Vector3.forward), out hit, 20))
        {
            if (focus != null)
            {
                focus.SetActive(true);
                focus.transform.position = hit.point;
            }

            DateTime now = System.DateTime.Now;
            if (now.Subtract(last).Milliseconds > 1000 / timesPerSec)
            {
                AddToLog(fileName, hit.point.ToString() + "@" + now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
                last = now;
            }
        }
        else
        {
            if (focus != null) focus.SetActive(false);
        }

        // Check for buttons
        if (Input.GetButtonDown("XRI_Right_PrimaryButton"))
            setActive(!active);
    }

    void setActive(Boolean active)
    {
        this.active = active;
        if (active) {
            fileName = System.DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
        }
    }

    void AddToLog(string fileName, string text)
    {
        string basePath = Application.platform == RuntimePlatform.WindowsEditor ? Application.dataPath +"/../data" : Application.persistentDataPath;
        string path = basePath + "/" + fileName + ".txt";
        Debug.Log(path);
        System.Text.UnicodeEncoding encode = new System.Text.UnicodeEncoding();
        byte[] byteData = encode.GetBytes(text);
        if (!File.Exists(path))
        {
            FileStream oFileStream = new FileStream(path, FileMode.Create);
            oFileStream.Close();
        }

        using (StreamWriter stream = File.AppendText(path))
        {
            stream.WriteLine(text);
        }
    }
}