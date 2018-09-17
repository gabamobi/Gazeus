using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour {

    //Direções para onde a Snake se move.
    public enum Direction
    {
        up,
        right,
        down,
        left
    }

    public static Direction dir;

    //Action funciona como delegate, um ponteiro para métodos void.
    public static Action PausePressed;
    public static Action AnyKeyPressed;

    //Update é chamado a cada atualização de tela; tem duração varíavel, 
    // pode ser de 0.015 e pode passar de 0.04, depende do tempo de processamento da tarefa executada no frame.
    //Chamando Time.deltaTime dentro de Update ou FixedUpdate, podemos saber quanto tempo o método levou para ser executado.
    private void Update()
    {
        switch (GameStateScript.GetGameState())
        {
            case State.Playing:
                HandleInputPlaying();
                break;

            case State.Paused:
                HandleInputPaused();
                break;

            case State.GameOver:
                HandleInputGameOver();
                break;

            default:
                break;
        }
    }

    private void HandleInputPlaying()
    {
        //Obtém nome de tecla quando seta para esquerda ou direita é pressionada.
        float h = Input.GetAxisRaw("Horizontal"); //retorna 1 ou -1.

        //Obtém nome de tecla quando seta para cima ou baixo é pressionada.
        float v = Input.GetAxisRaw("Vertical");

        //Mathf.Abs: -10.5f retorna 10.5. -10 retorna 10.
        //Usamos Mathf.Abs aqui somente para a comparação. 
        //Eixo não pressionado é 0, pressionado pode ser maior ou menor que zero.
        if (Mathf.Abs(v) > Mathf.Abs(h))
        {
            //Na Unity maior que zero (>0) é pra cima, menor (<0) é pra baixo.
            dir = v > 0 ? Direction.up : Direction.down;
        }
        else if (Mathf.Abs(h) > Mathf.Abs(v))
        {
            //Menor que zero, esquerda, maior, direita.
            dir = h > 0 ? Direction.right : Direction.left;
        }

        //Se tocou precisamente no botão com nome Pause, pausa o jogo.
        if (Input.GetButtonDown("Pause"))
        {
            PausePressed();
        }
    }

    private void HandleInputPaused()
    {
        //Trata quando é pressionado botão com nome Pause.
        if (Input.GetButtonDown("Pause"))
        {
            PausePressed();
        }
    }

    private void HandleInputGameOver()
    {
        //Trata quando é pressionado qualquer tecla ou botão do mouse.
        if (Input.anyKeyDown)
        {
            AnyKeyPressed();
        }
    }

}
