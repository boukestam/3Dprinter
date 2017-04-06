using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentManager : MonoBehaviour {

    private GameObject Filament;
    private GameObject EmptyFilament;
    private Mesh FilamentMesh;

    private int VerticesPerMesh;                    // Amount of vertices in the filament mesh
    private int MaxVerticesPerMesh = 65536;         // Maximum number of vertices allowed in a Unity mesh (16 bit)
    private int FilamentsPerMesh;                   // The amount of filaments that fit in a single mesh (MaxVerticesPerMesh / VerticesPerMesh)

    private List<GameObject> Objects;               // The objects containing the combined meshes

    private GameObject[] TempObjects;               // List of temporary gameobjects
    private int MaxTempObjects = 750;              // Maximum number of temporary gameobjects until they are combined
    private int TempIndex = 0;                      // Index to store temporary object

    private int FilamentCount = 0;                  // Amount of filaments created

    // Location of the previous filament, when a new object is 
    // added at the same location, it will extend the old
    private Vector3 PreviousFrom;
    
    // Random vector to unset previous from (Vector3 can't be set to null)
    private static Vector3 TotallyRandom = new Vector3(39642f, 43785f, 325346f);

    /// <summary>
    /// Combines all temporary objects into a single mesh
    /// </summary>
    private void CombineTemp() {
        List<CombineInstance> combine = new List<CombineInstance>();

        // Temporarely reset position of parent to combine meshes
        Vector3 old = transform.position;
        transform.position = new Vector3(0, 0, 0);

        // Get last object to combine with
        GameObject obj = Objects[Objects.Count - 1];
        MeshFilter objMeshFilter = obj.GetComponent<MeshFilter>();

        // Add the last object to the combine list
        CombineInstance objCombine = new CombineInstance();
        objCombine.mesh = objMeshFilter.mesh;
        objCombine.transform = obj.transform.localToWorldMatrix;
        combine.Add(objCombine);

        // Add all temp objects to the combine list
        for(int i = 0; i < TempIndex; i++) {
            CombineInstance tempCombine = new CombineInstance();
            tempCombine.mesh = FilamentMesh;
            tempCombine.transform = TempObjects[i].transform.localToWorldMatrix;

            combine.Add(tempCombine);
        }

        // Combine all meshes into a single mesh
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine.ToArray());

        // Destroy the old mesh to free memory and update the object to use the newly combined mesh
        Destroy(objMeshFilter.mesh);
        objMeshFilter.mesh = mesh;

        // Reset the parent position
        transform.position = old;

        // Hide all temp objects
        foreach (GameObject temp in TempObjects) {
            temp.SetActive(false);
        }

        TempIndex = 0;
    }

    /// <summary>
    /// Adds a piece of filament to the simulation
    /// </summary>
    /// <param name="from">Start position</param>
    /// <param name="to">End position</param>
    /// <param name="thickness">Radius of the thickness of the filament</param>
    public void AddFilament(Vector3 from, Vector3 to, float thickness) {
        // Calculate the transform of the piece of filament
        Vector3 distance = to - from;
        Vector3 position = from + (distance / 2);
        Quaternion rotation = Quaternion.LookRotation(distance);
        Vector3 scale = new Vector3(thickness, thickness, distance.magnitude);

        // If the previous filament started at the same location, change it to the new size instead of making a new piece
        if (PreviousFrom == from && TempIndex > 0) {
            GameObject t = TempObjects[TempIndex - 1];

            t.transform.localPosition = position;
            t.transform.localRotation = rotation;
            t.transform.localScale = scale;

            return;
        }

        // If the max number of filaments in a single mesh has been reached, combine everything and instantiate a new object
        if (FilamentCount != 0 && FilamentCount % FilamentsPerMesh == 0) {
            CombineTemp();
            GameObject clone = Instantiate(EmptyFilament, transform);
            Objects.Add(clone);
        }

        // Enable the current temp object and set its transform
        GameObject tempObject = TempObjects[TempIndex];
        tempObject.transform.localPosition = position;
        tempObject.transform.localRotation = rotation;
        tempObject.transform.localScale = scale;
        tempObject.SetActive(true);

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
    
	void Start () {
        // Load the filament prefabs
        Filament = (GameObject)Resources.Load("Filament");
        EmptyFilament = (GameObject)Resources.Load("EmptyFilament");
        FilamentMesh = Filament.GetComponent<MeshFilter>().sharedMesh;

        // Calculate the maximum amount of filaments per mesh
        VerticesPerMesh = FilamentMesh.vertices.Length;
        FilamentsPerMesh = MaxVerticesPerMesh / VerticesPerMesh;

        // Instantiate the first gameobject to combine in
        Objects = new List<GameObject>();
        Objects.Add(Instantiate(EmptyFilament, transform));

        // Instantiate all temporary objects
        TempObjects = new GameObject[MaxTempObjects];

        for(int i = 0; i < TempObjects.Length; i++) {
            TempObjects[i] = Instantiate(Filament, transform);
            TempObjects[i].SetActive(false);
        }
    }
}
