using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    public interface Tramo 
    {
        float ActualWidth { get; }
        float ActualRotation { get; }
        float ActualElevation { get; }
        Vector3 StartPoint { get; }
        Vector3 EndPoint { get; }
        Vector3 Center { get; }
        Tramo Build();
        Tramo SetTranslation(Vector3 translation);
        Tramo SetRotation(float radians);
        Tramo SetWidth(float width);
        Model3D To3DModel();
    }
}
