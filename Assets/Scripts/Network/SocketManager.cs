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
	// private IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("192.168.10.170"), 9000);
    private IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("35.156.153.137"), 9000);
    private UdpClient client = new UdpClient();
    private User user = new User();
    private Thread receiveThread;
    private Users users;

	public DataPacket lastDataPacket;

    public void Start() {
        coManager = GetComponent<CoPlayerManager>();
        receiveThread = new Thread(new ThreadStart(onDataDiagramReceive));
        receiveThread.IsBackground = true;
        receiveThread.Start();       
    }

    public void SendLocation(LocationInfo location) {
        var token = PlayerPrefs.GetString("token", "");
		user.SetPayload(token);
        user.SetLocation(new Location(40.752710, -73.979307));
        
        string json = JsonUtility.ToJson(user);
        byte[] data = Encoding.UTF8.GetBytes(json);
        try {       
            client.Send(data, data.Length, serverAddress);
        } catch(Exception err) {
            print(err.ToString());
        }
    }

    private void onDataDiagramReceive() {
		IPEndPoint remoteEndPoin = new IPEndPoint(IPAddress.Any, 0);
		string jsonPacket = "";
        while(true) {
            try {
                byte[] data = client.Receive(ref remoteEndPoin);
				if(data != null && data.Length > 0) {
                    jsonPacket = Encoding.UTF8.GetString(data);
					lastDataPacket = JsonUtility.FromJson<DataPacket>(jsonPacket);
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
