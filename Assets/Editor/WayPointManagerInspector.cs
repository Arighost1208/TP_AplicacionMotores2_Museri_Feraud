using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


[CustomEditor(typeof(WayPointManager))]
public class WayPointManagerInspector : Editor
{
    WayPointManager wpm;
    GUIStyle titleStyle;
    int _childrenCount;
    bool _error;
    string _msg;

    private void OnEnable()
    {
        wpm = target as WayPointManager;
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.BoldAndItalic;
  
        //Recorro los hijos del WayPointManager y mientras no sea el padre( i = 0 )
        // agrego su posicion a la lista.
        _childrenCount = wpm.GetComponentsInChildren(typeof(Transform)).Length - 1;
       
    }

    public override void OnInspectorGUI()
    {
        if (_error)
            ErrorMessage(_msg);

        EditorGUILayout.Space();

        wpm.path = EditorGUILayout.TextField("Name Path : ", wpm.path);

        EditorGUILayout.Space();

        wpm._pos = UpdateVectorPos(wpm._pos);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create WayPoint"))
        {
  
            string wpName = "Waypoint " + (_childrenCount + 1) ;
            GameObject go = new GameObject(wpName);  
            go.AddComponent<BoxCollider>().isTrigger = true;
            go.transform.parent = wpm.transform;
            go.transform.localPosition = wpm._pos;
             wpm._pos = Vector3.zero;
         
            wpm.transform.position = Vector3.zero;
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
                       UpdateVectorPos( i,wpm.transform.GetChild(i - 1).localPosition);
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

        if (GUILayout.Button("Create Path"))
        {
            int _childCount = wpm.GetComponentsInChildren(typeof(Transform)).Length;

            if (_childCount > 2 &&  !string.IsNullOrEmpty( wpm.path) )
            {
                _error = false;
                GameObject go = new GameObject(wpm.path);

                for (int i = _childCount - 1; i >= 1; i--)
                {
                    if (i > 0)
                        wpm.GetComponentsInChildren<Transform>()[i].parent = go.transform;
                }

                CreatePrefabPath(go);
            }
            
            else
            {
                _error = true;
                if (string.IsNullOrEmpty(wpm.path))
                    _msg = "Name Path can't be empty.";
                else
                _msg = "You must add two vector to create a Path";
            }

           
        }

        EditorGUILayout.Space();
    }

    private void OnSceneGUI()
    {
        /*bloqueo la posicion del gameObject padre(si tiene mas de un hijo)  para que no se mueva el gizmo de posicion y 
         * afecte a los hijos */
        if (wpm.GetComponentsInChildren(typeof(Transform)).Length > 2)
            wpm.GetComponentsInChildren<Transform>()[0].localPosition = Vector3.zero;

        Handles.color = Color.red;
        if (wpm.GetComponentsInChildren(typeof(Transform)).Length == 1 )  
            wpm._pos = Handles.PositionHandle(wpm.transform.position, wpm.transform.rotation);

       else
            wpm._pos = Handles.PositionHandle(wpm._pos, wpm.transform.rotation);

        Handles.SphereHandleCap(1, wpm._pos, Quaternion.identity, 1f, Event.current.type);
        wpm._pos = UpdateVectorPos(wpm._pos);

        if (wpm.GetComponentsInChildren<Transform>() != null)
        {

            int _count = wpm.GetComponentsInChildren(typeof(Transform)).Length-1;

            for (int i = _count-1; i >=0; i--)
            {
                wpm.transform.GetChild(i).position = Handles.PositionHandle(wpm.transform.GetChild(i).position, wpm.transform.GetChild(i).rotation);
                UpdateVectorPos( i+1,wpm.transform.GetChild(i).localPosition);
                
                if (Handles.Button(wpm.transform.GetChild(i).localPosition, Quaternion.identity, 1f, 0.4f, Handles.SphereHandleCap))
                {
                   
                    DestroyImmediate(wpm.transform.GetChild(i).gameObject);
                }

            }

    }


    }
    private void ErrorMessage(string msg)
    {
        EditorGUILayout.HelpBox(msg, MessageType.Error);
    }

    private void UpdateVectorPos(int indice,Vector3 vec)
    {
        EditorGUILayout.Vector3Field(string.Format("Vector{0} ", indice), vec);
        Repaint();
    }

    private Vector3 UpdateVectorPos( Vector3 vec)
    {
        
        EditorGUILayout.Vector3Field("VectorPos", vec);
        Repaint();
        return vec;
    }

    private void CreatePrefabPath(GameObject _prefab )
    {
        if (AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/" + _prefab.name + ".prefab");

            PrefabUtility.SaveAsPrefabAsset(_prefab, path);
        }
        else
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");

            string path = "Assets/Prefabs/" + _prefab.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(_prefab, path);
        }
    }
}