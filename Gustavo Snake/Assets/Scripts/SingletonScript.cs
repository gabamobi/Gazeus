using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScript<T> : MonoBehaviour where T : SingletonScript<T> //exemplo: GridManager : Singleton<GridManager>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    /// <summary>
    /// O método Awake é chamado quando a instância de script está sendo carregada.
    /// 
    /// É usado para inicializar qualquer variável ou estado do jogo antes do jogo começar. Awake é chamado apenas 
    /// uma vez durante a existência da instância de script. O Awake é chamado depois que todos os objetos são inicializados, 
    /// para que você possa falar com segurança com outros objetos ou consultá-los usando, por exemplo, GameObject.FindWithTag
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    /// <summary>
    /// O método OnDestroy ocorre quando uma cena ou jogo termina. Parar o modo de reprodução quando executado de dentro do Editor terminará o aplicativo.
    /// Quando isso acontecer, um OnDestroy será executado. Além disso, se uma cena for fechada e uma nova cena for carregada, a chamada OnDestroy será feita.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
