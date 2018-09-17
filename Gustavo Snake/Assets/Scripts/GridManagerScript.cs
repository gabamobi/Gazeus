using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerScript : SingletonScript<GridManagerScript>
{

    //SerializeField mostra o campo Private no Inspector, persiste sua informação entre Unity e Visual Studio.
    //Prefab é um Asset (arquivo físico) que pode ser entendido como um GameObject armazenado na pasta Project.
    //Quando você faz uma alteração em um prefab, todas suas instâncias, em todas as cenas, são alteradas automaticamente.
    [SerializeField] private GridTileScript gridTilePrefab;

    [SerializeField] private Transform playArea; //SerializeField podemos ver o campo no Inspector.

    //gridSize é private para evitarmos de acessá-la de fora de GridManager.
    [SerializeField] private int gridSize = 10; //SerializeField persiste os dados desse objeto.
    public int GridSize { get { return gridSize; } } 

    //startPoint é o ponto inferior esquerdo na PlayArea. Criaremos os Tiles da Grid a partir do StartPoint. 
    //Preencheremos a PlayArea com Tiles começando do StartPoint até cobri-la toda.
    //Cada Tile terá medida de um Unit, a quantidade de tiles pode mudar dependendo das dimensões da PlayArea.
    //Unit é a medida padrão usada em Position e Scale de Transform, etc.
    //Ex.: Objeto com Scale X Y e Z 1 teria 1 Unit em X Y Z.
    //Por padrão a física entende Unit como 1 metro, e a Interface, como 1 pixel.
    private Vector3 startPoint; //Vetor coordenadas X, Y e Z.
    public Vector3 StartPoint { get { return startPoint; } }

    private int width; //quantidade de tiles por linha.
    private int height; //quantidade de tiles po coluna.

    //Array de coordenadas X,Y, primeiro em 0,0 e o último em width,height.
    private Transform[,] grid; //Position X e Y da Grid.

    /// <summary>
    /// Inicializa a Grid ao instanciar essa classe.
    /// </summary>
    public void InitializeGrid()
    {
        //valor x de Scale de PlayArea vezes 10 (variável gridSize). Linhas da grid.
        //Mathf.RoundToInt retorna o inteiro mais próximo, pra mais ou pra menos.
        width = Mathf.RoundToInt(playArea.localScale.x * gridSize); //localScale X está 2.2, igual a 22.

        //valor y de Scale de PlayArea vezes 10 (variável gridSize). Colunas da Grid.
        //Mathf.RoundToInt se número for X.5 retorna o inteiro par mais próximo.
        height = Mathf.RoundToInt(playArea.transform.localScale.y * gridSize); //localScale Y está 1.271, igual a 12,71, vira 13.

        //grid é objeto Transform com X e Y definidos. Z será 0.
        grid = new Transform[width, height];

        //Renderer é o que faz o objeto aparecer na tela.
        //.bounds.min é o ponto na extremidade inferior esquerda da PlayArea (Vector3).
        startPoint = playArea.GetComponent<Renderer>().bounds.min;

        CreateGridTiles();

        //Debug.Log(playArea.GetComponent<Renderer>().bounds.max);
        //Debug.Log(startPoint);
    }

    /// <summary>
    /// Cria instâncias de GridTileScript
    /// </summary>
    private void CreateGridTiles()
    {
        //gridTilePrefab será null se não vincularmos nenhum Prefab ao objeto no Inspector.
        if (gridTilePrefab == null) return;

        //Y deve ser menor que altura da PlayArea.
        for (int y = 0; y < height; y++)
        {
            //X deve ser menor que largura da PlayArea.
            for (int x = 0; x < width; x++)
            {
                //X vai de 0 até 21, Y de 0 até 12, valores de Width 22 e Height 13.
                Vector3 worldPos = GetWorldPos(x, y);
                GridTileScript gridTile;

                //Quaternion.identity significa No Rotation, o objeto fica perfeitamente alinhado com os eixos de seu objeto Parent.
                //gridTilePrefab é Prefab com arquivo e configurações do Tile usado na grid.
                gridTile = Instantiate(gridTilePrefab, worldPos, Quaternion.identity);
                gridTile.name = string.Format("Tile({0},{1})", x, y);

                //Move Tile da Grid para sua posição definitiva.
                gridTile.MoveToGrid(x, y);

                //Mantém a posição do tile da grid na tela, mas altera seus valores passando a ter como base a posição de PlayArea e não a da Janela.
                gridTile.transform.parent = playArea.transform;

                //Armazenamos a posição deste tile no Array grid do tipo Transform[x,y]
                grid[x, y] = gridTile.transform;

            }
        }
    }

    /// <summary>
    /// Obtém a posição do Tile na Grid, chamada dentro de um for (loop).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Vector3 GetWorldPos(int x, int y)
    {
        float xf = x;
        float yf = y;

        // (xf / gridSize) = de 0 até 2,1; (yf / gridSize) = de 0 até 1,3. 
        // startPoint é o ponto na extremidade inferior esquerda da PlayArea.
        return new Vector3(startPoint.x + (xf / gridSize), startPoint.y + (yf / gridSize), startPoint.z);
    }

    // Use this for initialization
    void Start () {
        InitializeGrid();
    }

    public GridTileScript GetRandomTile(int margin = 0)
    {
        //width e height da grid em número de tiles.
        //se margin for menor que zero
        if ((margin > width || margin > height) && margin < 0)
        {
            return GetTileAt(0, 0);
        }

        //Se Grid tiver linha com 20 tiles e margin for 5.
        //Snake aparecerá entre o tile 5 e o 15 da linha.
        int x = UnityEngine.Random.Range(0 + margin, width - margin);
        int y = UnityEngine.Random.Range(0 + margin, height - margin);

        return GetTileAt(x, y);
    }

    public GridTileScript GetTileAt(int x, int y)
    {
        //Para evitar procurar Tile fora dos limites da grid de PlayArea.
        if ((x < width && x >= 0) && (y < height && y >= 0))
        {
            //Verifica se há no array grid um tile com width e height passados.
            if (grid[x, y] != null)
            {
                //.GetComponent<GridTileScript>: adicionamos esse componnete Transform à um componente GridTileScript e retornamos esse objeto.
                return grid[x, y].GetComponent<GridTileScript>();
            }
        }
        else
        {
            //GetTileAt é chamada para ver onde Snake está e para criar Tiles aleatórios. 
            //Somente onde verificamos a posição da Snake que pode ocorrer de tentar pegar um Tile fora dos limites de PlayArea.
            GameStateScript.SetGameState(State.GameOver);
            return null;
        }

        return null;
    }

    /// <summary>
    /// Esta função é chamada somente no modo de edição, ou quando adiciona um novo componente, ou ao clicar no botão Reset da Unity.
    /// </summary>
    public void Reset()
    {
        //Percorre todos os Tiles child de playArea, seu parent.
        foreach (Transform child in playArea.transform)
        {
            //Destroi o Tile child vinculado à PlayArea, seu parent.
            Destroy(child.gameObject);
        }

        //Recria a grid e seus tiles
        InitializeGrid();
    }

}
