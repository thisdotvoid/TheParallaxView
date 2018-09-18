using UnityEngine;

[System.Serializable]
public class FaceAdded
{
    public string type = "FaceAdded";
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public SerializableDictionary<string, float> blendShapes;

    public FaceAdded(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}