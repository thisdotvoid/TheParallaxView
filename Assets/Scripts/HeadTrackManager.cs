using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.iOS;

// script for setting up ARKit for 3D head tracking purposes

public class HeadTrackManager : MonoBehaviour
{

	[SerializeField]
	private GameObject headCenter;
	private SocketClient socketClient;

	//public Light keylight;
	public CameraManager camManager;

	private UnityARSessionNativeInterface m_session;

	Dictionary<string, float> currentBlendShapes;

	public float leftEyeClosed = 0f;
	public float rightEyeClosed = 0f;

	public enum OpenEye { Left, Right };
	[System.NonSerialized]
	public OpenEye openEye = OpenEye.Right;
	bool autoEye = false;

	public string eyeInfoText; // little status, which eye is being tracked, auto or not
	public float IPD = 64f; // inter pupil distance (mm)
	public float EyeHeight = 32f; // eye height from head anchor (mm)

	public string ARError;

	public void SetIPD(float value)
	{
		IPD = value;
	}

	public void SetEyeHeight(float value)
	{
		EyeHeight = value;
	}

	// Use this for initialization
	public void Start()
	{
		ARError = null;
		Application.targetFrameRate = 60;

		if (UnityHelpers.IsIPhoneX()) {
			// On IPhone
			socketClient = new SocketClient("10.10.20.41");

			// first try to get camera acess
			//yield return RequestCamera ();

			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

			UnityARSessionNativeInterface.ARSessionFailedEvent += CatchARSessionFailed;
			ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
			//config.alignment = UnityARAlignment.UnityARAlignmentGravity; // using gravity alignment enables orientation (3DOF) tracking of device camera. we don't need it
			config.alignment = UnityARAlignment.UnityARAlignmentCamera;

			config.enableLightEstimation = true;

			if (config.IsSupported) {
				m_session.RunWithConfig(config);

				UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
				UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
				UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
				//UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdate; //can't get the light direction estimate to work for some reason, it freezes the app
			} else {
				Debug.Log("ARKitFaceTrackingConfiguration not supported");
			}
		} else {
			// On Desktop
			var mainThread = SynchronizationContext.Current;

			socketClient = new SocketClient();
			socketClient.OnReceive((data) => {
				Debug.Log(data);
				SocketMessage socketMessage = JsonUtility.FromJson<SocketMessage>(data);

				mainThread.Post((s) => {
					switch (socketMessage.type) {
						case "FaceAdded":
							FaceAddedAction(JsonUtility.FromJson<FaceAddedMessage>(data));
							break;
						case "FaceUpdated":
							FaceUpdatedAction(JsonUtility.FromJson<FaceUpdatedMessage>(data));
							break;
						case "FaceRemoved":
							FaceRemovedAction(JsonUtility.FromJson<FaceRemovedMessage>(data));
							break;
						default:
							Debug.Log("Unknown message type: " + socketMessage.type);
							break;
					}
				}, null);
			});

			Debug.Log("Waiting for data on socket");
		}

	}

	void CatchARSessionFailed(string error)
	{
		//Debug.Log ("AR session failed. Error: " + error);
		ARError = error;
	}

	/* // this doesn't help at all
    IEnumerator RequestCamera() {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
            Debug.Log ("Camera granted");
        } else {
            Debug.Log ("Camera denied");
        }
    }
    */

    void UpdateHeadPosition(Vector3 pos, Quaternion rot) {
		if (camManager.DeviceCamUsed) {
			headCenter.transform.position = pos; // in device cam viewing mode, don't invert on x because this view is mirrored
            if (null != rot) {
				headCenter.transform.rotation = rot;
            }
		} else {
			// invert on x because ARfaceAnchors are inverted on x (to mirror in display)
			headCenter.transform.position = new Vector3(-pos.x, pos.y, pos.z);
            if (null != rot) {
				headCenter.transform.rotation = new Quaternion(-rot.x, rot.y, rot.z, -rot.w);
            }
		}
	}

