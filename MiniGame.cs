using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinigameIdle
{
    public abstract class MiniGame
    {
        public abstract void Initialize();

        public abstract void Update();

        public abstract void Draw();
    }
}
