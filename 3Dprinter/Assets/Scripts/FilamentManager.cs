using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentManager : MonoBehaviour {

    private GameObject Filament;
    private GameObject EmptyFilament;
    private Mesh FilamentMesh;

    private int VerticesPerMesh;
    private int MaxVerticesPerMesh = 65536;
    private int FilamentsPerMesh;

    private int MaxTempObjects = 1;

    private List<GameObject> Objects;

    private List<GameObject> TempObjects = new List<GameObject>();

    private int FilamentIndex = 0;

    private void CombineTemp() {
        List<CombineInstance> combine = new List<CombineInstance>();

        GameObject obj = Objects[Objects.Count - 1];

        CombineInstance objCombine = new CombineInstance();
        objCombine.mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        objCombine.transform = obj.transform.localToWorldMatrix;

        combine.Add(objCombine);

        foreach(GameObject temp in TempObjects) {
            CombineInstance tempCombine = new CombineInstance();
            tempCombine.mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            tempCombine.transform = temp.transform.localToWorldMatrix;

            combine.Add(tempCombine);
        }

        Destroy(obj.GetComponent<MeshFilter>().mesh);

        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());

        foreach(GameObject temp in TempObjects) {
            Destroy(temp);
        }

        TempObjects.Clear();
    }

    public void AddFilament(Vector3 from, Vector3 to, float thickness) {
        if (FilamentIndex != 0 && FilamentIndex % FilamentsPerMesh == 0) {
            CombineTemp();
            GameObject clone = Instantiate(EmptyFilament, transform);
            Objects.Add(clone);
        }

        Vector3 distance = to - from;

        Vector3 position = from + (distance / 2);
        Quaternion rotation = Quaternion.LookRotation(distance);
        Vector3 scale = new Vector3(thickness, thickness, distance.magnitude);

        GameObject tempObject = Instantiate(Filament, transform);
        tempObject.transform.localPosition = position;
        tempObject.transform.localRotation = rotation;
        tempObject.transform.localScale = scale;

        TempObjects.Add(tempObject);

        if (TempObjects.Count >= MaxTempObjects) {
            CombineTemp();
        }

        FilamentIndex++;
    }
    
	void Start () {
        Filament = (GameObject)Resources.Load("Filament");
        EmptyFilament = (GameObject)Resources.Load("EmptyFilament");
        FilamentMesh = Filament.GetComponent<MeshFilter>().sharedMesh;

        VerticesPerMesh = FilamentMesh.vertices.Length;
        FilamentsPerMesh = MaxVerticesPerMesh / VerticesPerMesh;

        Objects = new List<GameObject>();
        Objects.Add(Instantiate(EmptyFilament, transform));


        // testing
        //pos = Instantiate(new GameObject());
    }






    // Testing
    GameObject pos;
    
    void FixedUpdate () {
        /*
        for (int i = 0; i < 1; i++) {
            Vector3 from = pos.transform.position;
            pos.transform.Translate(new Vector3(0, 0, 3));
            Vector3 to = pos.transform.position;

            AddFilament(from, to, 1f);

            pos.transform.Rotate(new Vector3(0, 2f, 0.002f));
        }
        */
    }
}
