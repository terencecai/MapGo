using UnityEngine;
using System;

[Serializable]
public class User{
    [SerializeField]
    private string payload;
    [SerializeField]
    private double latitude;
    [SerializeField]
    private double longitude;

    public User() {
    }

    public void SetLocation(LocationInfo location) {
        this.latitude = location.latitude;
        this.longitude = location.longitude;
    }

    public void SetLocation(Location location) {
        this.latitude = location.GetLatitude();
        this.longitude = location.GetLongitude();
    }

    public void SetPayload(string payload) {
        this.payload = payload;
    }

    public string GetPayload() {
        return payload;
    }

    public double GetLat() {
        return latitude;
    }

    public Location GetLocation() {
        return new Location(latitude, longitude);
    }

    public double GetLon() {
        return longitude;
    }   
}

[Serializable]
public class Users {
    public User[] nearest_players;

    public Users(User[] users) {
        nearest_players = users;
    }
}
