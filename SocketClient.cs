using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour {

    // Use this for initialization
    public GameObject hero;
    private float xPos = -0.6400146f;
    private float yPos = 1.609985f;
    private float zPos = 5.92f;

    Thread receiveThread;
	UdpClient client;
	public int port;

	//info
	public string lastReceivedUDPPacket = "";
	public string allReceivedUDPPackets = "";

    void Start () {
		init();
	}

	void OnGUI(){
		Rect  rectObj=new Rect (40,10,200,400);
		GUIStyle  style  = new GUIStyle ();
		style .alignment  = TextAnchor.UpperLeft;
    }

	private void init(){
		print ("UPDSend.init()");
		port = 25001;
        print ("Sending to 127.0.0.1 : " + port);

		receiveThread = new Thread (new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start ();
	}

	private void ReceiveData(){
		client = new UdpClient (port);
		while (true) {
			try{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
				byte[] data = client.Receive(ref anyIP);

				string text = Encoding.UTF8.GetString(data);
                string[] str = text.Split(',');
				print (">> " + text);
				lastReceivedUDPPacket=text;
				allReceivedUDPPackets=allReceivedUDPPackets+text;
				xPos = float.Parse(str[0]);
				xPos *= 0.021818f;
                yPos = float.Parse(str[1]);
                yPos *= 0.021818f;
                zPos = float.Parse(str[2]);
                zPos *= 0.021818f;
              
            }
              catch(Exception e){
				print (e.ToString());
			}
		}
	}

	public string getLatestUDPPacket(){
		allReceivedUDPPackets = "";
		return lastReceivedUDPPacket;
	}
	
	// Update is called once per frame
	void Update () {
        hero.transform.position = new Vector3(xPos-6.5f, -yPos+8.5f, zPos-2.2f);
        //hero.transform.position = new Vector3(xPos - 6.5f, -yPos + 8.5f, zPos+4.2f);
    }

	void OnApplicationQuit(){
		if (receiveThread != null) {
			receiveThread.Abort();
			Debug.Log(receiveThread.IsAlive); //must be false
		}
	}
}
