using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class AvailableSong
    {
        public string FilePath { get; set; }

        private bool isPlayable = true;
        public bool IsPlayable { get { return isPlayable; } set { isPlayable = value; } }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
