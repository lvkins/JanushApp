using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A class that handles the file system tasks.
    /// </summary>
    public class FileManager
    {
        private ConcurrentDictionary<string, bool> _fileDict;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public async Task WriteTextToFileAsync(string text, string path, bool append)
        {
            if (_fileDict == null)
            {
                _fileDict = new ConcurrentDictionary<string, bool>();
            }
        }
    }
}
