using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : SingletonScript<GameManagerScript>
{

    //Texto da tela de Game Over.
    [SerializeField] private GameObject GameOverText;

    //Texto da tela de Pause.
    [SerializeField] private GameObject PauseText;

    private SnakeScript snake;

    //Indica se Snake saiu dos limites de PlayArea.
    private bool snakeIsOutOfBounds = false;

    //Dispara quando uma maçã é coletada.
    public static Action OnApplePickup;

    //Dispara quando jogador ganha pontos.
    public static Action OnScoreAdd;

    //Armazena a pontuação do jogador.
    private int score;
    public int Score { get { return score; } }

    // Use this for initialization
    void Start () {
        //Define tile inicial da Grid e cria todos os outros tiles;
        GridManagerScript.Instance.InitializeGrid();

        //FindObjectOfType retorna a primeira instância carregada do objeto tipo SnakeScript.
        snake = FindObjectOfType<SnakeScript>();

        //Cria o primeiro tile da Snake, sua cabeça.
        snake.InitializeSnake();

        //Delegate dos métodos nas Actions de GameState.
        GameStateScript.OnGameOver += GameOver;
        GameStateScript.OnPlaying += Playing;
        GameStateScript.OnPaused += Paused;

        PlayerInputScript.AnyKeyPressed += AnyKeyPressed;
        PlayerInputScript.PausePressed += PausePressed;

        //Define estado do jogo como Playing.
        GameStateScript.SetGameState(State.Playing);

        //Gera uma maçã em um local aleatório dentro de PlayArea.
        SpawnApple();
    }

    private void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        //Apresenta texto da tela de GameOver
        GameOverText.SetActive(true);

        while (GameStateScript.GetGameState() == State.GameOver)
        {
            //Interrompe GameOverRoutine e chama WaitForEndOfFrame. 
            //No final de WaitForEndOfFrame chama GameOverRoutine novamente.
            yield return new WaitForEndOfFrame();
        }

        //Se não entrar no while acima, esconde Game Over.
        GameOverText.SetActive(false);
    }

    private void Playing()
    {
        StartCoroutine(PlayingRoutine());
    }

    /// <summary>
    /// Move a Snake a cada 0.2 segundos.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayingRoutine()
    {
        while (GameStateScript.GetGameState() == State.Playing)
        {
            //Tick chama função para mover a Snake. A Snake move continuamente em uma direção até o jogador tocar outro canto da PlayArea.
            snake.Tick();

            //Se Snake saiu dos limites da PlayArea...
            if (snakeIsOutOfBounds)
            {
                //Altera estado do jogo para Game Over.
                GameStateScript.SetGameState(State.GameOver);
            }
            else
            {
                //Se Snake ainda permanece dentro da PlayArea.

                CheckIfSnakeOnApple();
                CheckIfSnakeOnSnake();
            }

            AddScore(1);

            //WaitForSeconds suspende a execução de uma coroutine por uma quantidade de segundos, multiplicada por Time.timeScale.
            yield return new WaitForSeconds(0.2f);
        }
    }

    //Altera a pontuação do jogo
    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        //Se houver função vinculada à Action OnScoreAdd, chama ela.
        if (OnScoreAdd != null) OnScoreAdd();
    }

    //Verifica se Snake está sobre uma maçã.
    private void CheckIfSnakeOnApple()
    {
        //Obtém o objeto Tile que é a cabeça da Snake.
        SnakeTileScript tileAtHead = snake.GetHead();

        //Obtém objeto Tile da PlayArea que esteja abaixo da cabeça da Snake, ou null se não houver nenhum Tile (quando sai da PlayArea).
        GridTileScript gridTile = GridManagerScript.Instance.GetTileAt(tileAtHead.GridPos.x, tileAtHead.GridPos.y);

        if (gridTile == null) return;

        //Verifica se há uma maçã abaixo do Tile da cabeça da Snake.
        if (gridTile.HasApple())
        {
            SnakeOnApple(gridTile);
            //TODO Add to a score
            //Notify apple pick up
            SpawnApple();
        }
    }

    /// <summary>
    /// Verifica se a cabeça da Snake mordeu sua cauda.
    /// </summary>
    private void CheckIfSnakeOnSnake()
    {
        //Obtém a posição do Tile da cabeça da Snake.
        Vector2Int snakeHeadGridPosition = snake.GetHead().GridPos;

        //.Skip ignora o primeiro objeto da lista (cabeça da Snake) e retorna os restantes.
        foreach (SnakeTileScript snakeTile in snake.SnakePieces.Skip(1))
        {
            //Verifica se este Tile da Snake está tocando sua cabeça.
            if (snakeTile.GridPos == snakeHeadGridPosition)
            {
                //Caso afirmativo, Game Over do jogo.
                GameStateScript.SetGameState(State.GameOver);
            }
        }
    }

    /// <summary>
    /// Gera uma maçã na PlayArea.
    /// </summary>
    private void SpawnApple()
    {
        //Obtém um objeto Tile de posição aleatória da PlayArea.
        GridTileScript tile = GridManagerScript.Instance.GetRandomTile();

        //Cria uma maça sobre esse objeto Tile.
        tile.SetApple();
    }

    /// <summary>
    /// Chamada quando a Snake está sobre a maçã.
    /// </summary>
    /// <param name="gridTile"></param>
    private void SnakeOnApple(GridTileScript gridTile)
    {
        //TakeApple destrói a maçã.
        gridTile.TakeApple();

        //Este evento chama o método AppleEaten, que adiciona um Tile à Snake.
        OnApplePickup();

        //Acrescentamos 10 pontos à pontuação.
        AddScore(10);
    }

    private void Paused()
    {
        StartCoroutine(PauseRoutine());
    }

    /// <summary>
    /// Mantém a tela de Pause aparecendo até o jogar despausar.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PauseRoutine()
    {
        //Mostra o texto da tela de Pause
        PauseText.SetActive(true);

        while (GameStateScript.GetGameState() == State.Paused)
        {
            //WaitForEndOfFrame: espera até o final do frame depois da câmera e da Interface gráfica ser renderizada, 
            // isso ocorre antes do frame ser apresentado na tela.
            // Com isso controlamos que somente no final do próximo método Update esse aqui é chamado novamente.
            yield return new WaitForEndOfFrame();
        }

        //Se não entrar no while acima, oculta texto da tela de Pause.
        PauseText.SetActive(false);
    }

    /// <summary>
    /// Função chamada ao clicar no botão Pause.
    /// </summary>
    private void PausePressed()
    {
        if (GameStateScript.GetGameState() == State.Paused)
        {
            GameStateScript.SetGameState(State.Playing);
        }
        else if (GameStateScript.GetGameState() == State.Playing)
        {
            GameStateScript.SetGameState(State.Paused);
        }

    }

    /// <summary>
    /// Função chamada ao clicar em qualquer área da tela (com exceção do botão pause).
    /// </summary>
    private void AnyKeyPressed()
    {
        if (GameStateScript.GetGameState() == State.GameOver)
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Função chamada ao clicar em qualquer local da tela de Game Over.
    /// </summary>
    private void RestartGame()
    {
        score = 0;

        //Reinicia a PlayArea, reconstruindo seus tiles.
        GridManagerScript.Instance.Reset();
        snake.Reset();
        SpawnApple();
        GameStateScript.SetGameState(State.Playing);
    }

}

