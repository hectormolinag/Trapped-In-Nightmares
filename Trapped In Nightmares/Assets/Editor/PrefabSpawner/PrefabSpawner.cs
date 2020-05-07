using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class PrefabSpawner : EditorWindow
{
    public float radius = 2f;
    public int spawnCount = 8;
    public GameObject spawnPrefab = null;
    public LayerMask layerMask;

    private struct RandomData
    {
        public Vector2 pointInDisc;
        public float randAngleDeg;

        public void SetRandomValues()
        {
            pointInDisc = Random.insideUnitCircle;
            randAngleDeg = Random.value * 360;
        }
    }
    
    private RandomData[] randPoints;
 
    private SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propSpawnCount;
    private SerializedProperty propSpawnPrefabs;
    private SerializedProperty propLayerMask;
    
    private readonly List<Pose> hitPoses = new List<Pose>();

    private GameObject[] prefabs;

    [MenuItem("Custom Tools/Prefab Spawner")]
    public static void OpenWindow() => GetWindow<PrefabSpawner>();

    void GenerateRandomPoints()
    {
        randPoints = new RandomData[spawnCount];
        for (int i = 0; i < spawnCount; i++)
            randPoints[i].SetRandomValues();
    }

    void DrawSphere(Vector3 pos) => Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.5f, EventType.Repaint);

    void TrySpawnObjects(List<Pose> poses)
    {
        if (spawnPrefab == null)
            return;
        
        foreach (Pose pose in poses)
        {
            // Instantiate Prefab
            GameObject spawnedObj = (GameObject)PrefabUtility.InstantiatePrefab(spawnPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedObj, "Spawned Objects");
            spawnedObj.transform.position = pose.position;
            spawnedObj.transform.rotation = pose.rotation;
        }
    }
    
    private void OnEnable()
    {
        so = new SerializedObject(this);
        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
        propSpawnPrefabs = so.FindProperty("spawnPrefab");
        propLayerMask = so.FindProperty("layerMask");
        
        GenerateRandomPoints();
        
        SceneView.duringSceneGui += DuringSceneGUI;

        string[] guids = AssetDatabase.FindAssets("t:prefab", new[] {"Assets/Prefabs"});
        IEnumerable<string> paths = guids.Select((AssetDatabase.GUIDToAssetPath));
        prefabs = paths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI()
    {
        so.Update();
        
        EditorGUILayout.PropertyField(propRadius);
        propRadius.floatValue = Mathf.Max(1f, propRadius.floatValue);
        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = Mathf.Max(0, propSpawnCount.intValue);
        EditorGUILayout.PropertyField(propSpawnPrefabs);
        EditorGUILayout.PropertyField(propLayerMask);

        if (so.ApplyModifiedProperties())
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }
        

        // If you clicked inside the EditorWindow, deselect everything
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GUI.FocusControl(null);
            Repaint();
        }
    }

    void DuringSceneGUI (SceneView sv)
    {
        
        Handles.BeginGUI();
        
        GUI.Label(new Rect(4f, 4f, 120, 12f), "Choose a Prefab");
        Rect rect = new Rect(8f, 16f, 72f, 72f);

        foreach (GameObject prefab in prefabs)
        {
            Texture icon = AssetPreview.GetAssetPreview(prefab);
            //if (GUI.Button(rect, icon))
            if(GUI.Toggle(rect, spawnPrefab == prefab, new GUIContent(icon)))
            {
                spawnPrefab = prefab;
                //propSpawnPrefabs = so.FindProperty("spawnPrefab");
                //so.ApplyModifiedProperties();
                Repaint();
            }

            rect.y += rect.height + 2;
        }
            
            
        Handles.EndGUI();
        
        
        Handles.zTest = CompareFunction.LessEqual;
        bool holdingShift = (Event.current.modifiers & EventModifiers.Shift) != 0;
        
        // Repaints SceneView when mouse moves
        if(Event.current.type == EventType.MouseMove)
            sv.Repaint();
        
        // Change radius on ScrollWheel
        
        if (Event.current.type == EventType.ScrollWheel && holdingShift)
        {
            float scrollDir = Mathf.Sign(Event.current.delta.y);
            
            so.Update();
            propRadius.floatValue *= 1f + scrollDir * 0.05f;
            GenerateRandomPoints();
            so.ApplyModifiedProperties();
            Repaint();     // Repaints editor window
            
            Event.current.Use();   // consume the event, don´t let it fall through
        }

        
        // Draw Handles only on Repaint
        if (Event.current.type == EventType.Repaint)
        {
            Transform camTf = sv.camera.transform;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            
            //Ray ray = new Ray(camTf.position, camTf.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                // Setting up the tangent space based on mesh
                Vector3 hitNormal = hit.normal;
                Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
                Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

                Ray GetTangentRay(Vector2 tangentSpacePos)
                {
                    // Converting points on worldPos in Tangent Space and scale them with the radius of the circle
                    Vector3 rayOrigin = hit.point + (hitTangent * tangentSpacePos.x + hitBitangent * tangentSpacePos.y) * radius;
                    rayOrigin += hitNormal * 2;
                    Vector3 rayDir = -hit.normal; 
                    // Raycast to find point on surface
                    return new Ray(rayOrigin, rayDir);
                }
                
                hitPoses.Clear();
                // Drawing points and storage them in a List
                foreach (RandomData randDataPoint in randPoints)
                {
                    Ray pointRay = GetTangentRay(randDataPoint.pointInDisc);
                    if (Physics.Raycast(pointRay, out RaycastHit pointHit, 30f))
                    {
                        // Random Rotation por Instanciated Prefabs
                        Quaternion randRot = Quaternion.Euler(0f, 0f, randDataPoint.randAngleDeg);
                        Quaternion rot = Quaternion.LookRotation(pointHit.normal) * (randRot * Quaternion.Euler(90f, 0f, 0f));
                        
                        Pose pose = new Pose(pointHit.point, rot);
                        hitPoses.Add(pose);
                        
                        DrawSphere(pointHit.point);
                        Handles.DrawAAPolyLine(3, pointHit.point, pointHit.point + (pointHit.normal * 2));

                        if (spawnPrefab != null)
                        {
                            // Draw Mesh preview
                            Matrix4x4 parentMtx = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
                            MeshFilter[] meshes = spawnPrefab.GetComponentsInChildren<MeshFilter>();
                            // Check for every child in prefab and making sure it has that components
                            foreach (MeshFilter filter in meshes)
                            {
                                if (filter != null)
                                {
                                    Matrix4x4 childMtx = filter.transform.localToWorldMatrix;
                                    Matrix4x4 finalMtx = parentMtx * childMtx;
                                    Mesh mesh = filter.sharedMesh;
                                    if (filter.GetComponent<MeshRenderer>() != null)
                                    {
                                        Renderer rend = filter.GetComponent<MeshRenderer>();
                                        Material mat = rend.sharedMaterial;
                                        mat.SetPass(0);
                                        Graphics.DrawMeshNow(mesh, finalMtx);     // Drawing the mesh with it's corresponding material
                                    }
        
                                }
                            }

                        }
                        
                    }
                }
                
                // Draw Axis of Tangent Space
                Handles.color = Color.red;
                Handles.DrawAAPolyLine(8, hit.point, hit.point + (hitTangent * 2));
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(8, hit.point, hit.point + (hitBitangent * 2));
                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(8, hit.point, hit.point + (hitNormal * 2));

                // Draw Circle adapted to the terrain
                const int circleDetail = 128;
                const float TAU = 6.28318530718f;
                
                Vector3[] ringPoints = new Vector3[circleDetail];
                for (int i = 0; i < circleDetail; i++)
                {
                    float t = i / ((float)circleDetail - 1);   // go back to 0/1 pos
                    float angRad = t * TAU;
                    Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                    Ray r = GetTangentRay(dir);
                    if (Physics.Raycast(r, out RaycastHit cHit))
                    {
                        ringPoints[i] = cHit.point + cHit.normal * 0.02f;
                    }
                    else
                    {
                        ringPoints[i] = r.origin; 
                    }
                }
                Handles.color = Color.black;
                Handles.DrawAAPolyLine(3, ringPoints);
                
                //Handles.DrawWireDisc(hit.point, hit.normal, radius);
                
                Handles.color = Color.white;     // Reset Handles Color
                
            }
            else
                hitPoses.Clear();
            
            
            
        }
        
        // Try Spawn Game Objects
        if (holdingShift && Event.current.type == EventType.MouseDown && Event.current.button == 1) 
        {
            TrySpawnObjects(hitPoses);
            Event.current.Use();

        }
        
        if (holdingShift && Event.current.type == EventType.MouseDrag && Event.current.button == 1)
        {
            TrySpawnObjects(hitPoses);
            Event.current.Use();
        }
        
    }
    
}
