using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentManager : MonoBehaviour {

    private GameObject Filament;
    private Mesh FilamentMesh;

    private int VerticesPerMesh;
    private int MaxVerticesPerMesh;
    private int FilamentsPerMesh;

    private List<GameObject> Objects;
    private List<CombineInstance> Combine;

    private int FilamentIndex = 0;

    public void AddFilament(Vector3 from, Vector3 to, float thickness) {
        if (FilamentIndex % FilamentsPerMesh == 0) {
            Objects.Add(Instantiate(Filament));
            Combine = new List<CombineInstance>();
        }

        GameObject obj = Objects[Objects.Count - 1];

        Vector3 distance = to - from;

        Matrix4x4 trs = Matrix4x4.TRS(
            from + (distance / 2),
            Quaternion.LookRotation(distance),
            new Vector3(thickness, thickness, distance.magnitude)
        );

        CombineInstance c = new CombineInstance();
        c.mesh = FilamentMesh;
        c.transform = trs;

        Combine.Add(c);

        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh.CombineMeshes(Combine.ToArray());

        FilamentIndex++;
    }
    
	void Start () {
        Filament = (GameObject)Resources.Load("Filament");
        FilamentMesh = Filament.GetComponent<MeshFilter>().sharedMesh;

        VerticesPerMesh = FilamentMesh.vertices.Length;
        MaxVerticesPerMesh = 65536;
        FilamentsPerMesh = MaxVerticesPerMesh / VerticesPerMesh;

        Objects = new List<GameObject>();
        Combine = new List<CombineInstance>();


        // testing
        //pos = Instantiate(new GameObject());
    }






    // Testing
    GameObject pos;
    
    void Update () {
        /*
        Vector3 from = pos.transform.position;
        pos.transform.Translate(new Vector3(0, 0, 3));
        Vector3 to = pos.transform.position;

        AddFilament(from, to, 1f);

        pos.transform.Rotate(new Vector3(0, 2f, 0.002f));
        */
    }
}
