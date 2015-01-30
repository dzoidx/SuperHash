using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SuperHash
{
    public class SuperHashNode
    {
        public SuperHashNode(HashAlgorithm hash, string fileName, long bufferSize, long offset, long step)
        {
            _fromOffset = offset;
            _chunks = new List<Chunk>();
            _chunksRo = new ReadOnlyCollection<Chunk>(_chunks);
            _buffer = new byte[bufferSize];
            _hash = hash;
            _step = step;
            _stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _stream.Seek(offset, SeekOrigin.Begin);
            Finished = delegate { };
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
                                      {
                                          while (true)
                                          {
                                              var read = _stream.Read(_buffer, 0, _buffer.Length);
                                              _toOffset = _fromOffset + read;
                                              if (read == 0) break;
                                              _chunks.Add(
                                                  new Chunk { FromOffset = _fromOffset, ToOffset = _toOffset, Hash = _hash.ComputeHash(_buffer, 0, read) }
                                                  );
                                              _stream.Seek(_step, SeekOrigin.Current);
                                              _fromOffset += _step;
                                          }
                                          Finished(_chunks.Count);
                                      });
        }

        public delegate void FinishedHandle(int count);
        public event FinishedHandle Finished;

        public IList<Chunk> Chunks
        {
            get { return _chunksRo; }
        }

        private long _fromOffset;
        private long _toOffset;
        private long _step;
        private List<Chunk> _chunks;
        private ReadOnlyCollection<Chunk> _chunksRo;  
        private Stream _stream;
        private HashAlgorithm _hash;
        private byte[] _buffer;
    }
}
