using System.Collections.Generic;

namespace AltSalt.Sequencing
{
    public interface JoinTools_IFork
    {
        List<JoinTools_BranchingPath> branchingPaths
        {
            get;
            set;
        }
    }
}