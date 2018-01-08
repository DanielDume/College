using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MPI;
using Environment = MPI.Environment;
using System.Diagnostics;

namespace KaratsubaMPI
{
    [Serializable]
    public sealed class Polynomial
    {
        public static Random rnd = new Random();
        public static Stopwatch sw = new Stopwatch();
        public static readonly Random R = new Random();

        public readonly int Degree;
        public readonly int[] Coefficients;
        public Polynomial(int degree)
        {
            if (degree < 0)
                throw new InvalidOperationException("Invalid degree specification!");
            Degree = degree;
            Coefficients = new int[degree + 1];

            for (var i = 0; i <= degree; ++i)
                Coefficients[i] = R.Next(-10, 10);
        }
        public Polynomial(params int[] coefficients)
        {
            if (coefficients == null || coefficients.Length == 0)
                throw new InvalidOperationException("Invalid coefficients specification!");

            Degree = coefficients.Length - 1;
            Coefficients = coefficients;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i <= Degree; ++i)
                sb.AppendFormat("{0}{1}*X^{2}", Coefficients[i] >= 0 ? "+" : "", Coefficients[i], i);

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            var polynomial = obj as Polynomial;

            if (Degree != polynomial?.Degree)
                return false;

            for (var i = 0; i <= Degree; ++i)
                if (Coefficients[i] != polynomial.Coefficients[i])
                    return false;

            return true;
        }

        // Auto implemented by ReSharper.
        public override int GetHashCode()
        {
            unchecked
            {
                return (Degree * 397) ^ Coefficients.GetHashCode();
            }
        }

        public static Polynomial RandomPolynomial(int degree)
        {
            if (degree < 0)
                throw new InvalidOperationException("Invalid degree specification!");

            var coefficients = new int[degree + 1];

            for (var i = 0; i <= degree; ++i)
                coefficients[i] = R.Next(1, 10);

            return new Polynomial(coefficients);
        }
        public static Polynomial Shift(Polynomial p, int offset)
        {
            if (offset < 0)
                throw new InvalidOperationException("Invalid offset specification!");

            var coefficients = new int[p.Degree + 1 + offset];
            Array.Copy(p.Coefficients, 0, coefficients, offset, p.Degree + 1);
            return new Polynomial(coefficients);
        }

        public static Polynomial operator <<(Polynomial p, int offset)
        {
            return Shift(p, offset);
        }

