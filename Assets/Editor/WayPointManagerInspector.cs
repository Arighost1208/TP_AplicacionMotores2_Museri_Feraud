using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(WayPointManager))]
public class WayPointManagerInspector : Editor
{
    WayPointManager wpm;
    List<Vector3> _waypoints;
    GUIStyle titleStyle;
    int _childrenCount;
    private void OnEnable()
    {
        wpm = target as WayPointManager;
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.BoldAndItalic;

        _waypoints = new List<Vector3>();

        //Recorro los hijos del WayPointManager y mientras no sea el padre( i = 0 )
        // agrego su posicion a la lista.
        _childrenCount = wpm.GetComponentsInChildren(typeof(Transform)).Length - 1;
        //if(wpm.GetComponentsInChildren(typeof(Transform)).Length > 0)
        //{
        //    for (int i = 0; i < wpm.GetComponentsInChildren(typeof(Transform)).Length; i++)
        //    {
        //        if (i > 0)
        //            _waypoints.Add(wpm.GetComponentsInChildren<Transform>()[i].position);
        //    }
        //}

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        wpm.path = EditorGUILayout.TextField("Name Path : ", wpm.path);

        EditorGUILayout.Space();

        wpm._pos = EditorGUILayout.Vector3Field("VectorPos", wpm._pos);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create WayPoint"))
        {

            //string wpName = "Waypoint " + (_waypoints.Count()+1);
            string wpName = "Waypoint " + (_childrenCount + 1) ;
            GameObject go = new GameObject(wpName);
            go.transform.localPosition = wpm._pos;
            go.AddComponent<BoxCollider>();
            go.transform.parent = wpm.transform;
          //  _waypoints.Add(wpm._pos);
            wpm._pos = Vector3.zero;
        }

        EditorGUILayout.Space();

        //Siempre y cuando tenga el WayPoint Manager algun hijo, se dibujara el boton de eliminar
        // y se mostrara las posiciones de los wayPoints creados
        if(wpm.GetComponentsInChildren(typeof(Transform)).Length > 1)
        {
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), Color.cyan);

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            //Comenzara a mostrarse los vectores creados desde el ultimo al primero,excluyendo al padre(WayPoint Manager i=0)
            int _childCount = wpm.GetComponentsInChildren(typeof(Transform)).Length;
            for (int i = _childCount - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    float originalValue = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.fieldWidth = 2;
                    EditorGUIUtility.labelWidth = 1;
                    
                    //se agrega esta validacion ya que el GetComponentsInChildren tiene en cuenta al padre
                    //y el GetChild solo a los hijos 
                    if (wpm.GetComponentsInChildren<Transform>().Length > i)
                   
                    {
                        
                        EditorGUIUtility.labelWidth = 60;
                        EditorGUIUtility.fieldWidth = 200;
                       
                        //Renombro a los vectores en el Inspector por si se eliminaron vectores y tambien los renombro en
                        //el Hierarchy por el orden correspondiente
                        EditorGUILayout.Vector3Field(string.Format("Vector{0} ", i), wpm.transform.GetChild(i - 1).localPosition);
                        wpm.transform.GetChild(i-1).name = "Waypoint " + (i);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                   
                }
                    
            }

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), Color.cyan);
        }
       
        EditorGUILayout.Space();
  
        EditorGUILayout.Space();
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        Handles.SphereHandleCap(1, wpm._pos, Quaternion.identity, 1f, Event.current.type);

        int _count = wpm.GetComponentsInChildren(typeof(Transform)).Length-1;
        for (int i = 1; i <= _count; i++)
        {          
            Handles.color = Color.red;
           
            Handles.SphereHandleCap(i, wpm.GetComponentsInChildren<Transform>()[i].localPosition, Quaternion.identity, 1f, Event.current.type);
        }

    }
}