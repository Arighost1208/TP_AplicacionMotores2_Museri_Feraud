using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(WayPointManager))]
public class WayPointManagerInspector : Editor
{
    WayPointManager wpm;
    GUIStyle titleStyle;
    int _childrenCount;
    bool _error;
    string _msg;
    bool _editMode;
    private void OnEnable()
    {
        wpm = target as WayPointManager;
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.BoldAndItalic;
      //  wpm._pos = new Vector3(3, 3, 3);
        //Recorro los hijos del WayPointManager y mientras no sea el padre( i = 0 )
        // agrego su posicion a la lista.
        _childrenCount = wpm.GetComponentsInChildren(typeof(Transform)).Length - 1;
       
    }

    public override void OnInspectorGUI()
    {

        if (_error)
            ErrorMessage(_msg);

        EditorGUILayout.Space();

        _editMode = EditorGUILayout.Toggle("Edit Mode ", _editMode);

        EditorGUILayout.Space();

        if (_editMode)
            wpm._transformToEdit = (Transform)EditorGUILayout.ObjectField("Path to edit ", wpm._transformToEdit, typeof(Transform), true);

        else
            wpm._transformToEdit = null;
      
        EditorGUILayout.Space();

        if (!_editMode)
        { 
            wpm.path = EditorGUILayout.TextField("Name Path : ", wpm.path);

            EditorGUILayout.Space();

            UpdateVectorPos(wpm._pos);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create WayPoint"))
            {
  
                string wpName = "Waypoint " + (_childrenCount + 1) ;
                GameObject go = new GameObject(wpName);  
                go.AddComponent<BoxCollider>().isTrigger = true;
                go.transform.parent = wpm.transform;
                go.transform.localPosition = wpm._pos;
                 wpm._pos +=  Vector3.right*3;

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

                    //for (int i = _childCount - 1; i >= 1; i--)
                    //{
                    //    if (i > 0)
                    //        wpm.GetComponentsInChildren<Transform>()[i].parent = go.transform;
                    //}

                    for (int i = 1; i <= _childCount - 1; i++)
                    {
                        if (i > 0)
                            wpm.GetComponentsInChildren<Transform>()[1].parent = go.transform;
                    }

                    CreatePrefabPath(go);
                    wpm._pos = Vector3.zero;
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
            EditorGUILayout.Space();

            EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), Color.cyan);

            EditorGUILayout.Space();

            wpm._countDefaultWayPoint = EditorGUILayout.IntField("Count Waypoint : ", wpm._countDefaultWayPoint);
            
            EditorGUILayout.Space();
            wpm._distanceX = EditorGUILayout.IntField("X Axis distance: ", wpm._distanceX);

            EditorGUILayout.Space();
            wpm._distanceY = EditorGUILayout.IntField("Y Axis distance: ", wpm._distanceY);

            EditorGUILayout.Space();
            wpm._distanceZ = EditorGUILayout.IntField("Z Axis distance: ", wpm._distanceZ);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Waypoint Default"))
            {
                Vector3 _lastPos = Vector3.zero;
                for (int i= 1; i<= wpm._countDefaultWayPoint; i++)
                {
                    string wpName = "WaypointDefault " + (i);
                    GameObject go = new GameObject(wpName);
                    go.AddComponent<BoxCollider>().isTrigger = true;
                    go.transform.parent = wpm.transform;
                    if (i == 1)
                    {
                        go.transform.localPosition = wpm._pos;
                        _lastPos = go.transform.localPosition;
                    }

                    else
                    {
                        go.transform.localPosition = new Vector3(_lastPos.x + wpm._distanceX, _lastPos.y + wpm._distanceY, _lastPos.z + wpm._distanceZ);
                        _lastPos  = go.transform.localPosition;
                    }
                    
                }
                if (wpm._countDefaultWayPoint!= 0)
                    wpm._pos = _lastPos + new Vector3(wpm._distanceX, wpm._distanceY, wpm._distanceZ) ;

                CleanDefaultVariables();
            }
            EditorGUILayout.Space();
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), Color.cyan);
        }
        //Edition Mode
        else
        {
            if (wpm._transformToEdit != null  && wpm._transformToEdit.GetComponentsInChildren(typeof(Transform)).Length > 1)
            {
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), Color.cyan);

                EditorGUILayout.Space();

                EditorGUILayout.Space();

                EditorGUILayout.Space();

                int _childCount = wpm._transformToEdit.GetComponentsInChildren(typeof(Transform)).Length;
                for (int i = _childCount - 1; i >= 0; i--)
                {
                    if (i > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        float originalValue = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.fieldWidth = 2;
                        EditorGUIUtility.labelWidth = 1;

                        if (wpm._transformToEdit.GetComponentsInChildren<Transform>().Length > i)

                        {
                            EditorGUIUtility.labelWidth = 60;
                            EditorGUIUtility.fieldWidth = 200;
                            UpdateVectorPos(i, wpm._transformToEdit.GetChild(i - 1).localPosition);
                            wpm._transformToEdit.GetChild(i - 1).name = "Waypoint " + (i);
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
        }
    }

    private void OnSceneGUI()
    {

        if (!_editMode)
        {
          
            Handles.color = Color.yellow;

            wpm._pos = Handles.PositionHandle(wpm._pos, Quaternion.identity);

            Handles.SphereHandleCap(1, wpm._pos, Quaternion.identity, 1f, Event.current.type);
            UpdateVectorPos(wpm._pos);

            if (wpm.GetComponentsInChildren<Transform>() != null)
            {

                int _count = wpm.GetComponentsInChildren(typeof(Transform)).Length - 1;

                for (int i = _count - 1; i >= 0; i--)
                {
                    
                    wpm.transform.GetChild(i).position = Handles.PositionHandle(wpm.transform.GetChild(i).position, wpm.transform.GetChild(i).rotation);
                    UpdateVectorPos(i + 1, wpm.transform.GetChild(i).localPosition);
                    if (i == _count - 1)
                    {
                        Handles.color = Color.red;
                    }
                    else if (i == 0)
                    {
                        Handles.color = Color.green;
                    }
                    else
                    {
                        Handles.color = Color.blue;
                    }

                    if (Handles.Button(wpm.transform.GetChild(i).position, Quaternion.identity, 1f, 0.4f, Handles.SphereHandleCap))
                    {

                        Undo.DestroyObjectImmediate(wpm.transform.GetChild(i).gameObject);
                    }

                }

            }
            Handles.color = Color.magenta;
            
            WayDraw();
        }
       
        else
        {
            
            if (wpm._transformToEdit != null  && wpm._transformToEdit.GetComponentsInChildren<Transform>() != null)
            {
                
                int _count = wpm._transformToEdit.GetComponentsInChildren(typeof(Transform)).Length - 1;

                for (int i = _count - 1; i >= 0; i--)
                {
                    wpm._transformToEdit.GetChild(i).position = Handles.PositionHandle(wpm._transformToEdit.GetChild(i).position, wpm._transformToEdit.GetChild(i).rotation);
                    
                    UpdateVectorPos(i + 1, wpm._transformToEdit.GetChild(i).localPosition);
                    if (i == _count - 1)
                    {
                        Handles.color = Color.red;
                    }
                    else if (i == 0)
                    {
                        Handles.color = Color.green;
                    }
                    else
                    {
                        Handles.color = Color.blue;
                    }

                    if (Handles.Button(wpm._transformToEdit.GetChild(i).position, Quaternion.identity, 1f, 0.4f, Handles.SphereHandleCap))
                    {

                        Undo.DestroyObjectImmediate(wpm.transform.GetChild(i).gameObject);
                    }

                }

            }
            Handles.color = Color.magenta;
            WayDraw();
        }

    }
    private void ErrorMessage(string msg)
    {
        EditorGUILayout.HelpBox(msg, MessageType.Error);
    }

    private void UpdateVectorPos(int indice,Vector3 vec)
    {
        EditorGUILayout.LabelField(string.Format("Vector{0} : {1} ", indice, vec));
        
        Repaint();

    }

    private void UpdateVectorPos( Vector3 vec)
    {

        wpm._pos = EditorGUILayout.Vector3Field("VectorPos", vec);
        Repaint();
       
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

    private void WayDraw()
    {
        int _count;

        if (!_editMode)
            _count = wpm.GetComponentsInChildren(typeof(Transform)).Length;
        else
        {
            if (wpm._transformToEdit != null)
                _count = wpm._transformToEdit.GetComponentsInChildren(typeof(Transform)).Length;
            else
                _count = 0;
        }
            

        for (int i = 1; i < _count; i++)
        {
            if (i + 1 < _count)
            {
                if (!_editMode)
                    Handles.DrawLine(wpm.GetComponentsInChildren<Transform>()[i].position, wpm.GetComponentsInChildren<Transform>()[i + 1].position);
                else
                    Handles.DrawLine(wpm._transformToEdit.GetComponentsInChildren<Transform>()[i].position, wpm._transformToEdit.GetComponentsInChildren<Transform>()[i + 1].position);
            }

            if (i +1 == _count && !_editMode)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(wpm.GetComponentsInChildren<Transform>()[_count-1].position, wpm._pos);
            }

        }
    }

    private void CleanDefaultVariables()
    {
        wpm._countDefaultWayPoint = 0;
        wpm._distanceX = 0;
        wpm._distanceY = 0;
        wpm._distanceZ = 0;
    }
}