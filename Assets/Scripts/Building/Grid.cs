using System;
using UnityEngine;
public class Grid <TGridObject>
{
    private int width, height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    private Vector3 OriginPos;

    public event EventHandler<OnGridValueChangedArgs> OnGridValueChanged;
    public class OnGridValueChangedArgs : EventArgs
    {
        public int x;
        public int z;
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignment textAlignment = TextAlignment.Center, int sortingLayer = 0)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingLayer);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    public Grid(int width, int height, float cellSize, Vector3 OriginPos, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.OriginPos = OriginPos;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebug = true;
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    debugTextArray[x, z] = CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPos(x, z) + new Vector3(cellSize, 0, cellSize) * .5f);
                    Debug.DrawLine(GetWorldPos(x, z), GetWorldPos(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPos(x, z), GetWorldPos(x + 1, z), Color.white, 100f);
                }
            }
        }

        OnGridValueChanged += (object sender, OnGridValueChangedArgs eventArgs) =>
        {
            debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
        };

        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width,height), Color.white, 100f);

    }

    private Vector3 GetWorldPos(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize +OriginPos;
    }

    private void GetXZ(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - OriginPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - OriginPos).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if(x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedArgs { x = x, z = z });
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPos, TGridObject value)
    {
        int x, z;
        GetXZ(worldPos, out x, out z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) return gridArray[x, y];
        return default(TGridObject);
    }


}
