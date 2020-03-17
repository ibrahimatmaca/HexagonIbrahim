using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexGrid2 : MonoBehaviour
{
    public static HexGrid2 instance;

    private float xIteration;
    private float yIteration;
    private float fWidth;
    private float fHeight;

    public int gridSizeX;
    public int gridSizeY;
    public int score;
    public int movesCount;

    public bool gameOver = false;
    public bool isSwap = false;
    public bool startGame = false;

    [Header("UI")]
    public Text scoreText;
    public Text movesText;
    public GameObject gameOverPanel;

    [Header("Prefab & List")]
    public GameObject flatHexagon;
    public List<GameObject> hexagonList = new List<GameObject>();
    public List<GameObject> selectedList = new List<GameObject>();
    public List<GameObject> hexDestroyList = new List<GameObject>();

    private List<GameObject> hexRowList = new List<GameObject>();

    [Header("Start Position")]
    public Vector2 startCreate;

    [Header("Colors")]
    public List<Color> colorsHex = new List<Color>();

    void Awake()
    {
        instance = this;
        CreateFlatHexGridStart();
    }

    void Start()
    {
        /*UI başlangıç için görsellik açısından*/
        scoreText.text = "Score: " + score.ToString();
        movesText.text = "Moves: " + movesCount.ToString();

        InvokeRepeating("CalculatorNewNeighbor", 2, 2);
    }

    private void Update()
    {
        if (gameOver)
            gameOverPanel.SetActive(true);
    }

    /*It is a function that aligns according to the x and y values ​​of the object to be used. */
    private void CreateFlatHexGridStart()
    {
        MeasureFlatHexTile();

        for (int xColumn = 0; xColumn < gridSizeX; xColumn++)
        {
            for (int yColumn = 0; yColumn < gridSizeY; yColumn++)
            {
                float xPos = xColumn * (fWidth * 32 / 40);
                float yPos = yColumn * (fHeight * 36 / 40);
                float offset;
                offset = 0;

                if (xColumn % 2 == 1)
                    offset = (xColumn - xColumn + fHeight / 2.3f);
                else
                    offset = 0;

                GameObject hexTile = Instantiate(flatHexagon, transform);
                hexTile.transform.position = new Vector2(startCreate.x + xPos, startCreate.y + yPos + offset);
                SetColorHex(hexTile, -1);
                hexTile.name = xColumn.ToString() + yColumn.ToString();
                hexagonList.Add(hexTile);
                
            }
        }
        yIteration = Mathf.Abs(hexagonList[0].transform.position.y - hexagonList[1].transform.position.y);
    }
    /*Using the height and width of the object.*/
    private void MeasureFlatHexTile()
    {
        fWidth = flatHexagon.GetComponent<SpriteRenderer>().bounds.size.x;
        fHeight = flatHexagon.GetComponent<SpriteRenderer>().bounds.size.y;
    }
    /*This function affects the sprite.color component for different colors of the object */
    public void SetColorHex(GameObject _gameObject, int _old)
    {
        if (_old == -1)
        {
            int random = Random.Range(0, colorsHex.Count);
            SpriteRenderer _sprite = _gameObject.GetComponent<SpriteRenderer>();
            _sprite.color = new Color(colorsHex[random].r, colorsHex[random].g, colorsHex[random].b);
            _gameObject.GetComponent<Hex>().hexColorIndex = random; // Burada atamamızın sebebi aynı renk koduna sahip objeleri bulmak
        }
        else
        {
            int rand;
            do
            {
                rand = Random.Range(0, colorsHex.Count);
            } while (rand == _old);

            SpriteRenderer _sprite = _gameObject.GetComponent<SpriteRenderer>();
            _sprite.color = new Color(colorsHex[rand].r, colorsHex[rand].g, colorsHex[rand].b);
            _gameObject.GetComponent<Hex>().hexColorIndex = rand;
        }
    }
    /*To select only 3 objects and assign selectList to selected ones*/
    public void SelectObject(GameObject selectedParent, Vector2 selectedPosition)
    {
        startGame = true;
        isSwap = true;
        if (selectedList != null)
        {
            OutLineControl(false);
            selectedList.Clear();
        }

        selectedList.Add(selectedParent);
        Vector2 parentPos = selectedParent.transform.position; // Tıkladığım obje pozisyon
        List<GameObject> neighbors = selectedParent.GetComponent<Hex>().neighbors; // Seçilen objenin komşularını al

        GameObject selectFirst = null;
        GameObject selectSecond = null;

        Vector2 distance = new Vector2(neighbors[0].transform.position.x, neighbors[0].transform.position.y);
        float distance2 = Vector2.Distance(distance, selectedPosition);

        foreach (GameObject e in neighbors)
        { // ilk elemanı uzaklığa göre seçiyor tıkladığım noktaya göre
            if (e != null)
                distance = new Vector2(e.transform.position.x, e.transform.position.y);
            if (Vector2.Distance(distance, selectedPosition) < distance2)
            {
                selectFirst = e;
                distance2 = Vector2.Distance(selectedPosition, distance);
            }
        }
        if (selectFirst != null)
        { // ilk obje ile parent tıkladığım objenin ortak noktası alınıyor!
            foreach (GameObject e in selectedParent.GetComponent<Hex>().neighbors)
            {
                if (e != null)
                {
                    if (selectFirst.GetComponent<Hex>().neighbors.Contains(e))
                    {
                        selectSecond = e;
                    }
                }
            }
        }
        if (selectFirst != null && selectSecond != null)
        {
            selectedList.Add(selectFirst);
            selectedList.Add(selectSecond);
            //selectedParent.GetComponent<Hex>().isMove = true;
            //selectFirst.GetComponent<Hex>().isMove = true;
            //selectSecond.GetComponent<Hex>().isMove = true;
        }

        OutLineControl(true);
    }

    public void DestroyListAndUpdateNeighbors()
    { // Oluşturulan üçgenleri yok etmek için!
        if (startGame  && hexDestroyList.Count > 0 && !isSwap)
        {
            while (true)
            {
                if (hexDestroyList.Count == 0)
                    break;
                HexMapController(hexDestroyList[0], 0);
                hexDestroyList.RemoveAt(0);
            }
            hexDestroyList.Clear();
        }
    }

    private void CalculatorNewNeighbor()
    {
        if (!isSwap && startGame && !gameOver)
        {
            foreach (GameObject my in hexagonList)
                my.GetComponent<Hex>().RestartNeighbors();
        }
        DestroyListAndUpdateNeighbors();
    }

    /*Open and close the outer circle*/
    private void OutLineControl(bool _outlineOpen)
    {
        if (_outlineOpen)
        {
            for (int i = 0; i < selectedList.Count; i++)
            {
                if (selectedList[i] != null)
                    selectedList[i].GetComponent<Hex>().outlineChild.SetActive(_outlineOpen);
            }
        }
        else
        {
            for (int i = 0; i < selectedList.Count; i++)
            {
                if (selectedList[i] != null)
                    selectedList[i].GetComponent<Hex>().outlineChild.SetActive(_outlineOpen);
            }
        }
        isSwap = false; // dönme kısmında tekrar kullanılsın
    }

    //Eğer patlarsa olacaklar ve 
    private void HexMapController(GameObject selected, int index)
    {
        
        if (selected != null)
        {
            int hex_index = hexagonList.IndexOf(selected);
            float x = selected.transform.position.x;
            float y = selected.transform.position.y;
            foreach (GameObject e in hexagonList)
            {
                if (e.transform.position.x == x && e.transform.position.y > selected.transform.position.y)
                {
                    hexRowList.Add(e);
                }
            }

            Destroy(selected);

            for (int i = 0; i < hexRowList.Count; i++)
            {
                hexRowList[i].transform.position = new Vector2(x, hexRowList[i].transform.position.y - yIteration);
            }
            int hexCount = hexRowList.Count;
            hexRowList.Clear();


            hexagonList[hex_index] = Instantiate(flatHexagon, new Vector2(x, y + hexCount * yIteration), Quaternion.identity, transform);
            SetColorHex(hexagonList[hex_index], 0);
            hexagonList[hex_index].name = hex_index.ToString();

            score += 5;
            scoreText.text = "Score: " + score.ToString();

        }
    }

    // Buradan aşağısına sonra bakarız!
    public IEnumerator UnClockWise()
    {
        Vector2 startPos;
        isSwap = true;
        for (int i = 0; i < selectedList.Count; i++)
        {
            if (isSwap)
            {
                startPos = selectedList[1].transform.position;
                selectedList[1].transform.position = selectedList[0].transform.position;
                selectedList[0].transform.position = selectedList[2].transform.position;
                selectedList[2].transform.position = startPos;

                SelectedRestart(selectedList[0]);
                SelectedRestart(selectedList[1]);
                SelectedRestart(selectedList[2]);

                yield return new WaitForSeconds(1f);
            }
        }
        OutLineControl(false);
        selectedList.Clear();
        if (hexDestroyList.Count > 0)
        {
            movesCount += 1;
            movesText.text = "Moves: " + movesCount.ToString();
        }
        DestroyListAndUpdateNeighbors();
        
        isSwap = false;
    }

    public IEnumerator ClockWise()
    {
        isSwap = true;
        Vector2 startPos;
        for (int i = 0; i < selectedList.Count; i++)
        {
            if (isSwap)
            {
                startPos = selectedList[2].transform.position;
                selectedList[2].transform.position = selectedList[1].transform.position;
                selectedList[1].transform.position = selectedList[0].transform.position;
                selectedList[0].transform.position = startPos;

                SelectedRestart(selectedList[0]);
                SelectedRestart(selectedList[1]);
                SelectedRestart(selectedList[2]);

                yield return new WaitForSeconds(1f);
            }
        }
        OutLineControl(false);
        selectedList.Clear();
        if (hexDestroyList.Count > 0)
        {
            movesCount += 1;
            movesText.text = "Moves: " + movesCount.ToString();
        }

        DestroyListAndUpdateNeighbors();
        
        isSwap = false;
    }

    private void SelectedRestart(GameObject _selectedGameO)
    {
        _selectedGameO.GetComponent<Hex>().RestartNeighbors();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
