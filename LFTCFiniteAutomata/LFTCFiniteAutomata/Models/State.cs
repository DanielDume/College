using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFTCFiniteAutomata.Models
{
    public class State
    {
        public string Name { get; set; }

        public State(string name)
        {
            Name = name;
        }

        public State()
        {
        }

        public override bool Equals(object obj)
        {
            var state = obj as State;
            return state != null &&
                   Name == state.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
