using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo1 : Enemigo
{
    public List<Vector3> posicionesPatrullaje;
   
    /*
    public Vector3 punto1;
    public Vector3 punto2;*/
    public NavMeshAgent navMeshAgent;

    

    public void SetSteering()
    {
        //steering = GetComponent<SteeringCombined>();
    }


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;


        navMeshAgent = GetComponent<NavMeshAgent>();
        fsm = new FSM(gameObject, this);
        Enemigo1PerseguirEstado estadoPerseguir = new Enemigo1PerseguirEstado(this);
        Enemigo1IdleEstado estadoIdle = new Enemigo1IdleEstado(this);
        EnemigoDesvanecidoEstado estadoDesvanecido = new EnemigoDesvanecidoEstado(this);
        //este siempre será blip state
        EnemigoAturdidoEstado estadoAturdido = new EnemigoAturdidoEstado(this);

        fsm.AddState(EstadosEnemigo1.Perseguir, estadoPerseguir);
        fsm.AddState(EstadosEnemigo1.Idle, estadoIdle);
        fsm.AddState(EstadosEnemigo.Desvanecido, estadoDesvanecido);
        fsm.AddState(EstadosEnemigo.Aturdido, estadoAturdido);

        Inicializar();



    }

    public void Inicializar()
    {
        fsm.ChangeState(EstadosEnemigo1.Idle);
        fsm.Activate();
    }


    // Update is called once per frame
    void Update()
    {

        if (fsm != null && fsm.IsActive())
        {
            fsm.UpdateFSM();
           
        }


    }


    private void OnTriggerEnter(Collider other)
    {
        //Aquí va a cambiar a los estados de desvanecido
        if (other.gameObject.tag.Equals("Attack"))
        {
            fsm.ChangeState(EstadosEnemigo.Desvanecido);
        }

        //Supongo que si recibe un ataque con "más fuerza" se desvanece
        //otrhe
    }


    public void LootAt(Vector3 p)
    {
        transform.LookAt(Vector3.Lerp(transform.position, p, 0.3f));
        
    }



#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);

        for (int i = 0; i < posicionesPatrullaje.Count; i++)
        {

            Gizmos.DrawSphere(posicionesPatrullaje[i], 1f);
        }

    }

    
    private void OnDrawGizmos()
    {
        
        /*
        Gizmos.DrawSphere(punto1, 1f);
        Gizmos.DrawSphere(punto2, 1f);*/

        buscadorPlayer.EditorGizmo(transform);



        //Debug.Log(buscadorPlayer.Detect(transform, target, true));
    }

#endif


}


enum EstadosEnemigo1
{
    Perseguir,
    Idle,

}



public class Enemigo1PerseguirEstado : State
{
    //Estado que persigue a un target;
    private Enemigo1 enem;

    NavMeshPath path;

    Coroutine esperarTargetNoVisto;
    //bool targetVisto;

    public Enemigo1PerseguirEstado(Enemigo1 enemigo)
    {
        enem = enemigo;
    }

    public override void Act(GameObject _object) 
    {

        ///
        ///Aquí entra al estado de ataque;
        ///
        enem.navMeshAgent.SetDestination(enem.target.position);


    }

    public override void Reason(GameObject _object)
    {
        if (enem.buscadorPlayer.Detect(enem.transform, enem.target, true))
        {
            //targetVisto = true;
            fsm.myMono.StopCoroutine(Algo());
        }
        else
        {
           // targetVisto = false;
            esperarTargetNoVisto = fsm.myMono.StartCoroutine(Algo());
        }
    
    }

    public override void OnEnter(GameObject _object) 
    {
        /*
        path = new NavMeshPath();
        Vector3 position = enem.transform.position - enem.target.position;

        NavMesh.CalculatePath(enem.transform.position, enem.target.position, 0, path);
        */

       // targetVisto = true;
        Vector3 position = enem.transform.position + new Vector3(0f, 0f, 0f);

        enem.navMeshAgent.Warp(position);
        //Debug.Log("Path seteado 1");
        enem.navMeshAgent.SetDestination(enem.target.position);
        //Debug.Log("Path seteado");
        /*
         
        fsm.myMono.GetComponent<NavMeshAgent>().CalculatePath(fsm.myMono.GetComponent<Enemigo1>().target.position, path);
        fsm.myMono.GetComponent<NavMeshAgent>().path = path;

        */

        //ebug.Log("Persiguiendo");

    }

    IEnumerator Algo()
    {


        yield return new WaitForSeconds(3);
        ChangeState(EstadosEnemigo1.Idle);
    }

    public override void OnExit(GameObject _object)
    
    {

    }

}


/// <summary>
/// Estado inicial de Patrullaje
/// 
/// 
/// 
/// </summary>
/// 


public class Enemigo1IdleEstado : State
{

    private Enemigo1 enem;
    private Vector3 rotationTarget = Vector3.zero;
    private int currentPosition = 0;
    private NavMeshAgent agent;

    public Enemigo1IdleEstado(Enemigo1 enemigo)
    {
        enem = enemigo;
        agent = enem.GetComponent<NavMeshAgent>();
    }

    //Acción que realizará este estado  
    public override void Act(GameObject _object)
    {
       

    

        if (!agent.pathPending && agent.remainingDistance < 2f)
            GotoNextPoint();
        /*
        enem.transform.rotation = Quaternion.RotateTowards(enem.transform.rotation, Quaternion.Euler(rotationTarget), 20f * Time.deltaTime);
        //Debug.Log(Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)));
        if(Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)) < 2f)
        {
            RotationRandom();
        }
       */
        //enem.transform.LookAt(rotationTarget);


    }


    //Condición para terminar este estado
    public override void Reason(GameObject _object)
    {
        if (enem.buscadorPlayer.Detect(enem.transform, enem.target, true))
        {
            fsm.ChangeState(EstadosEnemigo1.Perseguir);
        }
    }


    //Para cambiar Animación...
    public override void OnEnter(GameObject _object)
    {
        //Debug.Log("Patrullaje");
        agent.autoBraking = false;

        GotoNextPoint();

    }


    public override void OnExit(GameObject _object)

    {

        agent.autoBraking = true;
    }


    void GotoNextPoint()
    {
        // Regresa si no hay posiciones
        if (enem.posicionesPatrullaje.Count == 0)
            return;

        // Avanza a la posicion;
        agent.destination = enem.posicionesPatrullaje[currentPosition];


        currentPosition = (currentPosition + 1) % enem.posicionesPatrullaje.Count;

    }
}


