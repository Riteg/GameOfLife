public class Cell
{
    public int x;
    public int y;
    public bool isAlive;

    public Cell(int x, int y, bool isAlive)
    {
        this.x = x;
        this.y = y;
        this.isAlive = isAlive;
    }

    public override string ToString()
    {
        return $"{x}, {y}";
    }
}