using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFTCFiniteAutomata.Models
{
    class Transition
    {
        public Transition(string startState, char token, string endState)
        {
            StartState = startState;
            Token = token;
            EndState = endState;
        }

        public string StartState { get; private set; }
        public char Token { get; private set; }
        public string EndState { get; private set; }

        public override string ToString()
        {
            return string.Format("({0}, {1}) -> {2}", StartState, Token, EndState);
        }
    }
}
