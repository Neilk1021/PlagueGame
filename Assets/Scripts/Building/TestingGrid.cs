using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestingGrid : MonoBehaviour
{
    private Grid<GridObject> grid;
    [SerializeField] int Height, Width;
    [SerializeField] float Scale;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<GridObject>(Width, Height, Scale, transform.position - new Vector3(Scale,0), 
        (Grid<GridObject> g , int x, int z) => new GridObject (g,x,z));        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(worldPos);
            grid.SetGridObject(worldPos, new GridObject(grid, Width, Height));
        }
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x, z;

        public GridObject(Grid<GridObject> grid, int x , int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
    }
}
