using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFTCFiniteAutomata.Models
{
    class FSA
    {
        private readonly List<string> q = new List<string>();
        private readonly List<char> sigma = new List<char>();
        private readonly List<Transition> delta = new List<Transition>();
        private string q0;
        private readonly List<string> f = new List<string>();
        private string longestAccepted = "";

        public List<string> Q => q;

        public List<char> Sigma => sigma;

        internal List<Transition> Delta => delta;

        public string Q0 { get => q0; set => q0 = value; }

        public List<string> F => f;

        public FSA(string fileName){

            try
            {
                var fileContent = System.IO.File.ReadLines(fileName).ToList();
                q = fileContent[0].Split(' ').ToList();
                //Q.ForEach(q => Console.WriteLine(q));
                sigma = fileContent[1].Split(' ').Select(t => t[0]).ToList();
                //Sigma.ForEach(t => Console.WriteLine(t));
                List<List<string>> transitions = fileContent[2].Split(',').Select(t => t.Trim().Split(' ').ToList()).ToList();
                //transitions.ForEach(t => t.ForEach(ti => Console.WriteLine(ti)));
                AddTransitions(transitions.Select(t => new Transition(t[0],t[1][0],t[2])));
                //Delta.ForEach(t => Console.WriteLine(t.ToString()));
                AddInitialState(fileContent[3]);
                //Console.WriteLine(Q0);
                AddFinalStates(fileContent[4].Split(' '));
                //F.ForEach(f => Console.WriteLine(f));


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public FSA(IEnumerable<string> q, IEnumerable<char> sigma, IEnumerable<Transition> delta, string q0, IEnumerable<string> f)
        {
            q = q.ToList();
            sigma = sigma.ToList();
            AddTransitions(delta);
            AddInitialState(q0);
            AddFinalStates(f);
        }

        private void AddTransitions(IEnumerable<Transition> delta)
        {
            delta.ToList().Where(ValidTransition).ToList()
                .ForEach(t => Delta.Add(t));
        }

        private void AddInitialState(string q0)
        {
            if (q.Contains(q0))
            {
                this.q0 = q0;
            }
        }

        private void AddFinalStates(IEnumerable<string> f)
        {
            foreach (var finalState in f.Where(
               finalState => q.Contains(finalState)))
            {
                this.f.Add(finalState);
            }
        }

        private bool ValidTransition(Transition transition){
            return q.Contains(transition.StartState)
            && q.Contains(transition.EndState)
            && sigma.Contains(transition.Token)
            && !TransitionAlreadyDefined(transition);
        }

        private bool TransitionAlreadyDefined(Transition transition)
        {
            return delta.Any(t => t.StartState == transition.StartState 
                                    && t.Token == transition.Token
                                    && t.EndState == transition.EndState);
        }

        public void Accepts(string input)
        {
            Console.WriteLine("Trying to accept: " + input);
            StringBuilder currentSeq= new StringBuilder("");
            if (Accepts(Q0, input, new StringBuilder(), currentSeq))
            {
                return;
            }
            Console.WriteLine("Could not accept the input: " + input + " longest accepted:" + longestAccepted);
        }

        private bool Accepts(string currentState, string input, StringBuilder steps, StringBuilder currentSequence)
        {
            if (input.Length > 0)
            {
                if (f.Contains(currentState) && currentSequence.ToString().Length > longestAccepted.Length)
                {
                    longestAccepted = currentSequence.ToString();
                }
                var transitions = GetAllTransitions(currentState, input[0]);
                foreach (var transition in transitions)
                {
                    var currentSteps = new StringBuilder(steps.ToString() + transition);
                    var currentSeq = new StringBuilder(currentSequence.ToString() + transition.Token);
                    if (Accepts(transition.EndState, input.Substring(1), currentSteps, currentSeq))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (F.Contains(currentState))
            {
                Console.WriteLine("Successfully accepted the input " + input + " " +
                                       "in the final state " + currentState +
                                       " with steps:\n" + steps);
                return true;
            }
            return false;
        }

        private IEnumerable<Transition> GetAllTransitions(string currentState, char token)
        {
            return delta.FindAll(t => t.StartState == currentState &&
                                 t.Token == token);
        }

        internal void PrintQ()
        {
            Q.ForEach(q => Console.WriteLine(q));
        }
        internal void PrintSigma()
        {
            Sigma.ForEach(t => Console.WriteLine(t));
        }
        internal void PrintDelta()
        {
            Delta.ForEach(t => Console.WriteLine(t.ToString()));
        }
        internal void PrintQ0()
        {
            Console.WriteLine(Q0);
        }
        internal void PrintF()
        {
            F.ForEach(f => Console.WriteLine(f));
        }
    }
}
