using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WayPointSystemWindow : EditorWindow
{
    [MenuItem("Window/WayPointSystem/WayPoint Manager")]
    public static void OpenWindow()
    {
        GameObject go = new GameObject("WayPoint Manager", Type.EmptyTypes);
        WayPointManager wp = go.AddComponent<WayPointManager>();

    }
    private void OnEnable()
    {

    }
}