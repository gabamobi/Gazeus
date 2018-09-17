using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileScript : MonoBehaviour {

    //Protected pois queremos permitir que somente as classes que herdarem esta tenham permissão para alterar gridPos.
    protected Vector2Int gridPos;
    public Vector2Int GridPos { get { return gridPos; } }

    /// <summary>
    /// Atualiza posição tanto do Tile da Snake quanto de gridPos.
    /// </summary>
    /// <param name="x">gridPos.X + 1 ou gridPos.X - 1, move tile da cabeça da Snake para direita ou para esquerda.</param>
    /// <param name="y">gridPos.Y + 1 ou gridPos.Y - 1, move tile da cabeça da Snake para baixo ou para cima.</param>
    public void MoveToGrid(int x, int y)
    {
        float xf = x;
        float yf = y;

        //Movemos em 1 Unit o Tile da Snake.
        //GridSize está como constante 10. StartPoint são coordenadas X,Y e Z do ponto na extremidade inferior esquerda da PlayArea.
        //Acessamos transform do Objeto da Unity onde TileScript é um componente.
        transform.position = new Vector3(
            GridManagerScript.Instance.StartPoint.x + (xf / GridManagerScript.Instance.GridSize),
            GridManagerScript.Instance.StartPoint.y + (yf / GridManagerScript.Instance.GridSize),
            transform.position.z);

        //Atualizamos gridPos.
        gridPos = new Vector2Int(x, y);
    }

    public void MoveToTile(TileScript tile)
    {
        MoveToGrid(tile.gridPos.x, tile.gridPos.y);
    }

}
