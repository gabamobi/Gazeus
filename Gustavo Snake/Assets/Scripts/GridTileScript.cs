using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileScript : TileScript {

    //SerializeField mostra o campo Private no Inspector, persiste sua informação entre Unity e Visual Studio.
    //Prefab é um Asset (arquivo físico) que pode ser entendido como um GameObject armazenado na pasta Project.
    //Quando você faz uma alteração em um prefab, todas suas instâncias, em todas as cenas, são alteradas automaticamente.
    [SerializeField] private GameObject applePrefab;

    //apple receberá as configurações de applePrefab.
    private GameObject apple; //Maça que a Snake come.

    private bool hasApple = false; //Informa se há uma Apple no Tile desta instância.
    public bool HasApple()
    {
        return hasApple;
    }

    /// <summary>
    /// Se o Tile já possuir uma Apple, não faz nada.
    /// Se não possuir, instancia uma sobre ele.
    /// </summary>
    /// <returns></returns>
    public bool SetApple()
    {
        if (hasApple)
        {
            return false;
        }
        else
        {
            //Quaternion.identity significa No Rotation, o objeto fica perfeitamente alinhado com os eixos de seu objeto Parent.
            apple = Instantiate(applePrefab, transform.position, Quaternion.identity);
            apple.transform.parent = transform;
            hasApple = true;
            return true;
        }
    }

    /// <summary>
    /// Chamado quando a Snake come a Apple
    /// </summary>
    /// <returns></returns>
    public bool TakeApple()
    {
        if (!hasApple)
        {
            return false;
        }
        else
        {
            hasApple = false;
            Destroy(apple.gameObject); //destrói a Apple
            apple = null;
            return true;
        }
    }

}
