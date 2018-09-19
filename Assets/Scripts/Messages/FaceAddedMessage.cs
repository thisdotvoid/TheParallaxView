using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FaceAddedMessage
{
    public string type = "FaceAdded";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public DictionaryOfStringAndFloat blendShapes;

    public FaceAddedMessage(Vector3 position, Quaternion rotation, DictionaryOfStringAndFloat blendShapes)
    {
        this.position = position;
        this.rotation = rotation;
        this.blendShapes = blendShapes;
    }
}