using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SuperHash
{
    public class SuperHash<T> where T : HashAlgorithm, new()
    {
        public SuperHash(string fileName)
        {
            const long bufferSize = 1048576;
            const int nodes = 8;
            const long step = bufferSize * (nodes - 1);
            _chunks = new List<Chunk>();
            _chunksRo = new ReadOnlyCollection<Chunk>(_chunks);
            Finished = delegate { };
            _nodes = new[]
                {
                    new SuperHashNode(new T(), fileName, bufferSize, 0, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 2, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 3, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 4, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 5, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 6, step),
                    new SuperHashNode(new T(), fileName, bufferSize, bufferSize * 7, step),
                };
        }

        public void Start()
        {
            var maxCount = 0;
            var doneCount = 0;
            foreach (var superHashNode in _nodes)
            {
                superHashNode.Finished += count =>
                                              {
                                                  Interlocked.Increment(ref doneCount);
                                                  maxCount = Math.Max(count, maxCount);
                                                  if (doneCount == _nodes.Length)
                                                  {
                                                      for (var i = 0; i < maxCount; i++)
                                                      {
                                                          foreach (var hashNode in _nodes)
                                                          {
                                                              if (hashNode.Chunks.Count > i)
                                                                  _chunks.Add(hashNode.Chunks[i]);
                                                          }
                                                      }
                                                      Finished(_chunksRo);
                                                  }
                                              };
                superHashNode.Start();
            }
        }

        public delegate void FinishedHandle(IList<Chunk> chunks);
        public event FinishedHandle Finished;

        public IList<Chunk> Chunks
        {
            get { return _chunksRo; }
        }

        private List<Chunk> _chunks;
        private ReadOnlyCollection<Chunk> _chunksRo;  
        private SuperHashNode[] _nodes;
    }
}
