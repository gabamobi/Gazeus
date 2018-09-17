using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour {

    //Private pois não deve ser acessado por outra classe classe. SerializeField para ser acessada da Unity (Inspector).
    [SerializeField] private SnakeTileScript snakeTilePrefab;

    private List<SnakeTileScript> snakePieces;
    public List<SnakeTileScript> SnakePieces { get { return snakePieces; } } // Readonly

    /// <summary>
    /// Cria o primeiro tile da Snake, sua cabeça.
    /// </summary>
    public void InitializeSnake()
    {
        snakePieces = new List<SnakeTileScript>();

        //Vinculando método à Action OnApplePickup.
        GameManagerScript.OnApplePickup += AppleEaten;

        //Debug.Log(GridManagerScript.Instance.GetRandomTile(5).transform.position);

        //Obtém um Tile aleatório da PlayArea.
        GridTileScript gridTile = GridManagerScript.Instance.GetRandomTile(5);

        //A Snake começa sobre esse Tile aleatório. Sua posição inicial muda a cada jogo.
        transform.position = gridTile.transform.position;

        //Instanciamos a snake posicionada sobre o Tile obtido acima.
        //Instantiate instancia objeto passado como primeiro parâmetro. Quaternion.identity é a mesma coisa que No Rotation.
        SnakeTileScript snakeTile = Instantiate(snakeTilePrefab, transform.position, Quaternion.identity);

        //Position, rotation e scale de snakeTile (Prefab) passa a ter os valores de Snake (Player) como ponto referencial.
        //Player começa em Position 0,0,0.
        snakeTile.transform.parent = transform;

        //Movemos o tile de Snake (snakeTile) para cima do Tile aleatório.
        snakeTile.MoveToTile(gridTile);

        //Adicionamos esse SnakeTile (Prefab) à lista de SnakeTiles.
        snakePieces.Add(snakeTile);

    }

    /// <summary>
    /// Quando uma maça é comida, adiciona um Tile à Snake.
    /// </summary>
    private void AppleEaten()
    {
        AddPiece();
    }

    private void AddPiece()
    {
        //transform pega position, rotation e scale do objeto da unity a qual está anexado.
        SnakeTileScript snakeTile = Instantiate(snakeTilePrefab, transform.position, Quaternion.identity);

        //snakeTile pega rotation, position e scale do Tile da Snake que ja existe.
        snakeTile.transform.parent = transform;

        //Move snakeTile para posição do último Tile da lista de snakePieces.
        snakeTile.MoveToTile(snakePieces[snakePieces.Count - 1]);

        //Move o snakeTile na PlayArea para direção inversa da qual a Snake está se movendo.
        switch (PlayerInputScript.dir)
        {
            case PlayerInputScript.Direction.up:
                snakeTile.MoveDown();
                break;
            case PlayerInputScript.Direction.down:
                snakeTile.MoveUp();
                break;
            case PlayerInputScript.Direction.right:
                snakeTile.MoveLeft();
                break;
            case PlayerInputScript.Direction.left:
                snakeTile.MoveRight();
                break;
        }

        //Agora esse snaketile ocupa a última posição da lista de snakePieces.
        snakePieces.Add(snakeTile);

    }

    public void Tick()
    {
        Move();
    }

    /// <summary>
    /// Move a Snake. Começa pela cauda e move a cabeça por último.
    /// </summary>
    private void Move()
    {
        //For começa no total de Tiles da Snake até 0.
        for (int i = snakePieces.Count - 1; i >= 0; i--)
        {
            //Tile 0 é a cabeça da Snake.
            if (i == 0)
            {
                //Move a cabeça da Snake em 1 Unit na direção indicada pelo jogador.
                switch (PlayerInputScript.dir)
                {
                    case PlayerInputScript.Direction.up:
                        snakePieces[i].MoveUp();
                        break;

                    case PlayerInputScript.Direction.down:
                        snakePieces[i].MoveDown();
                        break;

                    case PlayerInputScript.Direction.right:
                        snakePieces[i].MoveRight();
                        break;

                    case PlayerInputScript.Direction.left:
                        snakePieces[i].MoveLeft();
                        break;
                }
            }
            else
            {
                //Move Tile da Snake para posição do Tile anterior à ele. Faz isso até o segundo. O primeiro é a cabeça, entra no If acima.
                snakePieces[i].MoveToTile(snakePieces[i - 1]);
            }

        }
    }

    public SnakeTileScript GetHead()
    {
        //Retorna o objeto Tile 0, a cabeça da Snake.
        return snakePieces[0];
    }

    /// <summary>
    /// Esta função é chamada somente no modo de edição, ou quando adiciona um novo componente, ou ao clicar no botão Reset da Unity.
    /// </summary>
    public void Reset()
    {
        //Percorre todos os Tiles child de Snake, seu parent.
        foreach (Transform child in transform)
        {
            //Destroi o Tile child vinculado à Snale, seu parent.
            Destroy(child.gameObject);
        }

        //Remove o evento vinculado à Action OnApplePickup
        GameManagerScript.OnApplePickup -= AppleEaten;

        //Recria a Snake, do zero.
        InitializeSnake();
    }

}
