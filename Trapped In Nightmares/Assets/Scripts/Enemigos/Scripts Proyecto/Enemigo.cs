using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemigo : MonoBehaviour
{


    public float vida;
    //public SteeringCombined steering;
    public FSM fsm;
    public TargetVisible buscadorPlayer;
    public bool estaDesvanecido = false;
    public bool estaAturdido = false ;
    public Rigidbody rigid;

    public Renderer render;


    public Transform target;

   // public float velocidad;
    public float tiempoAturdido;
    //public float distanciaDeAtaque;



}


enum EstadosEnemigo
{
    Desvanecido,
    Aturdido
}



//Lo unico que hace es activar la variable de estaDesvanecido 

public class EnemigoDesvanecidoEstado : State
{

    private Enemigo enem;
    private Vector3 rotationTarget = Vector3.zero;
    UnityEngine.AI.NavMeshAgent agent;
    Vector3 velocidad;
    bool vaARenderear = false;

    public EnemigoDesvanecidoEstado(Enemigo enemigo)
    {
        enem = enemigo;
        agent = enem.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    //Acción que realizará este estado  
    public override void Act(GameObject _object)
    {
        ///nada
        ///supongo... :v
        ///


        Renderer[] r = enem.GetComponentsInChildren<Renderer>();
        if ((Time.deltaTime*5f) % .5 < .2)
            vaARenderear = false;
        else
            vaARenderear = true;


        foreach (var item in r)
        {
            item.enabled = vaARenderear;
        }

    }

    //Condición para terminar este estado
    public override void Reason(GameObject _object)
    {
      //como el desvanecimiento significa que muere este enemigo entonces supongo que 
      //no habrá cambios a otro estado
    }

    //Para cambiar Animación...
    public override void OnEnter(GameObject _object)
    {
        //
        enem.estaDesvanecido = true;
        //Debug.Log("DEsvanecido");
        agent.isStopped = true;
        //velocidad = agent.velocity;
       // agent.velocity = Vector3.zero;
    }

    IEnumerator EsperarParaDesactivar()
    {

        yield return new WaitForSeconds(3f);
        Renderer[] r = enem.GetComponentsInChildren<Renderer>();
        foreach (var item in r)
        {
            item.enabled = false; ;
        }
        enem.fsm.DeActivate();
        //agent.enabled = true;
    }


    public override void OnExit(GameObject _object)
    {
        enem.estaDesvanecido = false;
        agent.isStopped = false;
        //agent.velocity = velocidad;
    }

}


public class EnemigoAturdidoEstado : State
{

    private Enemigo enem;
    private Vector3 rotationTarget = Vector3.zero;
    UnityEngine.AI.NavMeshAgent agent;
    bool vaARenderear = false;
    bool regresar = false;

    Vector3 velocidad;
    Coroutine esperar;


    public EnemigoAturdidoEstado(Enemigo enemigo)
    {
        enem = enemigo;
        agent = enem.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    //Acción que realizará este estado  
    public override void Act(GameObject _object)
    {

        //Debug.Log(Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)));


        Renderer[] r = enem.GetComponentsInChildren<Renderer>();
        if (Time.fixedTime % .5 < .2)
            vaARenderear = false;
        else
            vaARenderear = true;


        foreach (var item in r)
        {
            item.enabled = vaARenderear;
        }
        //enem.transform.LookAt(rotationTarget);

    }


    //Condición para terminar este estado
    public override void Reason(GameObject _object)
    {
        if(regresar)
            fsm.RevertBlipState();
    }


    IEnumerator EsperarParaRegresar()
    {

        yield return new WaitForSeconds(enem.tiempoAturdido);
        //Debug.Log("DesAturdido");
        regresar = true;
        //agent.enabled = true;
    }




    //Para cambiar Animación...
    public override void OnEnter(GameObject _object)
    {

        //Debug.Log("Aturdido");

        agent.isStopped = true;
        //velocidad = agent.velocity;
        //agent.velocity = Vector3.zero;
        enem.estaAturdido = true;
        regresar = false;
        esperar = fsm.myMono.StartCoroutine(EsperarParaRegresar());
        
    }


    public override void OnExit(GameObject _object)

    {
        agent.isStopped = false;
        //agent.velocity = velocidad;
        enem.estaAturdido = false;
        Renderer[] r = enem.GetComponentsInChildren<Renderer>();

        foreach (var item in r)
        {
            item.enabled = true;
        }

    }

}

