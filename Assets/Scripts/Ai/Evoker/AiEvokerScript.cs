using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiEvokerScript : MonoBehaviour
{
    public enum EvokerStates
    {
        Idle,
        CombatMoving,
        Chasing,
        Evoking,
    }

    [SerializeField]
    private EvokerStates evokerState;

    void Start()
    {
        
    }

    IEnumerator Idle()
    {
        //Executa antes do "Update"
        while (evokerState == EvokerStates.Idle)
        {

            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado) (Tem que sair do while quando muda de estado)

    }

    IEnumerator CombatMoving()
    {
        //Executa antes do "Update"
        while (evokerState == EvokerStates.CombatMoving)
        {

            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado
    }

    IEnumerator Chasing()
    {
        //Executa antes do "Update"
        while (evokerState == EvokerStates.Chasing)
        {

            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado
    }

    IEnumerator Evoking()
    {
        //Executa antes do "Update"
        while (evokerState == EvokerStates.Evoking)
        {

            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado
    }


}
