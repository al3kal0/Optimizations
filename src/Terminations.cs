

namespace Optimization
{
    ///<summary> defines a stopping condition generic class, <br/>
    /// NO need for <see cref="Chromosome"/> to be Specific </summary>
    public class MaxGenerationTermination<T> : ITermination<T> where T: unmanaged, IChromosome
    {
        int generation = 0;
        public int MaxGeneration {get; set;} = 20;

        public bool Terminate(GeneticAlg<T> algorithm)
        {
            return generation++ > MaxGeneration;
        }
    
    }        
}
