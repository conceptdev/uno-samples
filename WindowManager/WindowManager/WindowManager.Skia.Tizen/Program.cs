using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace WindowManager.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new WindowManager.App(), args);
            host.Run();
        }
    }
}
