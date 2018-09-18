using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketHead : MonoBehaviour {

	[SerializeField]
    private GameObject headCenter;

	SocketClient socketClient;

	// Use this for initialization
	void Start () {
		socketClient = SocketClient.Get();
		socketClient.OnReceive((data) => {
			String[] message = MessageParser.Parse(data);
			Debug.Log(data);
		});
	}

	// Update is called once per frame
	void Update () {

	}
}
