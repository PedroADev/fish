using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Flock : MonoBehaviour
{
    //variavel que define o floackManager
    public FlockManager myManager;

    //velocidade do peixe
    public float speed;

    //variavel que verifica se o peixe esta rodando
    bool turning = false;
    
    //o flockmanager esta definindo uma velocidade para o objeto com base em um valor minimo e um valor maximo
    void Start()
    {
        speed = Random.Range(myManager.minSpeed,
        myManager.maxSpeed);
    }
    void Update()
    {

        //delimita a area para o nado dos peixes, com os limites definidos no flockmanager
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);

        //variavel usada para identificar colisões
        RaycastHit hit = new RaycastHit();

        //define a direção com objeto com base na localização do flockmanager
        Vector3 direction = myManager.transform.position - transform.position;

        //caso o objeto esteja fora da area limite:
        if (!b.Contains(transform.position))
        {
            //define a rotacao como verdadeira
            turning = true;

            //objeto tem como direcao o flockmanager
            direction = myManager.transform.position - transform.position;
        }
        //ou, verifica com raycast se tem alguma possivel colisao proxima
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            //define a rotacao como verdadeira
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        //caso contrario, nao ativa a rotacao
        else
            turning = false;

        //caso a rotacao esteja como verdadeira:
        if (turning)
        {
            //rotaciona ate a nova direcao
            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(direction),
            myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {

            if (Random.Range(0, 100) < 10)

                //define uma nova velocidade aleatoria com base no flockmanager
                speed = Random.Range(myManager.minSpeed,
                myManager.maxSpeed);

            //aplica o metodo Apply rules
            if (Random.Range(0, 100) < 20)
                ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
    }
    void ApplyRules()
    {
        //obtem os objetos do cardume
        GameObject[] gos;
        gos = myManager.allFish;

        //define o centro do cardume
        Vector3 vcentre = Vector3.zero;

        //evitar colisoes
        Vector3 vavoid = Vector3.zero;

        //define a velocidade do cardume
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        //todos os objetos do cardume:
        foreach (GameObject go in gos)
        {
            //verifica se o objeto atual nao e o mesmo do script
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                //verifica se a distancia entre os objetos esta dentro do limite estabelecido
                if (nDistance <= myManager.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    //verifica se a distancia e menor que 1
                    if (nDistance < 1.0f)
                    {
                        //calcula uma direcao para evitar colisoes baseada nas posicoes dos objetos
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    //pega o flock do objeto
                    Flock anotherFlock = go.GetComponent<Flock>();

                    //soma a velocidade de todos os peixes do cardume
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        //caso haja peixe no cardume:
        if (groupSize > 0)
        {
            //calcula o centro do cardume
            vcentre = vcentre / groupSize + (myManager.goalPos - this.transform.position);

            //calcula a velocidade media do cardume
            speed = gSpeed / groupSize;

            //define uma nova direcao, baseada no centro do cardume e na direcao
            Vector3 direction = (vcentre + vavoid) - transform.position;

            //caso a direcao seja diferente de zero, faz uma rotacao ate a nova direcao
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                myManager.rotationSpeed * Time.deltaTime);
        }
    }

}