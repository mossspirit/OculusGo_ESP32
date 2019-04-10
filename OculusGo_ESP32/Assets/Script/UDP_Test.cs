using UnityEngine;
using UniRx;
using UdpReceiverUniRx;

public class UDP_Test : MonoBehaviour
{
    public UdpReceiverRx _udpReceiverRx;
    private IObservable<UdpState> myUdpSequence;

    void Start()
    {
        myUdpSequence = _udpReceiverRx._udpSequence;

        myUdpSequence.ObserveOnMainThread().Subscribe(x => {
            print(x.UdpMsg);
        }).AddTo(this);
    }
}