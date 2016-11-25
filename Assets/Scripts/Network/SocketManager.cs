using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Threading;

public class SocketManager : MonoBehaviour {

    private CoPlayerManager coManager;
    private IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.10.23"), 9000);
    private UdpClient client = new UdpClient();
    private User user = new User();
    private Thread receiveThread;
    private Users users;

    public void Start() {
        coManager = GetComponent<CoPlayerManager>();
        receiveThread = new Thread(new ThreadStart(OnCoPlayersRecieve));
        receiveThread.IsBackground = true;
        receiveThread.Start();       
    }

    public void SendLocation(LocationInfo location) {
        user.SetEmail("hardos@inbox.ru");
        user.SetLocation(location);
        string json = JsonUtility.ToJson(user);
        byte[] data = Encoding.UTF8.GetBytes(json);
        try {
            client.Send(data, data.Length, remoteEndPoint);
        } catch(Exception err) {
            print(err.ToString());
        }
    }

    private void OnCoPlayersRecieve() {
        
        while(true) {
            try {
                IPEndPoint remoteEndPoin = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref remoteEndPoin);
                if(data != null) {
                    string coPlayersJson = Encoding.UTF8.GetString(data);
                    users = JsonUtility.FromJson<Users>(coPlayersJson);
                    
                }

            } catch(Exception err) {
                print(err.ToString());
            }
        }
    }

    public Users GetUsers() {
        return users;
    }   

}
