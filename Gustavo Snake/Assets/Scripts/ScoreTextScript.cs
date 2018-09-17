using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextScript : MonoBehaviour {

    private Text text;

    // Use this for initialization
    void Start()
    {
        //Pega o Text do Canvas que usamos na Unity para apresentar pontuação.
        text = GetComponent<Text>();

        //Vincula à Action OnScoreAdd o método ChangeText.
        GameManagerScript.OnScoreAdd += ChangeText;
    }

    /// <summary>
    /// Atualiza a pontuação na tela.
    /// </summary>
    private void ChangeText()
    {
        //{0:D5} preenche à esquerda com 0. Decimal com 5 posições.
        //https://sites.google.com/site/tecguia/formatar-string-c-string-format
        text.text = string.Format("{0:D5}", GameManagerScript.Instance.Score);
    }

}