        // Simple addition. No need for parallelization.
        public static Polynomial Add(Polynomial a, Polynomial b)
        {
            var min = Math.Min(a.Degree, b.Degree);
            var max = Math.Max(a.Degree, b.Degree);

            var coefficients = new int[max + 1];

            for (var i = 0; i <= min; ++i)
                coefficients[i] = a.Coefficients[i] + b.Coefficients[i];

            for (var i = min + 1; i <= max; ++i)
                if (i <= a.Degree)
                    coefficients[i] = a.Coefficients[i];
                else
                    coefficients[i] = b.Coefficients[i];

            return new Polynomial(coefficients);
        }

        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            return Add(a, b);
        }

        // Simple subtraction. No need for parallelization.
        public static Polynomial Subtract(Polynomial a, Polynomial b)
        {
            var min = Math.Min(a.Degree, b.Degree);
            var max = Math.Max(a.Degree, b.Degree);

            var coefficients = new int[max + 1];

            for (var i = 0; i <= min; ++i)
                coefficients[i] = a.Coefficients[i] - b.Coefficients[i];

            for (var i = min + 1; i <= max; ++i)
                if (i <= a.Degree)
                    coefficients[i] = a.Coefficients[i];
                else
                    coefficients[i] = -b.Coefficients[i];

            var degree = coefficients.Length - 1;
            while (coefficients[degree] == 0 && degree > 0)
                degree--;

            var clean = new int[degree + 1];
            Array.Copy(coefficients, 0, clean, 0, degree + 1);
            return new Polynomial(clean);
        }

        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            return Subtract(a, b);
        }

        // Serial multiplication.
        public static Polynomial Multiply(Polynomial a, Polynomial b)
        {
            var coefficients = new int[a.Degree + b.Degree + 1];

            for (var i = 0; i <= a.Degree; ++i)
                for (var j = 0; j <= b.Degree; ++j)
                    coefficients[i + j] += a.Coefficients[i] * b.Coefficients[j];

            return new Polynomial(coefficients);
        }

        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            return Multiply(a, b);
        }

        [Serializable]
        internal struct VectorElement
        {
            public int Index;
            public int Value;
        }

        [Serializable]
        internal struct MatrixElement
        {
            public int Row;
            public int Column;
            public int Value;
        }
        public int[,,] GetSplitIndexes(int length, int nrWorkers)
        {
            var res = new List<Tuple<int, int>>();
            var counter = 0;
            for (int i = 0; i <= length; i++)
            {
                for (int j = 0; j <= length; j++)
                {

                }
            }
            return null;
        }
        //public int GetFirstPolIndexStart (int length)
        public static void RegMult(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;
                Polynomial x = null, y = null;
                int n;
                if (comm.Rank == 0)
                {
                    const int degree = 2;
                    x = new Polynomial(new int[] { 1, 2, 3 });
                    y = new Polynomial(new int[] { 4, 5, 6 });
                    n = degree + 1;

                    comm.Broadcast(ref x, 0);
                    comm.Broadcast(ref y, 0);

                    var coefficients = new int[x.Degree + y.Degree + 1];

                    for (int i = 0; i < (x.Degree + 1) * (y.Degree + 1); i++)
                    {
                        var res = comm.Receive<VectorElement>(Communicator.anySource, 1);
                        coefficients[res.Index] += res.Value;
                    }

                    var result = new Polynomial(coefficients);
                    Console.WriteLine(result.ToString());
                }
                else
                {
                    comm.Broadcast(ref x, 0);
                    comm.Broadcast(ref y, 0);
                    int start = (comm.Rank - 1) * ((x.Degree + 1) / (comm.Size - 1));
                    if (comm.Rank < comm.Size - 1)
                    {
                        int end = start + ((x.Degree + 1) / (comm.Size - 1));
                        for (int i = start; i < end; i++)
                        {
                            for (int j = 0; j <= y.Degree; j++)
                            {
                                comm.Send(new VectorElement { Index = i + j, Value = x.Coefficients[i] * y.Coefficients[j] }, 0, 1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = start; i <= x.Degree; i++)
                        {
                            for (int j = 0; j <= y.Degree; j++)
                            {
                                comm.Send(new VectorElement { Index = i + j, Value = x.Coefficients[i] * y.Coefficients[j] }, 0, 1);
                            }
                        }
                    }
                }
            }
        }

        public static void KaratsubaMain(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;
                Polynomial x = null, y = null;
                int n;
                if (comm.Rank == 0)
                {
                    //x = new Polynomial(new int[] { 1, 2, 3, 4, 5});
                    //y = new Polynomial(new int[] { 6, 7, 8, 9, 10});
                    x = RandomPolynomial(10);
                    y = RandomPolynomial(10);
                    KaratsubaMaster(x, y, comm.Size - 1, comm);
                }
                else
                {
                    KaratsubaWorker(comm.Rank, comm.Size - 1, comm, comm.Rank - 1);
                }
            }
        }

        private static void KaratsubaWorker(int rank, int pr, Communicator comm, int caller)
        {
            //throw new NotImplementedException();
            int sizeA, sizeB;
            var a = comm.Receive<Polynomial>(Communicator.anySource, 1);
            var b = comm.Receive<Polynomial>(Communicator.anySource, 2);
            Polynomial p = Karatsuba(a, b, comm.Rank, pr, comm);
            comm.Send<Polynomial>(p, comm.Rank, 3);

        }

        private static void KaratsubaMaster(Polynomial x, Polynomial y, int pr, Communicator comm)
        {
            //throw new NotImplementedException();
            Polynomial p;
            sw.Reset();
            sw.Start();
            p = Karatsuba(x, y, 0, pr, comm);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(p.ToString());
        }

        private static Polynomial Karatsuba(Polynomial x, Polynomial y, int v, int pr, Communicator comm)
        {
            //throw new NotImplementedException();
            const int threshold = 2;

            if (x.Degree <= threshold || y.Degree <= threshold)
                return Multiply(x, y);

            var length = Math.Max(x.Degree + 1, y.Degree + 1);
            length = length / 2;


            var low1 = new int[length];
            Array.Copy(x.Coefficients, 0, low1, 0, low1.Length);

            var high1 = new int[x.Degree + 1 - length];
            Array.Copy(x.Coefficients, length, high1, 0, high1.Length);

            var low2 = new int[length];
            Array.Copy(y.Coefficients, 0, low2, 0, low2.Length);

            var high2 = new int[y.Degree + 1 - length];
            Array.Copy(y.Coefficients, length, high2, 0, high2.Length);

            var h1 = new Polynomial(high1);
            var l1 = new Polynomial(low1);
            var h2 = new Polynomial(high2);
            var l2 = new Polynomial(low2);
            var lh1 = l1 + h1;
            var lh2 = l2 + h2;
            Polynomial r1 = null, r2 = null, r3 = null;
            int next = 3 * comm.Rank + 1;
            if (pr >= next + 2)
            {

                comm.Send(l1, next, 1);
                comm.Send(l2, next, 2);

                comm.Send(lh1, next + 1, 1);
                comm.Send(lh2, next + 1, 2);

                comm.Send(h1, next + 2, 1);
                comm.Send(h2, next + 2, 2);

                r1 = comm.Receive<Polynomial>(next, 3);
                r2 = comm.Receive<Polynomial>(next + 1, 3);
                r3 = comm.Receive<Polynomial>(next + 2, 3);

            }
            else
            {

                r1 = Karatsuba(l1, l2, v, pr, comm);
                r2 = Karatsuba(lh1, lh2, v, pr, comm);
                r3 = Karatsuba(h1, h2, v, pr, comm);

            }

            return (r3 << (2 * length)) + ((r2 - r3 - r1) << length) + r1;

        }
        //public static List<int> variables;
        public static void PrintList(List<int> var)
        {
            for (int i = 0; i < var.Count; i++)
            {
                Console.WriteLine("{0}: {1}", i, var[i]);
            }
        }        
        [Serializable]
        public enum Operations
        {
            LOCK, UNLOCK, READ, WRITE, DONE
        }
        [Serializable]
        public class OperationsMessage
        {
            public Operations Operation;
            public int Sender;
        }
        public static string VariablesString(List<int> var)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < var.Count; i++)
            {
                s.Append(String.Format("{0} : {1}\n", i, var[i]));
            }
            return s.ToString();
        }
        public static void NewMain(string[] args)
        {
            using (new Environment(ref args))
            {
                List<int> variables = null;
                List<int> ownership = null;
                var comm = Communicator.world;
                if (comm.Rank == 0) //main
                {
                    variables = new List<int>(new int[] { 0, 0, 0 });
                    ownership = new List<int>(new int[] { 0, 0, 0 });
                    int Processes = comm.Size - 1;
                    bool done = false;
                    comm.Broadcast(ref variables, 0);
                    while (!done)
                    {
                        var op = comm.Receive<OperationsMessage>(Communicator.anySource, 1);
                        switch (op.Operation)
                        {
                            case Operations.LOCK:
                                Console.WriteLine("LOCK request from : {0}", op.Sender);
                                HandleLock(op.Sender, ownership, comm);
                                break;

                            case Operations.UNLOCK:
                                Console.WriteLine("UNLOCK request from : {0}", op.Sender);
                                HandleUnlock(op.Sender, ownership, comm);
                                break;

                            case Operations.READ:
                                Console.WriteLine("READ request from : {0}", op.Sender);
                                HandleRead(op.Sender, ownership, comm, variables);
                                break;

                            case Operations.WRITE:
                                Console.WriteLine("WRITE request from : {0}", op.Sender);
                                HandleWrite(op.Sender, ownership, comm, variables);
                                break;

                            case Operations.DONE:
                                Console.WriteLine("Process {0} has terminated", op.Sender);
                                Processes--;
                                if (Processes == 0)
                                {
                                    Console.WriteLine("All processes are done");
                                    done = true;
                                    PrintList(variables);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
                else
                {
                    variables = new List<int>(new int[] { 0, 0, 0 });
                    switch (comm.Rank)
                    {
                        case 1:
                            Lock(comm.Rank, 0, comm);
                            Thread.Sleep(rnd.Next() % 1000);
                            int a = Read(comm.Rank, 0, comm);
                            a = a + 1;
                            Write(comm.Rank, 0, comm, a);
                            //variables[0] = a;
                            Unlock(comm.Rank, 0, comm);
                            break;
                        case 2:
                            Lock(comm.Rank, 1, comm);
                            Thread.Sleep(rnd.Next() % 1000);
                            int b = Read(comm.Rank, 1, comm);
                            b = b + 7;
                            Write(comm.Rank, 1, comm, b);
                            Unlock(comm.Rank, 1, comm);
                            Lock(comm.Rank, 0, comm);
                            Thread.Sleep(rnd.Next() % 1000);
                            int d = Read(comm.Rank, 0, comm);
                            d = d + 10;
                            Write(comm.Rank, 0, comm, d);
                            //variables[0] = a;
                            Unlock(comm.Rank, 0, comm);
                            break;
                        case 3:
                            Lock(comm.Rank, 2, comm);
                            Thread.Sleep(rnd.Next() % 1000);
                            int c = Read(comm.Rank, 2, comm);
                            c = c + 14;
                            Write(comm.Rank, 2, comm, c);
                            Unlock(comm.Rank, 2, comm);
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("Printing variable status from process : {0} \n{1}", comm.Rank, VariablesString(variables));
                    //PrintList(variables);
                    comm.Send(new OperationsMessage { Operation = Operations.DONE, Sender = comm.Rank }, 0, 1);
                }
            }
        }

        private static int Read(int rank, int v, Communicator comm)
        {
            Console.WriteLine("Send READ request for variable {0} by process {1}", v, rank);
            Operations op = Operations.READ;
            comm.Send(new OperationsMessage { Operation = op, Sender = rank }, 0, 1);
            comm.Send(v, 0, 2);
            var res = comm.Receive<int>(0, 4);
            return res;
        }

        private static void Write(int rank, int v, Communicator comm, int newVal)
        {
            Console.WriteLine("Send WRITE request for variable {0} by process {1}", v, rank);
            Operations op = Operations.WRITE;
            comm.Send(new OperationsMessage { Operation = op, Sender = rank }, 0, 1);
            comm.Send(v, 0, 2);
            comm.Send(newVal, 0, 4);
        }

        private static void Unlock(int rank, int v, Communicator comm)
        {
            Console.WriteLine("Send UNLOCK request for variable {0} by process {1}", v, rank);
            Operations op = Operations.UNLOCK;
            comm.Send(new OperationsMessage { Operation = op, Sender = rank }, 0, 1);
            comm.Send(v, 0, 2);
        }

        private static void Lock(int rank, int v, Communicator comm)
        {
            Console.WriteLine("Send LOCK request for variable {0} by process {1}", v, rank);
            bool locked = false;
            var op = Operations.LOCK;
            while (!locked)
            {
                comm.Send(new OperationsMessage { Operation = op, Sender = rank }, 0, 1);
                comm.Send(v, 0, 2);
                locked = comm.Receive<bool>(0, 3);
                if (!locked)
                {
                    Thread.Sleep(rnd.Next() % 1000);
                }
            }
            Console.WriteLine("Succesfully locked variable {0} for process {1}", v, rank);
        }

        private static void HandleWrite(int sender, List<int> ownership, Communicator comm, List<int> variables)
        {
            var variableIndex = comm.Receive<int>(sender, 2);
            variables[variableIndex] = comm.Receive<int>(sender, 4);
            //Read(sender, variableIndex, comm);            
        }

        private static void HandleRead(int sender, List<int> ownership, Communicator comm, List<int> variables)
        {
            var variableIndex = comm.Receive<int>(sender, 2);
            comm.Send(variables[variableIndex], sender, 4);
        }

        private static void HandleLock(int sender, List<int> ownership, Communicator comm)
        {
            bool locked;
            var variableIndex = comm.Receive<int>(sender, 2);
            if (ownership[variableIndex] == 0 || ownership[variableIndex] == sender) 
            {
                ownership[variableIndex] = sender;
                locked = true;
            }
            else
            {
                locked = false;
            }
            comm.Send(locked, sender, 3);
        }

        private static void HandleUnlock(int sender, List<int> ownership, Communicator comm)
        {
            var variableIndex = comm.Receive<int>(sender, 2); 
            if (ownership[variableIndex] == sender)
            {
                ownership[variableIndex] = 0;
            }
        }

        public static void Main(string[] args)
        {
            NewMain(args);
            //RegMult(args);
            //KaratsubaMain(args);
            //var d = DateTime.Now;
            //d = d.AddDays(-254);
            //Console.WriteLine(d.ToString());


        }
    }
}
