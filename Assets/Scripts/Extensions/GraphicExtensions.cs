using UnityEngine.UI;

namespace Game
{
    public static class GraphicExtensions
    {
        public static void SetAlpha(this Graphic self, float a)
        {
            var color = self.color;
            color.a = a;
            self.color = color;
        }
    }
}