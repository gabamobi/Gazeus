using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Playing,
    Paused,
    GameOver
}

public static class GameStateScript {

    private static State gameState;

    //Action funciona como delegate, um ponteiro para métodos void.
    public static Action OnGameOver;
    public static Action OnPaused;
    public static Action OnPlaying;

    /// <summary>
    /// Retorna o estado atual do jogo.
    /// </summary>
    /// <returns></returns>
    public static State GetGameState()
    {
        return gameState;
    }

    public static void SetGameState(State state)
    {
        switch (state)
        {
            case State.Playing:
                SetStatePlaying();
                break;

            case State.Paused:
                SetStatePaused();
                break;

            case State.GameOver:
                SetStateGameOver();
                break;

            default:
                break;
        }
    }

    private static void SetStateGameOver()
    {
        gameState = State.GameOver;
        OnGameOver();
    }

    private static void SetStatePaused()
    {
        gameState = State.Paused;
        OnPaused();
    }

    private static void SetStatePlaying()
    {
        gameState = State.Playing;
        OnPlaying();
    }

}
