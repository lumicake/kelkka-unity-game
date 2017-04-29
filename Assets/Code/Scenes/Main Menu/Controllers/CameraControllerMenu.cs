using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraControllerMenu : MonoBehaviour {
    List<Vector3[]> navPoints = new List<Vector3[]>();
    Vector3[] lookAtPoints;
    float flightTime = 0f;
    int currentNavPoint = 0;

    [SerializeField]
    float timePerPoint = 6f;
    [SerializeField]
    bool useInterpolation = false;

    void Start () {
        Transform flightpath = transform.parent.transform.GetChild(1);
        Transform lookAts = transform.parent.transform.GetChild(2);
        int numberNavPoints = flightpath.childCount;
        lookAtPoints = new Vector3[numberNavPoints];

        for (int i = 0; i < numberNavPoints; i++) {
            Transform thisNavPoint = flightpath.GetChild(i);
            int numberPoints = thisNavPoint.childCount;

            Vector3[] points = new Vector3[numberPoints];

            for (int j = 0; j < numberPoints; j++) {
                points[j] = thisNavPoint.GetChild(j).position;
            }

            navPoints.Add(points);
            lookAtPoints[i] = lookAts.GetChild(i).position;
        }

        transform.position = navPoints[0][0];
        //currentNavPoint = 16;
        //flightTime = 0.5f * timePerPoint;
    }
    
    void Update () {
        flightTime += Time.deltaTime;
        int nextNavPoint = currentNavPoint + 1;
        if (nextNavPoint > navPoints.Count - 1)
            nextNavPoint = 0;

        if (flightTime > timePerPoint) {
            flightTime = 0f;
            currentNavPoint++;

            if (currentNavPoint > navPoints.Count - 1)
                currentNavPoint = 0;
        }

        //Vector3 newPos1 = CalculateCubicBezierPoint(navPoints[currentNavPoint],
        //                                            flightTime / timePerPoint);
        transform.position = Vector3.Lerp(transform.position,
                                          CalculateCubicBezierPoint(navPoints[currentNavPoint],
                                          flightTime / timePerPoint),
                                          Time.deltaTime * 2f);
        Quaternion rotation;

        if (useInterpolation)
            rotation = Quaternion.LookRotation(
                Vector3.Lerp(lookAtPoints[currentNavPoint],
                             lookAtPoints[nextNavPoint],
                             flightTime / timePerPoint) - transform.position);
        else {
            Vector3 lookVector = lookAtPoints[currentNavPoint] - transform.position;
            rotation = Quaternion.LookRotation(lookVector);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
    }

    Vector3 CalculateQuadraticBezierPoint(Vector3[] positions, float time) {
        Vector3 pos0 = positions[0];
        Vector3 pos1 = positions[1];
        Vector3 pos2 = positions[2];

        Vector3 point = Mathf.Pow(1f - time, 2f) * pos0;
        point += 2f * (1f - time) * time * pos1;
        point += Mathf.Pow(time, 2f) * pos2;

        return point;
    }

    Vector3 CalculateCubicBezierPoint(Vector3[] positions, float time) {
        Vector3 pos0 = positions[0];
        Vector3 pos1 = positions[1];
        Vector3 pos2 = positions[2];
        Vector3 pos3 = positions[3];

        Vector3 point1 = CalculateQuadraticBezierPoint(new Vector3[3] { pos0, pos1, pos2 }, time);
        Vector3 point2 = CalculateQuadraticBezierPoint(new Vector3[3] { pos1, pos2, pos3 }, time);
        Vector3 point = (1f - time) * point1 + time * point2;

        return point;
    }
}
