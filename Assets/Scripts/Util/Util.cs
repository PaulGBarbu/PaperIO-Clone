using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Util : MonoBehaviour
{

    public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        Vector2 currentPosition2D = ToVector2D(point);
        List<Vector2> area2D = ToVector2DList(polygon);
        return IsPointInPolygon(currentPosition2D, area2D.ToArray());
    }
    
    public static float CalculateArea(List<Vector3> polygon)
    {
        return CalculateArea(ToVector2DList(polygon));
    }

    public static float CalculateArea(List<Vector2> polygon)
    {
        polygon.Add(polygon[0]);

        float sum = 0;

        for (int i = 0; i < polygon.Count - 1; i++)
        {
            sum += polygon[i].x * polygon[i + 1].y - polygon[i].y * polygon[i + 1].x;
        }

        return Math.Abs(sum * 0.5f);
    }
    
    // https://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
    private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon) {
        int polygonLength = polygon.Length, i=0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength-1];           
        endX = endPoint.x; 
        endY = endPoint.y;
        while (i<polygonLength) {
            startX = endX;           startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x;       endY = endPoint.y;
            //
            inside ^= ( endY > pointY ^ startY > pointY ) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
        }
        return inside;
    }

    private static Vector2 ToVector2D(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    private static List<Vector2> ToVector2DList(List<Vector3> list)
    {
        List<Vector2> result = new List<Vector2>();
        foreach (Vector3 v in list)
            result.Add(ToVector2D(v));
        return result;
    }
    
    

}
