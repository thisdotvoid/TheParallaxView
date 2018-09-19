using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FaceUpdatedMessage
{
    public string type = "FaceUpdated";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public SerializableDictionary<string, float> blendShapes;

    public FaceUpdatedMessage(Vector3 position, Quaternion rotation, Dictionary<string, float> blendShapes) {
        this.position = position;
        this.rotation = rotation;
        this.blendShapes = new SerializableDictionary<string, float>(blendShapes);
    }
}