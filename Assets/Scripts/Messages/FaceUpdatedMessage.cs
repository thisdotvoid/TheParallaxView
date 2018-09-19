using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FaceUpdatedMessage
{
    public string type = "FaceUpdated";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public DictionaryOfStringAndFloat blendShapes;

    public FaceUpdatedMessage(Vector3 position, Quaternion rotation, DictionaryOfStringAndFloat blendShapes) {
        this.position = position;
        this.rotation = rotation;
        this.blendShapes = blendShapes;
    }
}