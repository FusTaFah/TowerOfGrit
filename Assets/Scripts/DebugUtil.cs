using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

class Pair<T, M>
{
    public T left;
    public M right;
    public Pair(T t, M m)
    {
        left = t;
        right = m;
    }
}

public class DebugUtil : MonoBehaviour
{
    //text of the debugging system
    Text debugText;
    //list of debug items
    List<string> debugList;
    //custom gameobject that represents a position marked by some stimulus
    public GameObject placeHolder;
    List<Pair<GameObject, float>> existingPlaceholders;
    //length of MarkLocation2D lines
    public float markLocation2DLineLength;

    // Use this for initialization
    void Start()
    {
        debugText = gameObject.GetComponent<Text>();
        debugList = new List<string>();
        existingPlaceholders = new List<Pair<GameObject, float>>();
    }

    //add a string to the debug list
    public void AppendDebugger(string debugItem)
    {
        debugList.Add(debugItem);
    }

    //add a vector to the debug list
    public void AppendDebugger(Vector3 debugItem)
    {
        debugList.Add(debugItem.x + " " + debugItem.y + " " + debugItem.z);
    }

    //instantiate a persistent placeholder that lasts until termination
    public void MarkLocation(Vector3 position)
    {
        Instantiate(placeHolder, position, Quaternion.identity);
    }

    public void MarkDebugLocation2D(Vector2 position, Color color)
    {
        //diagonal right
        Vector2 offsetDiagonalRight = new Vector2(-1.0f, -1.0f).normalized * markLocation2DLineLength;
        Debug.DrawLine(position + offsetDiagonalRight, position - offsetDiagonalRight, color);
        //diagonal left
        Vector2 offsetDiagonalLeft = new Vector2(-1.0f, 1.0f).normalized * markLocation2DLineLength;
        Debug.DrawLine(position + offsetDiagonalLeft, position - offsetDiagonalLeft, color);
        
    }

    //instantiate a placeholder that terminates after a timeSpan
    public void MarkLocation(Vector3 position, float timeSpan)
    {
        if(existingPlaceholders.Count > 0)
        {
            foreach(Pair<GameObject, float> p in existingPlaceholders)
            {
                if (!p.left.activeInHierarchy)
                {

                }
            }
        }
        else
        {
            GameObject obj = Instantiate(placeHolder, position, Quaternion.identity) as GameObject;
            existingPlaceholders.Add(new Pair<GameObject, float>(obj, timeSpan));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //display all debug items
        debugText.text = "";
        foreach (string x in debugList)
        {
            debugText.text += x + "\n";
        }
        debugList.Clear();

        if(existingPlaceholders.Count > 0)
        {
            for (int i = existingPlaceholders.Count - 1; i >= 0; i--)
            {
                Pair<GameObject, float> p = existingPlaceholders[i];
                if (p.right <= 0.0f)
                {
                    Destroy(p.left);
                    existingPlaceholders.RemoveAt(i);
                }
                else
                {
                    p.right -= Time.deltaTime;
                }
            }
        }
    }
}