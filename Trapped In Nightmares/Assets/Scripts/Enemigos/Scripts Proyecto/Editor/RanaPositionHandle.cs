using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rana))]
public class RanaPositionHandle : Editor
{
        void OnSceneGUI()
        {
        Rana r = target as Rana;


            EditorGUI.BeginChangeCheck();
            //Dibujar la posicion Inicial y poder moverla a gusto
            r.posicionInicial = Handles.PositionHandle(r.posicionInicial, Quaternion.identity);
            Handles.Label(r.posicionInicial, "Posición Inicial");


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Look Target");
                
             
                /*
                enem.punto1 = lookTarget;
                enem.punto2 = lookTarget2;*/

            }
        }
}
