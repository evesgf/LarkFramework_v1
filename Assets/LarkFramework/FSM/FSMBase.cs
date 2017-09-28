using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.LarkFramework.FSM
{
    public class FSMBase
    {
        private readonly string m_Name;

        public FSMBase() : this(null)
        {

        }

        public FSMBase(string name)
        {
            m_Name = name ?? string.Empty;
        }
    }
}
