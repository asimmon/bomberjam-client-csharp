using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    public class Gamelog : IEnumerable<GameStateStep>
    {
        private readonly string _path;

        public Gamelog(string path)
        {
            this._path = path;
        }

        private IEnumerable<GameStateStep> GetEnumerable()
        {
            using (var fileStream = File.OpenRead(this._path))
            using (var fileReader = new StreamReader(fileStream, true))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        yield break;

                    yield return JsonConvert.DeserializeObject<GameStateStep>(line.Trim());
                }
            }
        }

        public IEnumerator<GameStateStep> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}