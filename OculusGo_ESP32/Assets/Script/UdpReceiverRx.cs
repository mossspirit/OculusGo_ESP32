namespace UdpReceiverUniRx
{

    using UnityEngine;
    using System.Net;
    using System.Net.Sockets;
    using UniRx;
    using System.Text;

    public class UdpState : System.IEquatable<UdpState>
    {
        //UDP通信の情報を収める。送受信ともに使える
        public IPEndPoint EndPoint { get; set; }
        public string UdpMsg { get; set; }

        public UdpState(IPEndPoint ep, string udpMsg)
        {
            this.EndPoint = ep;
            this.UdpMsg = udpMsg;
        }
        public override int GetHashCode()
        {
            return EndPoint.Address.GetHashCode();
        }

        public bool Equals(UdpState s)
        {
            if (s == null)
            {
                return false;
            }
            return EndPoint.Address.Equals(s.EndPoint.Address);
        }
    }

    public class UdpReceiverRx : MonoBehaviour
    {
        public string host = "172.20.10.2";
        private const int Port = 3333;
        private static UdpClient myClient;
        private bool isAppQuitting;
        private UdpClient client;
        public IObservable<UdpState> _udpSequence;
        public string msg;

        void Awake()
        {

            client = new UdpClient();
            client.Connect(host, Port);
            _udpSequence = Observable.Create<UdpState>(observer =>
            {
                Debug.Log(string.Format("_udpSequence thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
                try
                {
                    myClient = new UdpClient(Port);
                }
                catch (SocketException ex)
                {
                    observer.OnError(ex);
                }
                IPEndPoint remoteEP = null;
                myClient.EnableBroadcast = true;
                myClient.Client.ReceiveTimeout = 5000;
                while (!isAppQuitting)
                {
                    try
                    {
                        remoteEP = null;
                        var receivedMsg = System.Text.Encoding.ASCII.GetString(myClient.Receive(ref remoteEP));
                        msg = receivedMsg;
                        observer.OnNext(new UdpState(remoteEP, receivedMsg));
                    }
                    catch (SocketException)
                    {
                        Debug.Log("UDP::Receive timeout");
                    }
                }
                observer.OnCompleted();
                return null;
            })
            .SubscribeOn(Scheduler.ThreadPool)
            .Publish()
            .RefCount();
        }
        public void SendMessage(byte num)
        {
            byte[] dgram = { num };
            Debug.Log("" + num);
            client.Send(dgram, dgram.Length);
        }

        void OnApplicationQuit()
        {
            isAppQuitting = true;
            myClient.Client.Blocking = false;
            client.Close();
        }
    }
}