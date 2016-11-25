using UnityEngine;
using System;

[Serializable]
public class User{
    [SerializeField]
    private string email;
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

    public void SetEmail(string email) {
        this.email = email;
    }

    public string GetEmail() {
        return email;
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
