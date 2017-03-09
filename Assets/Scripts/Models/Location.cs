using System;
using UnityEngine;

using MapzenGo.Helpers;

[Serializable]
public class Location{

    public double latitude;
    public double longitude;

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

    public Location toRadians()
    {
        return new Location(GM.toRadians(latitude), GM.toRadians(longitude));
    }

    public Location toDegrees()
    {
        return new Location(GM.toDegrees(latitude), GM.toDegrees(longitude));
    }
}
