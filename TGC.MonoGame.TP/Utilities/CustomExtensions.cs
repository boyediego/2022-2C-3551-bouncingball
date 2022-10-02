using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using NumericVector3 = System.Numerics.Vector3;

namespace TGC.MonoGame.TP.Utilities
{
    public static class CustomExtensions
    {
        public static float Abs(this float value)
        {
            return (float)Math.Abs(value);
        }

        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(vector.X.Abs(), vector.Y.Abs(), vector.Z.Abs());
        }

        public static NumericVector3 ToNumericVector3(this Vector3 vector)
        {
            return new NumericVector3(vector.X, vector.Y, vector.Z);
        }

        public static Vector3 PerpendicularClockwiseIn2D(this Vector3 vector2)
        {
            return new Vector3(vector2.Z,0, -vector2.X);
        }

        public static Vector3 PerpendicularCounterClockwiseIn2D(this Vector3 vector2)
        {
            return new Vector3(-vector2.Z, 0, vector2.X);
        }


        public static BoundingSphere GetSphereFrom(this Model model)
        {
            var minPoint = Vector3.One * float.MaxValue;
            var maxPoint = Vector3.One * float.MinValue;

            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            var meshes = model.Meshes;
            for (var index = 0; index < meshes.Count; index++)
            {
                var meshParts = meshes[index].MeshParts;
                for (var subIndex = 0; subIndex < meshParts.Count; subIndex++)
                {
                    var vertexBuffer = meshParts[subIndex].VertexBuffer;
                    var declaration = vertexBuffer.VertexDeclaration;
                    int vertexSize = declaration.VertexStride / sizeof(float);

                    var rawVertexBuffer = new float[vertexBuffer.VertexCount * vertexSize];
                    vertexBuffer.GetData(rawVertexBuffer);

                    for (var vertexIndex = 0; vertexIndex < rawVertexBuffer.Length; vertexIndex += vertexSize)
                    {
                        var transform = transforms[meshes[index].ParentBone.Index];
                        var vertex = new Vector3(rawVertexBuffer[vertexIndex], rawVertexBuffer[vertexIndex + 1], rawVertexBuffer[vertexIndex + 2]);
                        vertex = Vector3.Transform(vertex, transform);
                        minPoint = Vector3.Min(minPoint, vertex);
                        maxPoint = Vector3.Max(maxPoint, vertex);
                    }
                }
            }
            var difference = (maxPoint - minPoint) * 0.5f;
            return new BoundingSphere(difference, difference.Length());
        }

        public static BoundingBox GetBoundingBox(this Model model, Matrix rotationMatrix)
        {
            var minPoint = Vector3.One * float.MaxValue;
            var maxPoint = Vector3.One * float.MinValue;

            var transforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i] *= rotationMatrix;
            }
            model.CopyAbsoluteBoneTransformsTo(transforms);

            var meshes = model.Meshes;
            for (int index = 0; index < meshes.Count; index++)
            {
                var meshParts = meshes[index].MeshParts;
                for (int subIndex = 0; subIndex < meshParts.Count; subIndex++)
                {
                    var vertexBuffer = meshParts[subIndex].VertexBuffer;
                    var declaration = vertexBuffer.VertexDeclaration;
                    var vertexSize = declaration.VertexStride / sizeof(float);

                    var rawVertexBuffer = new float[vertexBuffer.VertexCount * vertexSize];
                    vertexBuffer.GetData(rawVertexBuffer);

                    for (var vertexIndex = 0; vertexIndex < rawVertexBuffer.Length; vertexIndex += vertexSize)
                    {
                        var transform = transforms[meshes[index].ParentBone.Index];
                        var vertex = new Vector3(rawVertexBuffer[vertexIndex], rawVertexBuffer[vertexIndex + 1], rawVertexBuffer[vertexIndex + 2]);
                        vertex = Vector3.Transform(vertex, transform);
                        minPoint = Vector3.Min(minPoint, vertex);
                        maxPoint = Vector3.Max(maxPoint, vertex);
                    }
                }
            }
            return new BoundingBox(minPoint, maxPoint);
        }
    }
}
