public class Coordinate
{
    private int _x, _y;
    private int[] _coordinates = new int[2];
    public int x
    {
        get => _x;
        set        
        {
            _x = value;
            _coordinates[0] = _x;
        }
    }

    public int y
    {
        get => _y;
        set
        {
            _y = value;
            _coordinates[1] = _y;
        }
    }

    public int[] coordinates
    {
        get => _coordinates;
        set
        {
            //clone is necessary so the array is not changed from other means that are not here
            _coordinates = (int[])value.Clone();
            _x = _coordinates[0];
            _y = _coordinates[1];
        }
    }

    public Coordinate(int[] coord = null,int Xcoord = 0, int Ycoord = 0)
    {
        if(coord == null)
        {
            x = Xcoord;
            y = Ycoord;
        }
        else
        {
            coordinates = coord;
        }
        
    }

    public Coordinate Clone()
    {
        return new Coordinate(coordinates);
        
    }


}
