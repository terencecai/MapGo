using System;
using UnityEngine;


[Serializable]
public class Location{

    private double latitude;
    private double longitude;

    public Location(double latitude, double longitude) {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public Location(LocationInfo info) {
        this.latitude = info.latitude;
        this.longitude = info.longitude;
    }

    public void SetLatitude(double latitude) {
        this.latitude = latitude;
    }

    public void SetLongitude(double longitude) {
        this.longitude = longitude;
    }

    public double GetLongitude() {
        return longitude;
    }

    public double GetLatitude() {
        return latitude;
    }
}
