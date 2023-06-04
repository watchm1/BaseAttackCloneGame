 using System.Collections.Generic;
 using UnityEngine;

 namespace MeshManipulation
 {
     public class PartMesh
     {
         private List<Vector3> _verticies = new List<Vector3>();
         private List<Vector3> _normals = new List<Vector3>();
         private List<List<int>> _triangles = new List<List<int>>();
         private readonly List<Vector2> _uVs = new List<Vector2>();
         public Vector3[] Vertices;
         public Vector3[] Normals;
         public int[][] Triangles;
         public Vector2[] UV;
         public GameObject GameObject;
         public Bounds Bounds = new Bounds();

         public PartMesh()
         {

         }

         public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
         {
             if (_triangles.Count - 1 < submesh)
                 _triangles.Add(new List<int>());

             _triangles[submesh].Add(_verticies.Count);
             _verticies.Add(vert1);
             _triangles[submesh].Add(_verticies.Count);
             _verticies.Add(vert2);
             _triangles[submesh].Add(_verticies.Count);
             _verticies.Add(vert3);
             _normals.Add(normal1);
             _normals.Add(normal2);
             _normals.Add(normal3);
             _uVs.Add(uv1);
             _uVs.Add(uv2);
             _uVs.Add(uv3);

             Bounds.min = Vector3.Min(Bounds.min, vert1);
             Bounds.min = Vector3.Min(Bounds.min, vert2);
             Bounds.min = Vector3.Min(Bounds.min, vert3);
             Bounds.max = Vector3.Min(Bounds.max, vert1);
             Bounds.max = Vector3.Min(Bounds.max, vert2);
             Bounds.max = Vector3.Min(Bounds.max, vert3);
         }

         public void FillArrays()
         {
             Vertices = _verticies.ToArray();
             Normals = _normals.ToArray();
             UV = _uVs.ToArray();
             Triangles = new int[_triangles.Count][];
             for (var i = 0; i < _triangles.Count; i++)
                 Triangles[i] = _triangles[i].ToArray();
         }

         public void MakeGameobject(MeshDestroy original, float delayForDeadTime)
         {
             GameObject = new GameObject(original.name);
             var transform = original.transform;
             GameObject.transform.position = transform.position;
             GameObject.transform.rotation = transform.rotation;
             GameObject.transform.localScale = transform.localScale;

             var mesh = new Mesh();
             mesh.name = original.GetComponent<MeshFilter>().mesh.name;

             mesh.vertices = Vertices;
             mesh.normals = Normals;
             mesh.uv = UV;
             for(var i = 0; i < Triangles.Length; i++)
                 mesh.SetTriangles(Triangles[i], i, true);
             Bounds = mesh.bounds;
            
             var renderer = GameObject.AddComponent<MeshRenderer>();
             renderer.materials = original.GetComponent<MeshRenderer>().materials;

             var filter = GameObject.AddComponent<MeshFilter>();
             filter.mesh = mesh;

             var collider = GameObject.AddComponent<MeshCollider>();
             collider.convex = true;

             var rigidbody = GameObject.AddComponent<Rigidbody>();
             rigidbody.isKinematic = true;
             var meshDestroy = GameObject.AddComponent<MeshDestroy>();
             meshDestroy.cutCascades = original.cutCascades;
             meshDestroy.explodeForce = original.explodeForce;
             var triggerScript = GameObject.AddComponent<PieceTrigger>();
             triggerScript.lifeTime = delayForDeadTime;
         }

     }
 }