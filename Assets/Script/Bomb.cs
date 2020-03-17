using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public Text bombCount;

    public int movesCount = 9;

    private void Update()
    {
        if (movesCount == 0)
            HexGrid2.instance.gameOver = true;
        bombCount.text = "Bomb Moves " + movesCount.ToString();
    }

}