	void FaceAdded(ARFaceAnchor anchorData)
	{
		Vector3 pos = UnityARMatrixOps.GetPosition(anchorData.transform);
		Quaternion rot = UnityARMatrixOps.GetRotation(anchorData.transform);
		FaceAddedMessage message = new FaceAddedMessage(pos, rot, new DictionaryOfStringAndFloat(anchorData.blendShapes));

		socketClient.Send(JsonUtility.ToJson(message));
		FaceAddedAction(message);
	}

	void FaceAddedAction(FaceAddedMessage message)
	{
        UpdateHeadPosition(message.position, message.rotation);
		headCenter.SetActive(true);
		//currentBlendShapes = message.blendShapes;
	}

	void FaceUpdated(ARFaceAnchor anchorData)
	{
		Vector3 pos = UnityARMatrixOps.GetPosition(anchorData.transform);
		Quaternion rot = UnityARMatrixOps.GetRotation(anchorData.transform);
		FaceUpdatedMessage message = new FaceUpdatedMessage(pos, rot, new DictionaryOfStringAndFloat(anchorData.blendShapes));

		socketClient.Send(JsonUtility.ToJson(message));
		FaceUpdatedAction(message);
	}

	void FaceUpdatedAction(FaceUpdatedMessage message)
	{
		UpdateHeadPosition(message.position, message.rotation);
		//currentBlendShapes = message.blendShapes;

		if (autoEye && null != currentBlendShapes) {

			if (currentBlendShapes.ContainsKey("eyeBlink_L")) { // of course, eyeBlink_L refers to the RIGHT eye! (mirrored geometry)
				rightEyeClosed = currentBlendShapes["eyeBlink_L"];
			}

			if (currentBlendShapes.ContainsKey("eyeBlink_R")) {
				leftEyeClosed = currentBlendShapes["eyeBlink_R"];
			}

			//string str = string.Format ("L={0:#.##} R={1:#.##}", leftEyeClosed, rightEyeClosed);
			//eyeInfoText = str;

			// these values seem to be in the 0.2 .. 0.7 range..
			// but sometimes, when viewing the phone low in the visual field, they get very high even while open (eyelids almost close)
			// we'll use a difference metric and if exceeded we select the most open eye

			if (Mathf.Abs(rightEyeClosed - leftEyeClosed) > 0.2f) {
				if (rightEyeClosed > leftEyeClosed)
					openEye = OpenEye.Left;
				else
					openEye = OpenEye.Right;
			}

			/* // old method
            if (rightEyeClosed > 0.5 && leftEyeClosed < 0.5)
                openEye = OpenEye.Left;

            if (rightEyeClosed < 0.5 && leftEyeClosed > 0.5)
                openEye = OpenEye.Right;*/
		}

		string str;
		if (openEye == OpenEye.Left)
			str = "Left Eye";
		else
			str = "Right Eye";

		if (autoEye)
			eyeInfoText = "Auto: " + str;
		else
			eyeInfoText = str;
	}

	void FaceRemoved(ARFaceAnchor anchorData)
	{
		FaceRemovedMessage message = new FaceRemovedMessage();

		socketClient.Send(JsonUtility.ToJson(message));
		FaceRemovedAction(message);
	}

	void FaceRemovedAction(FaceRemovedMessage message)
	{
		headCenter.SetActive(false);
		string str = "Lost Eye Tracking";
		eyeInfoText = str;
	}

	void FrameUpdate(UnityARCamera cam)
	{
		//can't get the light direction estimate to work for some reason, it freezes the app
		//keylight.transform.rotation = Quaternion.FromToRotation(Vector3.back, cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably incorrect way to do it
		//keylight.transform.rotation = Quaternion.LookRotation(cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably correct way to do it
	}


	// Update is called once per frame
	void Update()
	{
	}

	void OnDestroy()
	{
        socketClient.Close();
	}

	public void SetLeftEye()
	{
		autoEye = false;
		openEye = OpenEye.Left;
	}

	public void SetRightEye()
	{
		autoEye = false;
		openEye = OpenEye.Right;
	}

	public void SetAutoEye()
	{
		autoEye = true;
	}
}
