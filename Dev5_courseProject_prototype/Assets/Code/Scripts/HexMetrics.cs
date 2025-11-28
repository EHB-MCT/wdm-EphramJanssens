using UnityEngine;

public static class HexMetrics
{
    public static float OuterRadius(float HexSize)
    {
        return HexSize;
    }

    public static float InnerRadius(float HexSize)
    {
        return HexSize * 0.866025404f;
    }

    public static Vector3[] Corners(float HexSize, HexOrientation orientation)
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            corners[i] = Corner(HexSize, orientation, i);
        }
        return corners;
    }

    public static Vector3 Corner(float HexSize, HexOrientation orientation, int index)
    {
        float angle = 60f * index;
        if (orientation == HexOrientation.FlatTop)
        {
            angle += 30f;
        }
        Vector3 corner = new Vector3(HexSize * Mathf.Cos(angle * Mathf.Deg2Rad),
        0f, HexSize * Mathf.Sin(angle * Mathf.Deg2Rad));
        return corner;
    }

    public static Vector3 Center(float HexSize, int x, int z, HexOrientation orientation)
    {
        Vector3 centerPosition;
        if (orientation == HexOrientation.FlatTop)
        {
            centerPosition.x = (x + z * 0.5f - z / 2) * (InnerRadius(HexSize) * 2f);
            centerPosition.y = 0f;
            centerPosition.z = z * (OuterRadius(HexSize) * 1.5f);
        }
        else
        {
            centerPosition.x = (x) * (OuterRadius(HexSize) * 1.5f);
            centerPosition.y = 0f;
            centerPosition.z = (z + x * 0.5f - x / 2) * (InnerRadius(HexSize) * 2f);
        }
        return centerPosition;
    }

    public static Vector2 CubeToAxial(Vector3 cube)
    {
        return new Vector2(cube.x, cube.y);
    }

    public static Vector2 OffsetToAxial(int x, int z, HexOrientation orientation)

    {

        if (orientation == HexOrientation.PointyTop)
        {
            return OffsetToAxialPointy(x, z);
        }
        else
        {
            return OffsetToAxialFlat(x, z);
        }
    }

    public static Vector2Int OffsetToAxialFlat(int col, int row)
  {
      int q = col;
      int r = row - (col + (col & 1)) / 2;
      return new Vector2Int(q, r);
  }
    public static Vector2Int OffsetToAxialPointy(int col, int row)
    {
        int q = col - (row + (row & 1)) / 2;
        int r = row;
        return new Vector2Int(q, r);
    }

    public static Vector2 CubeToOffset(int x, int y, int z, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return CubeToOffsetPointy(x, y, x);
        }
        else
        {
            return CubeToOffsetFlat(x, y, z);
        }
    }

    public static Vector2 CubeToOffset(Vector3 offsetCoord, HexOrientation orientation)
    {
        return CubeToOffset((int)offsetCoord.x, (int)offsetCoord.y, (int)offsetCoord.z, orientation);
    }

    private static Vector2 CubeToOffsetPointy(int x, int y, int z)
    {
        Vector2 offsetCoordinates = new Vector2(x + (y - (y & 1)) / 2, y);
        return offsetCoordinates;
    }

    private static Vector2 CubeToOffsetFlat(int x, int y, int z)
    {
        Vector2 offsetCoordinates = new Vector2(x, y + (x - (x & 1)) / 2);
        return offsetCoordinates;
    }

    private static Vector3 CubeRound(Vector3 frac)
    {
        Vector3 roundedCoordinates = new Vector3();
        int rx = Mathf.RoundToInt(frac.x);
        int ry = Mathf.RoundToInt(frac.y);
        int rz = Mathf.RoundToInt(frac.z);
        float xDiff = Mathf.Abs(rx - frac.x);
        float yDiff = Mathf.Abs(ry - frac.y);
        float zDiff = Mathf.Abs(rz - frac.z);

        if (xDiff > yDiff && xDiff > zDiff)
        {
            rx = -ry - rz;
        }
        else if (yDiff > zDiff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        roundedCoordinates.x = rx;
        roundedCoordinates.y = ry;
        roundedCoordinates.z = rz;
        return roundedCoordinates;
    }

    public static Vector2 AxialRound(Vector2 coordinates)
    {
        return CubeToAxial(CubeRound(AxialToCube(coordinates.x, coordinates.y)));
    }

    public static Vector2 CoordinateToAxial(float x, float z, float Hexsize, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return CoordinateToPointyAxial(x, z, Hexsize);
        }
        else
        {
            return CoordinateToFlatAxial(x, z, Hexsize);
        }
    }

    private static Vector2 CoordinateToPointyAxial(float x, float z, float Hexsize)
    {
        Vector2 pointyHexCoordinates = new Vector2();
        pointyHexCoordinates.x = (2f / 3 * x) / Hexsize;
        pointyHexCoordinates.y = (-1f / 3 * x + Mathf.Sqrt(3) / 3 * z) / Hexsize;
        return AxialRound(pointyHexCoordinates);
    }

    public static Vector2 CoordinateToFlatAxial(float x, float z, float HexSize)
    {
        Vector2 flatHexCoordinates = new Vector2();
        flatHexCoordinates.x = (Mathf.Sqrt(3) / 3 * x - 1f / 3 * z) / HexSize;
        flatHexCoordinates.y = (2f / 3 * z) / HexSize;
        return AxialRound(flatHexCoordinates);
    }

    public static Vector2 CoordinateToOffset(float x, float z, float Hexsize, HexOrientation orientation)
    {
        return CubeToOffset(AxialToCube(CoordinateToAxial(x, z, Hexsize, orientation)), orientation);
    }

    public static Vector3 AxialToCube(int q, int r)
    {
        return new Vector3(q, r, -q - r);
    }
    public static Vector3 AxialToCube(float q, float r)
    {
        return new Vector3(q, r, -q - r);
    }
    public static Vector3 AxialToCube(Vector2 axialCoord)
    {
        return AxialToCube(axialCoord.x, axialCoord.y);
    }

    public static Vector3Int AxialToCubeInt(Vector2Int axial)
    {
        int q = axial.x;
        int r = axial.y;
        int s = -q - r;
        return new Vector3Int(q,r,s);
    }

    public static int GetDistance(Vector2Int a, Vector2Int b)
    {
        Vector3Int ac = AxialToCubeInt(a);
        Vector3Int bc = AxialToCubeInt(b);
        return (Mathf.Abs(ac.x - bc.x) + Mathf.Abs(ac.y-bc.y) + Mathf.Abs(ac.z - bc.z)) / 2;
    }
}
