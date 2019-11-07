using System.Collections.Generic;

namespace AltSalt.Sequencing
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