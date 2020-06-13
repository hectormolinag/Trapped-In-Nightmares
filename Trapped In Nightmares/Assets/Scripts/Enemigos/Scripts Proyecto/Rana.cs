using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class Rana : Enemigo
{
    public NavMeshAgent navMeshAgent;


    public Vector3 posicionInicial = Vector3.zero;
   

    // Start is called before the first frame update
    void Start()
    {

        rigid = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;


        navMeshAgent = GetComponent<NavMeshAgent>();
        fsm = new FSM(gameObject, this);

        RanaEstadoSalto estadoSalto = new RanaEstadoSalto(this);
        IdleRana estadoIdle = new IdleRana(this);

        EnemigoDesvanecidoEstado estadoDesvanecido = new EnemigoDesvanecidoEstado(this);
        EnemigoAturdidoEstado estadoAturdido = new EnemigoAturdidoEstado(this);

        fsm.AddState(EstadosRana.Salto, estadoSalto);
        fsm.AddState(EstadosRana.IdleRana, estadoIdle);
        fsm.AddState(EstadosEnemigo.Aturdido, estadoAturdido);
        fsm.AddState(EstadosEnemigo.Desvanecido, estadoDesvanecido);
        Inicializar();

    }


    public void Inicializar()
    {
        fsm.ChangeState(EstadosRana.IdleRana);
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


        //transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, 120f * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(p - transform.position), 10f * Time.deltaTime);



    }



#if UNITY_EDITOR





    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);


        /*
        Gizmos.DrawSphere(punto1, 1f);
        Gizmos.DrawSphere(punto2, 1f);*/

        buscadorPlayer.EditorGizmo(transform);

    


    }


    private void OnDrawGizmos()
    {
        //Debug.Log(buscadorPlayer.Detect(transform, target, true));
    }



#endif



}


enum EstadosRana
{
    Salto,
    IdleRana,

}


public class IdleRana : State
{

    private Rana enem;
    private Vector3 rotationTarget = Vector3.zero;

    private NavMeshAgent agent;

    private Vector3 posicionTarget = Vector3.zero;

    bool aSaltar = false;

    public IdleRana(Rana enemigo)
    {
        enem = enemigo;
        agent = enem.GetComponent<NavMeshAgent>();
    }

    //Acción que realizará este estado  
    public override void Act(GameObject _object)
    {



        /*
        enem.transform.rotation = Quaternion.RotateTowards(enem.transform.rotation, Quaternion.Euler(rotationTarget), 20f * Time.deltaTime);
        //Debug.Log(Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)));
        if (Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)) < 2f)
        {
            RotationRandom();
        }
        */
        //enem.transform.LookAt(rotationTarget);

        
        if (!aSaltar && enem.buscadorPlayer.Detect(enem.transform, enem.target, true))
        {
            //Debug.Log("Encontró player");
            aSaltar = true;
            posicionTarget = enem.target.position;
            
        }


        //El jugador ha entrado al radio de salto, entones giramos hacia él antes de que saltemos
        if(aSaltar)
        {
            
            Vector3 targetDir = posicionTarget - enem.transform.position;
            float angulo = Vector3.Angle(targetDir, enem.transform.forward);

            enem.LootAt(posicionTarget);

            agent.SetDestination(posicionTarget);
            //enem.transform.LookAt(Vector3.MoveTowards(enem.transform.position, posicionTarget, 5f * Time.deltaTime));
            

            //Estando casi mirando hacia él saltamos
            // Debug.Log(angulo);
            if (angulo < 10f)
            {
                //Debug.Log(angulo);               //
                ChangeState(EstadosRana.Salto);
               // Debug.Log("Cambiar Estado");
            }
        }


    }


    //Condición para terminar este estado
    public override void Reason(GameObject _object)
    {
       
    }




    //Para cambiar Animación...
    public override void OnEnter(GameObject _object)
    {
        aSaltar = false;
        //Debug.Log("Estar quieto");
        agent.SetDestination(enem.posicionInicial);
        posicionTarget = Vector3.zero;

    }

    private void RotationRandom()
    {


        rotationTarget = Vector3.zero;
        Random.InitState(System.DateTime.Now.Millisecond);
        //this behavior is dependent on the update rate, so this line must
        //be included when using time independent framerate.
        //float JitterThisTimeSlice = wanderJitter * Time.deltaTime;

        //first, add a small random vector to the target's position
        rotationTarget += new Vector3(Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
        //wanderTarget += Random.onUnitSphere;
        rotationTarget.Normalize();
        rotationTarget *= 2f;
        rotationTarget.y = enem.transform.position.y;
    }

    public override void OnExit(GameObject _object)

    {
       
    }





    }


public class RanaEstadoSalto : State
{

    private Rana enem;
    private Vector3 rotationTarget = Vector3.zero;
    NavMeshAgent agent;
    bool estaSaltando = false;

    Coroutine salto;


    public RanaEstadoSalto(Rana enemigo)
    {
        enem = enemigo;
        agent = enem.GetComponent<NavMeshAgent>();
    }

    //Acción que realizará este estado  
    public override void Act(GameObject _object)
    {
       



       
        //Debug.Log(Quaternion.Angle(enem.transform.rotation, Quaternion.Euler(rotationTarget)));
      

        //enem.transform.LookAt(rotationTarget);


    }


    //Condición para terminar este estado
    public override void Reason(GameObject _object)
    {
        if (!estaSaltando && enem.buscadorPlayer.Detect(enem.transform, enem.target, true))
        {
            fsm.ChangeState(EstadosRana.IdleRana);
            //Debug.Log("Ha saltado");
        }
    }


    IEnumerator EsperarSalto()
    {

        yield return new WaitForSeconds(2);
        estaSaltando = false;
        enem.rigid.isKinematic = true;

        agent.enabled = true ;
    }


    //Para cambiar Animación...
    public override void OnEnter(GameObject _object)
    {
        //Debug.Log("Salto");

        enem.rigid.isKinematic = false;
        estaSaltando = true;

        agent.enabled = false;
        enem.rigid.AddForce(enem.transform.forward*100f);
        enem.rigid.AddForce(0f, 500f, 0f);

        salto = fsm.myMono.StartCoroutine(EsperarSalto());


    }


    public override void OnExit(GameObject _object)

    {
        enem.rigid.isKinematic = true;
        
    }

}




