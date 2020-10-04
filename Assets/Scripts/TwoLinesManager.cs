using UnityEngine;

public class TwoLinesManager : MiniGameManager
{
    public LineFactory lineFactory;

    private Line drawnLine;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Start line drawing
            drawnLine = lineFactory.GetLine(pos, pos, 0.02f, Color.black);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drawnLine = null; // End line drawing
        }

        if (drawnLine != null)
        {
            drawnLine.end = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Update line end
        }*/

        CreateStraightLines();
    }

    private void CreateStraightLines()
    {
        // A length of 1 translates to "100% of the game space"
       // var start1 = Camera.main.ScreenToWorldPoint(new Vector2(-1, -1));
       //var end1 = Camera.main.ScreenToWorldPoint(new Vector2(1, 1));

        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        var scale = canv.GetComponent<RectTransform>().localScale;

        var gameOrigin = Camera.main.ScreenToWorldPoint(m_gameArea.GetComponent<RectTransform>().position);

        var realStart1 = (Vector2)gameOrigin + new Vector2(-10 / scale.x, -10 / scale.y);
        var realEnd1 = (Vector2)gameOrigin + new Vector2(10 / scale.x, 10 / scale.y);

        /*float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Camera.main.aspect;
        float gameWidth = width * 0.3f; // TODO tweak
        float gameHeight = height * 0.8f; // TODO tweak
        
        var realStart1 = (Vector2)gameOrigin + new Vector2(start1.x * gameWidth / 2, start1.y * gameHeight / 2);
        var realEnd1 = (Vector2)gameOrigin + new Vector2(end1.x * gameWidth / 2, end1.y * gameHeight / 2);*/


        var line1 = lineFactory.GetLine(realStart1, realEnd1, 0.05f, Color.black);


        /*var start1 = new Vector2(-1, 1f);
        var end1 = new Vector2(1, -1f);

        var gameOrigin = Camera.main.ScreenToWorldPoint(m_gameArea.GetComponent<RectTransform>().position);
        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Camera.main.aspect;
        float gameWidth = width * 0.3f; // TODO tweak
        float gameHeight = height * 0.8f; // TODO tweak

        var realStart1 = (Vector2)gameOrigin + new Vector2(start1.x * gameWidth / 2, start1.y * gameHeight / 2);
        var realEnd1 = (Vector2)gameOrigin + new Vector2(end1.x * gameWidth / 2, end1.y * gameHeight / 2);
        var line1 = lineFactory.GetLine(realStart1, realEnd1, 0.05f, Color.black);*/
    }
}
