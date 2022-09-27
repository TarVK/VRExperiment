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
    string fileName;

    // Start is called before the first frame update
    void Start()
    {
        fileName = System.DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
    }

    // Update is called once per frame
    void Update()
    {
        Transform headTransform = gameObject.transform;

        RaycastHit hit;
        if (Physics.Raycast(headTransform.position, headTransform.TransformDirection(Vector3.forward), out hit, 20))
        {
            if (focus != null)
                focus.transform.position = hit.point;

            AddToLog(fileName, hit.point.ToString() + "@" + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
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