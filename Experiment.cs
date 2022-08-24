using System;

namespace Sample
{
    public ref struct Experiment
    {
        Span<T> span;
        int size;
        int genes;

        public double Fitness()
        {
            
        }

        public void InitRandom()
        {
            
        }

        public void Repair()
        {
            
        }
    }

    public unsafe struct Experiment2
    {
        public int Genes; 
        public fixed float[5] genes;
    }
}
