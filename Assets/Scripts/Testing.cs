using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


public class Testing
{
    
    
    
    // https://www.mathopenref.com/coordpolygonarea.html
    [Test]
    public void TestCalculateArea()
    {
        List<Vector2> list = new List<Vector2>();

        list.Add(new Vector2(2, 7));
        list.Add(new Vector2(11, 7));
        list.Add(new Vector2(11, 2));
        list.Add(new Vector2(2, 2));

        float result = Util.CalculateArea(list);

        bool correctAnswer = result.Equals(45);
        
        Assert.IsTrue(correctAnswer, "Area should have been 45 but is " + result);
    }

    [Test]
    public void TestAddition()
    {

        List<Vector2> polygon = new List<Vector2>();
        
        polygon.Add(new Vector2(2, 7));
        polygon.Add(new Vector2(11, 7));
        polygon.Add(new Vector2(11, 2));
        polygon.Add(new Vector2(2, 2));


        polygon.Add(polygon[0]);

        float sum1 = 0;
        float sum2 = 0;

        float sum = 0;
        
        for (int i = 0; i < polygon.Count - 1; i++)
        {
            sum += (polygon[i].x * polygon[i + 1].y) - (polygon[i].y * polygon[i + 1].x);
            //sum2 += polygon[i].y * polygon[i + 1].x;
        }

        //float result = sum1 + sum2;
        float result = sum;
        
        bool correctAnswer = result.Equals(-90);
        Assert.IsTrue(correctAnswer, "Area should have been -90 but is " + result);
    }
}