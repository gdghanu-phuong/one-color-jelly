using UnityEngine;

public class BlockData 
{
    public int Row;
    public int Col;
    public string Color;
    public GameObject Block;

    public BlockData(int row, int col, string color, GameObject block)
    {
        Row = row;
        Col = col;
        Color = color;
        Block = block;
    }
}
