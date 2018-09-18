using UnityEngine;

[System.Serializable]
public class FaceUpdated
{
    public string type = "FaceUpdated";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public SerializableDictionary<string, float> blendShapes;

    public FaceUpdated(Vector3 position, Quaternion rotation) {
        this.position = position;
        this.rotation = rotation;
    }
}