/*
O que são Coroutines:
https://pt.stackoverflow.com/questions/241437/o-que-s%C3%A3o-coroutines

São rotinas cooperativas, ou seja, são rotinas (funções, métodos, procedimentos) que concordam em parar sua execução 
permitindo que outra rotina possa ser executada naquele momento esperando que essa rotina secundária devolva a execução 
para ela em algum momento, portanto uma coopera com a outra.

Isto permite a execução em partes. Um dos grandes benefícios é manter algum estado entre os momentos de execução então sua principal 
função é facilitar a execução segmentada, possivelmente criando alguma abstração na execução. 

O principal mecanismo em linguagens para alcançar isto é o yield. Também pode usar uma máquina de estados manualmente ou por biblioteca.

Não tem tanta diferença assim das Threads, só que a suspensão de execução é determinada pela própria rotina e não pelo sistema operacional, 
por isso ela é chamada de cooperativa, o sistema operacional faz de forma preemptiva.

***********************
O que é yeld:
https://pt.stackoverflow.com/questions/44293/qual-a-utilidade-da-palavra-reservada-yield

Por uma questão de legado a sintaxe dele é sempre yield return, o mais usado, ou yield break.

O primeiro encerra o funcionamento de um método retornando um valor para quem chamou como é de se esperar de um return. 
Mas neste caso o valor é encapsulado em uma estrutura de dados que conta com um iterador criando um generator indicando 
onde ele parou para poder retomar dali.

O segundo encerra o método de forma "definitiva" encerrando o iterador.

Na verdade o yield é uma forma limitada de continuation.

Então podemos dizer, de outra forma, que o método com yield retorna um valor sem sair deste método. 
Claro que ele sai, mas sai sabendo onde parou e sabe que tem voltar lá quando ele for invocado novamente então dá a impressão que ele nunca saiu.

Ele gera o que poderíamos chamar de coleção de dados virtual temporária que é materializada mais tarde quando os dados são necessários de fato.

********************************

O que é máquina de estado:
https://pt.stackoverflow.com/questions/208328/o-que-%C3%A9-uma-m%C3%A1quina-de-estado


O assincronismo pode ser obtido através de uma máquina de estados já que ele precisa apenas garantir que não ocorra espera 
enquanto está fazendo algo potencialmente demorado, então ele precisa trocar o contexto de execução entre mais de uma parte da aplicação.

Esta é uma técnica antiga e bem conhecida usada em diversos problemas. Ela vai trocando um ou mais determinados estados de acordo 
com o que vai acontecendo em algo relacionado com o que ela está controlando.

*****************************
Programação assíncrona com Async e Await:
https://msdn.microsoft.com/pt-br/library/hh191443(v=vs.120).aspx



 */
