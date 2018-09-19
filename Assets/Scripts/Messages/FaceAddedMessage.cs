using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FaceAddedMessage
{
    public string type = "FaceAdded";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public SerializableDictionary<string, float> blendShapes;

    public FaceAddedMessage(Vector3 position, Quaternion rotation, Dictionary<string, float> blendShapes)
    {
        this.position = position;
        this.rotation = rotation;
        this.blendShapes = new SerializableDictionary<string, float>(blendShapes);
    }
}