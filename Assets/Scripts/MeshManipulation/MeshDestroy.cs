using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace MeshManipulation
{
    public class MeshDestroy : MonoBehaviour
    {
        private bool _edgeSet = false;
        private Vector3 _edgeVertex = Vector3.zero;
        private Vector2 _edgeUV = Vector2.zero;
        private Plane _edgePlane = new Plane();
        
        
        private Mesh _perfectMesh;
        private List<PartMesh> _partsOfMeshes;
        private List<PartMesh> _subParts;
        private PartMesh _mainPart;

        [SerializeField] private float pieceLifeTime;
        [FormerlySerializedAs("CutCascades")] public int cutCascades = 1;
        [FormerlySerializedAs("ExplodeForce")] public float explodeForce = 0;

        // Start is called before the first frame update
        private void Start()
        {
            //SetMeshesConfigurationForOriginalObject();
            //DivideMeshIntoPieces();
        }

        // Update is called once per frame
        private void Update()
        {
            
        }

        private void SetMeshesConfigurationForOriginalObject()
        {
            this._perfectMesh = GetComponent<MeshFilter>().mesh;
            this._perfectMesh.RecalculateBounds();
            this._partsOfMeshes = new List<PartMesh>();
            this._subParts = new List<PartMesh>();
            this._mainPart = new PartMesh()
            {
                UV = this._perfectMesh.uv,
                Vertices = this._perfectMesh.vertices,
                Normals = this._perfectMesh.normals,
                Triangles = new int[this._perfectMesh.subMeshCount][],
                Bounds = this._perfectMesh.bounds
            };
            for (var i = 0; i <_perfectMesh.subMeshCount; i++)
                _mainPart.Triangles[i] = _perfectMesh.GetTriangles(i);
            _partsOfMeshes.Add(_mainPart);
        }

        private void DivideMeshIntoPieces()
        {
            for (var c = 0; c < cutCascades; c++)
            {
                for (var i = 0; i < _partsOfMeshes.Count; i++)
                {
                    var bounds = _partsOfMeshes[i].Bounds;
                    bounds.Expand(0.5f);
                    var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                        UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                        UnityEngine.Random.Range(bounds.min.z, bounds.max.z)));
                    
                    _subParts.Add(GenerateMesh(_partsOfMeshes[i], plane, true));
                    _subParts.Add(GenerateMesh(_partsOfMeshes[i], plane, false));
                }

                _partsOfMeshes = new List<PartMesh>(_subParts);
                _subParts.Clear();
            }

            for (var i = 0; i < _partsOfMeshes.Count; i++)
            {
                _partsOfMeshes[i].MakeGameobject(this, pieceLifeTime);
                AddForceToPieces(_partsOfMeshes[i]);
                
            }
        }

        private void AddForceToPieces(PartMesh piece)
        {
            var rb = piece.GameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForceAtPosition(piece.Bounds.center * explodeForce, transform.position);
        }

        public void DestroyMesh()
        {
            // var originalMesh = GetComponent<MeshFilter>().mesh;
            // originalMesh.RecalculateBounds();
            // var parts = new List<PartMesh>();
            // var subParts = new List<PartMesh>();
            //
            // var mainPart = new PartMesh()
            // {
            //     UV = originalMesh.uv,
            //     Vertices = originalMesh.vertices,
            //     Normals = originalMesh.normals,
            //     Triangles = new int[originalMesh.subMeshCount][],
            //     Bounds = originalMesh.bounds
            // };
            // for (int i = 0; i < originalMesh.subMeshCount; i++)
            //     mainPart.Triangles[i] = originalMesh.GetTriangles(i);
            //
            // parts.Add(mainPart);
            //
            // for (var c = 0; c < cutCascades; c++)
            // {
            //     for (var i = 0; i < parts.Count; i++)
            //     {
            //         var bounds = parts[i].Bounds;
            //         bounds.Expand(0.5f);
            //
            //         var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            //             UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            //             UnityEngine.Random.Range(bounds.min.z, bounds.max.z)));
            //
            //
            //         subParts.Add(GenerateMesh(parts[i], plane, true));
            //         subParts.Add(GenerateMesh(parts[i], plane, false));
            //     }
            //     parts = new List<PartMesh>(subParts);
            //     subParts.Clear();
            // }
            //
            // for (var i = 0; i < parts.Count; i++)
            // {
            //     parts[i].MakeGameobject(this);
            //     parts[i].GameObject.GetComponent<Rigidbody>().AddForceAtPosition(parts[i].Bounds.center * explodeForce, transform.position);
            // }
            SetMeshesConfigurationForOriginalObject();
            DivideMeshIntoPieces();
            Destroy(gameObject);
        }

        private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
        {
            var partMesh = new PartMesh() { };
            var ray1 = new Ray();
            var ray2 = new Ray();


            for (var i = 0; i < original.Triangles.Length; i++)
            {
                var triangles = original.Triangles[i];
                _edgeSet = false;

                for (var j = 0; j < triangles.Length; j = j + 3)
                {
                    var sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                    var sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                    var sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                    var sideCount = (sideA ? 1 : 0) +
                                    (sideB ? 1 : 0) +
                                    (sideC ? 1 : 0);
                    if (sideCount == 0)
                    {
                        continue;
                    }
                    if (sideCount == 3)
                    {
                        partMesh.AddTriangle(i,
                            original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                            original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                            original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                        continue;
                    }

                    //cut points
                    var singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                    ray1.origin = original.Vertices[triangles[j + singleIndex]];
                    var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                    ray1.direction = dir1;
                    plane.Raycast(ray1, out var enter1);
                    var lerp1 = enter1 / dir1.magnitude;

                    ray2.origin = original.Vertices[triangles[j + singleIndex]];
                    var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                    ray2.direction = dir2;
                    plane.Raycast(ray2, out var enter2);
                    var lerp2 = enter2 / dir2.magnitude;

                    //first vertex = ancor
                    AddEdge(i,
                        partMesh,
                        left ? plane.normal * -1f : plane.normal,
                        ray1.origin + ray1.direction.normalized * enter1,
                        ray2.origin + ray2.direction.normalized * enter2,
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                    if (sideCount == 1)
                    {
                        partMesh.AddTriangle(i,
                            original.Vertices[triangles[j + singleIndex]],
                            //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            ray1.origin + ray1.direction.normalized * enter1,
                            ray2.origin + ray2.direction.normalized * enter2,
                            original.Normals[triangles[j + singleIndex]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            original.UV[triangles[j + singleIndex]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                    
                        continue;
                    }

                    if (sideCount == 2)
                    {
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normals[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]]);
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            ray2.origin + ray2.direction.normalized * enter2,
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                        continue;
                    }


                }
            }

            partMesh.FillArrays();

            return partMesh;
        }

        private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
        {
            if (!_edgeSet)
            {
                _edgeSet = true;
                _edgeVertex = vertex1;
                _edgeUV = uv1;
            }
            else
            {
                _edgePlane.Set3Points(_edgeVertex, vertex1, vertex2);

                partMesh.AddTriangle(subMesh,
                    _edgeVertex,
                    _edgePlane.GetSide(_edgeVertex + normal) ? vertex1 : vertex2,
                    _edgePlane.GetSide(_edgeVertex + normal) ? vertex2 : vertex1,
                    normal,
                    normal,
                    normal,
                    _edgeUV,
                    uv1,
                    uv2);
            }
        }

       
    }
}