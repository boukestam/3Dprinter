using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///     The filament manager manages the spawning of filement meshes in the scene.
/// </summary>
public class FilamentManager : MonoBehaviour {

    /// <param name="EmptyFilament">This object is used to add an empty mesh object to the scene. This object is then used to combine filament into one mesh.</param>
    private GameObject EmptyFilament;
    /// <param name="FilamentMesh">This object is used to make a copy from the filament mesh resource.</param>
    private Mesh FilamentMesh;

    /// <param name="VerticesPerMesh">Amount of vertices in the filament mesh.</param>
    private int VerticesPerMesh;
    /// <param name="MaxVerticesPerMesh">Maximum number of vertices allowed in a Unity mesh (16 bit).</param>
    private int MaxVerticesPerMesh = 65536;
    /// <param name="FilamentsPerMesh">The amount of filaments that fit in a single mesh (MaxVerticesPerMesh / VerticesPerMesh).</param>
    private int FilamentsPerMesh;

    /// <param name="Objects">The objects containing the combined meshes.</param>
    private List<GameObject> Objects;

    /// <param name="TempObjects">List of temporary gameobjects.</param>
    private GameObject[] TempObjects;
    /// <param name="MaxTempObjects">Maximum number of temporary gameobjects until they are combined.</param>
    private int MaxTempObjects = 2500;
    /// <param name="TempIndex">Index to store temporary object.</param>
    private int TempIndex = 0;

    /// <param name="FilamentCount">Amount of filaments created.</param>
    private int FilamentCount = 0;

    /// <param name="FilamentCount">Location of the previous filament, when a new object is added at the same location, it will extend the old.</param>
    private Vector3 PreviousFrom;

    /// <param name="TotallyRandom">Random vector to unset PreviousFrom (Vector3 can't be set to null).</param>
    private static Vector3 TotallyRandom = new Vector3(39642f, 43785f, 325346f);

    private const string GroupedFilamentName = "GroupedFilament";

    void Start() {
        // Load the filament prefabs
        var filament = (GameObject)Resources.Load("Filament");
        EmptyFilament = (GameObject)Resources.Load("EmptyFilament");
        FilamentMesh = filament.GetComponent<MeshFilter>().sharedMesh;

        // Calculate the maximum amount of filaments per mesh
        VerticesPerMesh = FilamentMesh.vertices.Length;
        FilamentsPerMesh = MaxVerticesPerMesh / VerticesPerMesh;

        // Instantiate all temporary objects
        TempObjects = new GameObject[MaxTempObjects];

        for (int i = 0; i < TempObjects.Length; i++) {
            TempObjects[i] = Instantiate(filament, transform);

            TempObjects[i].GetComponent<MeshRenderer>().enabled = false;
        }

        // Instantiate the first gameobject to combine in
        Objects = new List<GameObject>();
        var clone = Instantiate(EmptyFilament, transform);
        clone.name = GroupedFilamentName;
        Objects.Add(clone);
    }

    /// <summary>
    /// Adds a piece of filament to the simulation
    /// </summary>
    /// <param name="from">Start position</param>
    /// <param name="to">End position</param>
    /// <param name="thickness">Radius of the thickness of the filament</param>
    public void AddFilament(Vector3 from, Vector3 to, float thickness) {
        // Calculate the transform of the piece of filament
        var distance = to - from;
        var position = from + (distance / 2);
        var rotation = Quaternion.LookRotation(distance);
        var scale = new Vector3(thickness, thickness, distance.magnitude);

        // If the previous filament started at the same location, change it to the new size instead of making a new piece
        if (PreviousFrom == from && TempIndex > 0) {
            var t = TempObjects[TempIndex - 1];

            t.transform.localPosition = position;
            t.transform.localRotation = rotation;
            t.transform.localScale = scale;

            return;
        }

        // If the max number of filaments in a single mesh has been reached, combine everything and instantiate a new object
        if (FilamentCount != 0 && FilamentCount % FilamentsPerMesh == 0) {
            CombineTemp();
            var clone = Instantiate(EmptyFilament, transform);
            clone.name = GroupedFilamentName;
            Objects.Add(clone);
        }

        // Enable the current temp object and set its transform
        var tempObject = TempObjects[TempIndex];
        tempObject.transform.localPosition = position;
        tempObject.transform.localRotation = rotation;
        tempObject.transform.localScale = scale;

        tempObject.GetComponent<MeshRenderer>().enabled = true;

        TempIndex++;

        // If the maximum number of temp objects has been reached, combine all into the current object
        if (TempIndex >= MaxTempObjects) {
            CombineTemp();
            PreviousFrom = TotallyRandom;
        } else {
            PreviousFrom = from;
        }

        FilamentCount++;
    }

    /// <summary>
    /// Combines all temporary objects into a single mesh
    /// </summary>
    private void CombineTemp() {
        var combine = new List<CombineInstance>();

        // Temporarely reset position of parent to combine meshes
        var old = transform.position;
        transform.position = new Vector3(0, 0, 0);

        // Get last object to combine with
        var obj = Objects[Objects.Count - 1];
        var objMeshFilter = obj.GetComponent<MeshFilter>();

        // Add the last object to the combine list
        var objCombine = new CombineInstance();
        objCombine.mesh = objMeshFilter.mesh;
        objCombine.transform = obj.transform.localToWorldMatrix;
        combine.Add(objCombine);

        // Add all temp objects to the combine list
        for(int i = 0; i < TempIndex; i++) {
            var tempCombine = new CombineInstance();
            tempCombine.mesh = FilamentMesh;
            tempCombine.transform = TempObjects[i].transform.localToWorldMatrix;

            combine.Add(tempCombine);
        }

        // Combine all meshes into a single mesh
        var mesh = new Mesh();
        mesh.CombineMeshes(combine.ToArray());

        // Destroy the old mesh to free memory and update the object to use the newly combined mesh
        Destroy(objMeshFilter.mesh);
        objMeshFilter.mesh = mesh;

        // Reset the parent position
        transform.position = old;

        // Hide all temp objects
        foreach (var temp in TempObjects) {
            temp.GetComponent<MeshRenderer>().enabled = false;
        }

        TempIndex = 0;
    }

}
