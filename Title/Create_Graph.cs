using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Graph : MonoBehaviour
{
    public float width = 100f;
    public float height = 100f;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0, 0, -10);
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        // Vertices
        Vector3[] verticies = new Vector3[4]
        {
            new Vector3(0,0,0), new Vector3(width, 0, 0), new Vector3(0, height, 0), new Vector3(-width, height, 0)
        };

        // Triangles
        int[] tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 3;
        tri[4] = 2;
        tri[5] = 0;

        // Normals (Only if you want to display object in the game)
        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        // UV (How textures are displayed)
        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(-1, 1);

        // Assign Arrays!
        mesh.vertices = verticies;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;
    }


}    


