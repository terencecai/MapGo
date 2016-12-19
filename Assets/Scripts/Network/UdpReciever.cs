using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UniRx;

[Serializable]
public class UdpState : System.IEquatable<UdpState>
{
    public IPEndPoint EndPoint {get; set;}
    public string UdpMsg {get; set;}

    public UdpState(IPEndPoint ep, string udpMsg)
    {
        this.EndPoint = ep;
        this.UdpMsg = udpMsg;
    }

    public override int GetHashCode() {
        return EndPoint.Address.GetHashCode();
    }

    public bool Equals(UdpState s)
    {
        if (s == null) {
            return false;
        }

        return EndPoint.Address.Equals(s.EndPoint.Address);
    }
}

public class UdpReciever : MonoBehaviour {
    private const int listenPort = 10000;
    private static UdpClient client;
    private bool isAppQuitting;
    public IObservable<UdpState> _udpSequence;

    void Awake() {
        _udpSequence = Observable.Create<UdpState>(observer => {
            try { client = new UdpClient(listenPort); }
            catch (SocketException se) { observer.OnError(se); }

            IPEndPoint remoteEP = null;
            while (!isAppQuitting)
            {
                try {
                    remoteEP = null;
                    var receiveMsg = System.Text.Encoding.UTF8.GetString(client.Receive(ref remoteEP));
                    observer.OnNext(new UdpState(remoteEP, receiveMsg));
                } catch (SocketException) {
                    Debug.Log("Recieve timeout");
                }
            }
            observer.OnCompleted();

            return null;
        })
        .SubscribeOn(Scheduler.ThreadPool)
        .ObserveOn(Scheduler.MainThread)
        .Publish()
        .RefCount();
    }

    void OnApplicationQuit() {
        isAppQuitting = true;
        client.Client.Blocking = false;
    }
}