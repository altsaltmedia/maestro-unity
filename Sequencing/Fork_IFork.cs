using System.Collections.Generic;

namespace AltSalt.Maestro.Sequencing
{
    public interface Fork_IFork
    {
        List<BranchingPath> branchingPaths
        {
            get;
            set;
        }
    }
}