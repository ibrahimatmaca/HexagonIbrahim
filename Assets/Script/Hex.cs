using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    private float distanceNeighbor = 0.93f;
    public LayerMask targetlayer;
    public int hexColorIndex = 0;

    public bool isMove = false;

    [Header("Gameobject")]
    public GameObject outlineChild;
    public List<GameObject> neighbors = new List<GameObject>();
    //public List<GameObject> sameColorHex = new List<GameObject>();
    public List<GameObject> selectedHex = new List<GameObject>();

    private void Update()
    {
        if(!isMove && !HexGrid2.instance.startGame) // Yalnızca bir sefer çalıştırmış olacağız
            ControlNeighbor2();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!HexGrid2.instance.isSwap && !HexGrid2.instance.startGame)
            neighbors.Add(other.gameObject);
    }

    public void RestartNeighbors() // Etrafımdaki obje listesini sil! Her obje pozisyon değiştirdiğinde bunu çağıracam
    {
        neighbors.Clear();//TÜm listeyi temizle
        //Listeye yeni elemanları ekle
        
        ControlUpdateNeighbor();
    }

    private void ControlUpdateNeighbor() // Sürekli etrafımdaki objeleri güncelliyor!
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, distanceNeighbor, targetlayer);
        for (int i = 0; i < hit.Length; i++)
        {
            if (neighbors.Count == 0)
                neighbors.Add(hit[i].gameObject);
            else
            {
                if (neighbors.Contains(hit[i].gameObject))
                    continue;
                else
                    neighbors.Add(hit[i].gameObject);
            }

        }
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (transform.name == neighbors[i].transform.name)
                neighbors.RemoveAt(i);
        }
        ControlNeighbor2();
    }

    /* 3-group control is done at the beginning!*/
    public void ControlNeighbor2()// 3 lü eşleşme olanlara bakıyor!
    {
        selectedHex.Add(transform.gameObject);

        foreach (GameObject first in neighbors)
        {
            if (first.GetComponent<Hex>().hexColorIndex == hexColorIndex)
            {
                selectedHex.Add(first);
            }
        }

        if (selectedHex.Count == 2)
        {
            foreach (GameObject second in selectedHex[1].GetComponent<Hex>().neighbors)
            { // 
                if (second.GetComponent<Hex>().hexColorIndex == hexColorIndex && neighbors.Contains(second))
                {
                    selectedHex.Add(second);
                }
            }
        }

        if (selectedHex.Count == 3) // Eşlesen objeleri burada bulabiliriz!!başlangıçta sadece renk değiştir
        {
            if (!HexGrid2.instance.isSwap && !HexGrid2.instance.startGame)
                HexGrid2.instance.SetColorHex(transform.gameObject, hexColorIndex);
            else
            {
                foreach (GameObject e in selectedHex)
                    HexGrid2.instance.hexDestroyList.Add(e);

                HexGrid2.instance.isSwap = false;
            }
        }
        selectedHex.Clear();
    }

}



/*
 * Sadece objeyi destroy et ve sonra da yeni obje oluştur!
 * 
 * 

*/
