using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor( typeof( Enemigo1 ) )]
public class PositionHandleEditor : Editor
{
	void OnSceneGUI( )
	{
		//PositionHandle t = target as PositionHandle;
        Enemigo1 enem = target as Enemigo1;
        //t.gameObject.GetComponent<Enemigo1>().punto1

       
        Handles.color = Color.magenta;

		EditorGUI.BeginChangeCheck( );
  
        List<Vector3> posiciones = enem.posicionesPatrullaje;//global
       // List<Vector3> posiciones2 = new List<Vector3>(enem.posicionesPatrullaje);//local
       
        for (int i = 0; i < enem.posicionesPatrullaje.Count; i++)
        {
            Vector3 p = Handles.PositionHandle(posiciones[i], Quaternion.identity);
            //posiciones2[i] = p - enem.transform.position;//local
           
            posiciones[i] = p; //glonbal

            Handles.Label(posiciones[i], "Posición "+ i);
        }
        /*
		Vector3 lookTarget = Handles.PositionHandle(enem.punto1, Quaternion.identity);
        Vector3 lookTarget2 = Handles.PositionHandle(enem.punto2, Quaternion.identity);

        Handles.Label(lookTarget, "Posición 1");
        */
        
        if ( EditorGUI.EndChangeCheck( ) )
		{
           
            //Undo.RecordObject( target, "Changed Look Target" );
            for (int i = 0; i < enem.posicionesPatrullaje.Count; i++)
            {
                enem.posicionesPatrullaje[i] = posiciones[i];
                

       }
            /*
            enem.punto1 = lookTarget;
            enem.punto2 = lookTarget2;*/
			//t.Update( );
		}
	}

}