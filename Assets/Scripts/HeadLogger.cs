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
    private GameObject focus;
    private GameObject focusLeftTop;
    private GameObject focusRightTop;
    private GameObject focusLeftBottom;
    private GameObject focusRightBottom;

    public FocusMode showTracker = FocusMode.Center;

    public float fovAngleYaw = 0.5f;
    public float fovAnglePitch = 0.2f;

    public double timesPerSec = 5;
    public float trackerSize = 0.2f;
    public Material trackerMaterial;

    public int randomSeedData = 0;
    public bool isEasy = false;
    private Boolean active = false;
    private DateTime last = System.DateTime.Now;
    private string fileName = "";

    // Start is called before the first frame update
    void Start()
    {
        focus = generateSphere();
        focusLeftTop = generateSphere();
        focusRightTop = generateSphere();
        focusLeftBottom = generateSphere();
        focusRightBottom = generateSphere();
    }

    GameObject generateSphere()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        float scale = trackerSize;
        obj.transform.localScale = new Vector3(scale, scale, scale);
        obj.SetActive(false);
        obj.GetComponent<Collider>().enabled = false;
        obj.GetComponent<Renderer>().material = trackerMaterial;
        return obj;
    }

    // Update is called once per frame
    void Update()
    {
        Transform headTransform = gameObject.transform;

        RaycastHit hit;
        Vector3 direction = headTransform.TransformDirection(Vector3.forward);
        Vector3 right = headTransform.TransformDirection(Vector3.right);
        Vector3 up = headTransform.TransformDirection(Vector3.up);

        if (Physics.Raycast(headTransform.position, direction, out hit, 200))
        {
            focus.transform.position = hit.point;
            focus.SetActive(showTracker == FocusMode.Center || showTracker == FocusMode.CenterAndCorners);
        }
        else focus.SetActive(false);

        bool showCorners = showTracker == FocusMode.Corners || showTracker == FocusMode.CenterAndCorners;
        if (Physics.Raycast(headTransform.position, rotate(rotate(direction, right, -fovAnglePitch), up, -fovAngleYaw), out hit, 200))
        {
            focusLeftTop.transform.position = hit.point;
            focusLeftTop.SetActive(showCorners);
        }
        else focusLeftTop.SetActive(false);

        if (Physics.Raycast(headTransform.position, rotate(rotate(direction, right, -fovAnglePitch), up, fovAngleYaw), out hit, 200))
        {
            focusRightTop.transform.position = hit.point;
            focusRightTop.SetActive(showCorners);
        }
        else focusRightTop.SetActive(false);

        if (Physics.Raycast(headTransform.position, rotate(rotate(direction, right, fovAnglePitch), up, -fovAngleYaw), out hit, 200))
        {
            focusLeftBottom.transform.position = hit.point;
            focusLeftBottom.SetActive(showCorners);
        }
        else focusLeftBottom.SetActive(false);

        if (Physics.Raycast(headTransform.position, rotate(rotate(direction, right, fovAnglePitch), up, fovAngleYaw), out hit, 200))
        {
            focusRightBottom.transform.position = hit.point;
            focusRightBottom.SetActive(showCorners);
        }
        else focusRightBottom.SetActive(false);

        if (active)
        {
            // Add to log
            DateTime now = System.DateTime.Now;
            if (now.Subtract(last).Milliseconds > 1000 / timesPerSec)
            {
                AddToLog(fileName, 
                    focus.transform.position.ToString() +
                    focusLeftTop.transform.position.ToString() +
                    focusRightTop.transform.position.ToString() +
                    focusLeftBottom.transform.position.ToString() +
                    focusRightBottom.transform.position.ToString() +
                    "@" + now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
                last = now;
            }
        }
    }

    Vector3 rotate(Vector3 direction, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * direction;
    }

    public void setActive(Boolean active)
    {
        this.active = active;
        if (active) {
            fileName = System.DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")+" "+randomSeedData+";"+isEasy;
        }
    }

    void AddToLog(string fileName, string text)
    {
        string basePath = Application.platform == RuntimePlatform.WindowsEditor ? Application.dataPath +"/../data" : Application.persistentDataPath;
        string path = basePath + "/" + fileName + ".txt";
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