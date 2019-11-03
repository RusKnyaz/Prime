using System.IO;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.WinForms.Tools
{
    public class StaticResourceProvider : IResourceProvider
    {
        public StaticResourceProvider(string data) =>
            Data = data;

        public string Data { get; }

        public Task<IResource> SendRequestAsync(Request request) =>
            Task.Run(() => (IResource)new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(Data))));
    }
}